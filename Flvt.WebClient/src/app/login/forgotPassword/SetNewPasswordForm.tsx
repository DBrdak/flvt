import Button from '@mui/material/Button';
import {Box, TextField, Typography} from "@mui/material";
import {useStore} from "../../../stores/store.ts";
import {observer} from "mobx-react-lite";
import {Form, Formik} from "formik";
import FormLabel from "@mui/material/FormLabel";
import TextInput from "../../sharedComponents/TextInput.tsx";
import * as Yup from "yup";
import {passwordPattern} from "../../../utils/constants/bussinessRules.ts";
import React, {useRef, useState} from "react";

interface Props {
    onSubmit: (values: {password: string, verificationCode: string}) => void,
}

function SetNewPasswordForm({onSubmit}: Props) {
    const {modalStore} = useStore()
    const [verificationCode, setVerificationCode] = useState(['', '', '', '', '', ''])
    const inputsRef = useRef<HTMLInputElement[]>([])

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
        }
    }

    const setInitialValues = {password: ''}
    const setValidationSchema = Yup.object({
        password: Yup.string().required('New password is required').matches(passwordPattern, 'Password is too weak')
    })

    return (
        <Formik sx={{width: '100%'}} initialValues={setInitialValues} onSubmit={(values) => onSubmit({password: values.password, verificationCode: verificationCode.join('')})}
                validationSchema={setValidationSchema}>
            {({errors}) => (
                <Form autoComplete={'off'} style={{
                    width: '100%',
                    display: 'flex',
                    justifyContent: 'center',
                    gap: '2em',
                    alignItems: 'center',
                    flexDirection: 'column',
                    position: 'relative',
                }}>
                    <Typography variant={'subtitle1'} sx={{padding: '1rem'}}>
                        Check your inbox and pass the details below.
                    </Typography>
                    <Box sx={{width: '100%'}}>
                        <FormLabel>New Password</FormLabel>
                        <TextInput
                            placeholder={''}
                            type={'password'}
                            name={'password'}
                            errorMessage={errors.password}
                        />
                    </Box>
                    <Box sx={{width: '100%'}}>
                        <FormLabel>Verification Code</FormLabel>
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
                    </Box>
                    <Box sx={{padding: '1em', display: 'flex', justifyContent: 'space-around'}}>
                        <Button onClick={modalStore.closeModal} style={{marginRight: '0.5em'}}>
                            Cancel
                        </Button>
                        <Button variant="contained" type="submit" style={{marginLeft: '0.5em'}}>
                            Continue
                        </Button>
                    </Box>
                </Form>
            )}
        </Formik>
    )
}

export default observer(SetNewPasswordForm)