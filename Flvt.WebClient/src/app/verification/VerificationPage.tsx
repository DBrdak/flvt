import React, { useState, useRef, useEffect } from 'react'
import { Box, Typography, Button, TextField } from '@mui/material'
import { styled } from '@mui/material/styles'
import MuiCard from '@mui/material/Card'
import { useStore } from '../../stores/store'
import { observer } from 'mobx-react-lite'
import { toast } from 'react-toastify'
import { useNavigate } from 'react-router-dom'
import LoadingPage from "../sharedComponents/LoadingPage.tsx";

const Card = styled(MuiCard)(({ theme }) => ({
    display: 'flex',
    flexDirection: 'column',
    alignSelf: 'center',
    width: '100%',
    padding: theme.spacing(4),
    gap: theme.spacing(2),
    boxShadow:
        'hsla(220, 30%, 5%, 0.05) 0px 5px 15px 0px, hsla(220, 25%, 10%, 0.05) 0px 15px 35px -5px',
    [theme.breakpoints.up('sm')]: {
        width: '450px'
    },
    ...theme.applyStyles('dark', {
        boxShadow:
            'hsla(220, 30%, 5%, 0.5) 0px 5px 15px 0px, hsla(220, 25%, 10%, 0.08) 0px 15px 35px -5px'
    })
}))

function VerificationPage() {
    const { subscriberStore } = useStore()
    const navigate = useNavigate()
    const [verificationCode, setVerificationCode] = useState(['', '', '', '', '', ''])
    const [isResendAvailable, setIsResendAvailable] = useState<boolean>(false)
    const [resendCountdown, setResendCountdown] = useState<number>(60)
    const inputsRef = useRef<HTMLInputElement[]>([])

    useEffect(() => {
        if (resendCountdown > 0) {
            const timer = setTimeout(() => setResendCountdown(resendCountdown - 1), 1000)
            return () => clearTimeout(timer)
        } else {
            setIsResendAvailable(true)
        }
    }, [resendCountdown])

    const handleInputChange = (value: string, index: number) => {
        const digit = value.replace(/\D/g, '')
        if (!digit) return

        const newCode = [...verificationCode].filter(c => c !== '')
        newCode.push(value)

        for (let i = newCode.length; i < 6; i++) {
            newCode[i] = ''
        }

        setVerificationCode(newCode)

        if (index < 5 && digit) {
            inputsRef.current[index + 1]?.focus()
        }

        if (index === 5 && newCode.every((d) => d !== '')) {
            handleSubmit(newCode.join(''))
        }
    }

    const handleKeyDown = (e: React.KeyboardEvent<HTMLDivElement>, index: number) => {
        if (e.key === 'Backspace' && !verificationCode[index]) {
            if (index > 0) {
                inputsRef.current[index - 1]?.focus()
            }
        }
    }

    const handlePaste = (e: React.ClipboardEvent<HTMLInputElement>) => {
        e.preventDefault()
        const pasteData = e.clipboardData.getData('Text').replace(/\D/g, '').slice(0, 6)
        if (pasteData.length === 6) {
            setVerificationCode(pasteData.split(''))
            handleSubmit(pasteData)
        }
    }

    const handleSubmit = async (code: string) => {
        const verificationResult = await subscriberStore.verifyEmailAsync(code)

        if (!verificationResult) {
            toast.error('Verification failed')
            setVerificationCode(['', '', '', '', '', '']) // Clear inputs on failure
            inputsRef.current[0]?.focus()
            return
        }

        navigate('/filters')
    }

    const resendCode = async () => {
        if (!isResendAvailable) return

        setIsResendAvailable(false)
        setResendCountdown(60)

        const resendResult = await subscriberStore.resendVerificationEmailAsync(subscriberStore.currentSubscriber!.email)

        if (resendResult) {
            toast.success('Verification code resent')
        } else {
            toast.error('Failed to resend verification code')
        }
    }

    return (
        subscriberStore.loading === 'verify' ?
            <LoadingPage variant={'spinner'} />
            :
            <Box
                component="main"
                sx={(theme) => ({
                    minHeight: '100vh',
                    minWidth: '100vw',
                    display: 'flex',
                    justifyContent: 'center',
                    alignItems: 'center',
                    backgroundImage:
                        'radial-gradient(ellipse at 50% 50%, hsl(210, 100%, 97%), hsl(0, 0%, 100%))',
                    backgroundRepeat: 'no-repeat',
                    ...theme.applyStyles('dark', {
                        backgroundImage: 'radial-gradient(at 50% 50%, hsla(210, 100%, 16%, 0.5), hsl(220, 30%, 5%))'
                    })
                })}
            >
                <Card variant="outlined" sx={{ p: 4 }}>
                    <Box sx={{ width: '100%', p: 2 }}>
                        <Typography variant="h5">
                            Check your inbox and type the verification code below
                        </Typography>
                    </Box>
                    <Box sx={{ display: 'flex', gap: 2, justifyContent: 'center' }}>
                        {verificationCode.map((digit, index) => (
                            <TextField
                                key={index}
                                inputRef={(el) => (inputsRef.current[index] = el!)}
                                value={digit}
                                onChange={(e) => handleInputChange(e.target.value, index)}
                                onKeyDown={(e) => handleKeyDown(e, index)}
                                onPaste={handlePaste}
                                inputProps={{
                                    maxLength: 1,
                                    style: { textAlign: 'center', fontSize: '1rem', width: '3rem', caretColor: 'transparent' }
                                }}
                                variant="outlined"
                            />
                        ))}
                    </Box>
                    <Box sx={{ padding: '1em', display: 'flex', justifyContent: 'center', gap: 2 }}>
                        <Button
                            variant="outlined"
                            onClick={resendCode}
                            disabled={!isResendAvailable}
                        >
                            {isResendAvailable ? 'Resend code' : `Resend available in ${resendCountdown}s`}
                        </Button>
                    </Box>
                </Card>
            </Box>
    )
}

export default observer(VerificationPage)
