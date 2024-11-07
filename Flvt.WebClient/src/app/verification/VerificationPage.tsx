import { observer } from "mobx-react-lite";
import { useStore } from "../../stores/store.ts";
import * as Yup from "yup";
import { verificationCodePattern } from "../../utils/constants/bussinessRules.ts";
import { toast } from "react-toastify";
import { useNavigate } from "react-router-dom";
import { Form, Formik } from "formik";
import { Box, Card, Paper, Typography, Button } from "@mui/material";
import TextInput from "../sharedComponents/TextInput.tsx";
import { useEffect, useState } from "react";
import LoadingPage from "../sharedComponents/LoadingPage.tsx";

function VerificationPage() {
    const { subscriberStore } = useStore();
    const navigate = useNavigate();
    const [isResendAvailable, setIsResendAvailable] = useState<boolean>(false);
    const [resendCountdown, setResendCountdown] = useState<number>(60); // countdown timer for resend button

    const initialValues = { verificationCode: '' };
    const validationSchema = Yup.object({
        verificationCode: Yup.string().required('Verification code is required').matches(verificationCodePattern, 'Invalid verification code')
    });

    const handleSubmit = async (values: { verificationCode: string }) => {
        const verificationResult = await subscriberStore.verifyEmailAsync(values.verificationCode);

        if (!verificationResult) {
            toast.error('Verification failed');
            return;
        }

        navigate('/filters');
    };

    useEffect(() => {
        if (resendCountdown > 0) {
            const timer = setTimeout(() => setResendCountdown(resendCountdown - 1), 1000);
            return () => clearTimeout(timer);
        } else {
            setIsResendAvailable(true); // enable the resend button
        }
    }, [resendCountdown]);

    const resendCode = async () => {
        if(!isResendAvailable){
            return
        }

        setIsResendAvailable(false)
        setResendCountdown(60)

        const resendResult = await subscriberStore.resendVerificationEmailAsync(subscriberStore.currentSubscriber!.email);

        if (resendResult) {
            toast.success('Verification code resent');
        } else {
            toast.error('Failed to resend verification code');
        }
    };

    useEffect(() => {
        const loadSubscriber = async () => {
            if(!subscriberStore.currentSubscriber) {
                await subscriberStore.loadCurrentSubscriberAsync()
            }

            if(!subscriberStore.currentSubscriber) {
                navigate('/')
                return
            }

            if(subscriberStore.currentSubscriber!.isEmailVerified) {
                navigate('/filters')
            }
        }

        loadSubscriber()
    }, []);

    return (
        ['verify', 'init', 'resendCode'].some(action => action === subscriberStore.loading) ?
            <LoadingPage variant={'spinner'} />
            :
            <Box
                component="main"
                sx={[
                    (theme) => ({
                        minHeight: '100vh',
                        minWidth: '100vw',
                        display: 'flex',
                        justifyContent: 'center',
                        alignItems: 'center',
                        backgroundImage:
                            'radial-gradient(ellipse at 50% 50%, hsl(210, 100%, 97%), hsl(0, 0%, 100%))',
                        backgroundRepeat: 'no-repeat',
                        ...theme.applyStyles('dark', {
                            backgroundImage:
                                'radial-gradient(at 50% 50%, hsla(210, 100%, 16%, 0.5), hsl(220, 30%, 5%))',
                        })
                    })
                ]}>
                <Card variant="outlined" sx={{ p: 4 }}>
                    <Box sx={{ width: '100%', p: 2 }}>
                        <Typography variant={'h5'}>Check your inbox and type the verification code below</Typography>
                    </Box>
                    <Formik
                        initialValues={initialValues}
                        onSubmit={handleSubmit}
                        validationSchema={validationSchema}
                    >
                        {({ errors }) => (
                            <Form
                                autoComplete={'off'}
                                style={{
                                    width: '100%',
                                    display: 'flex',
                                    justifyContent: 'center',
                                    gap: '2em',
                                    alignItems: 'center',
                                    flexDirection: 'column',
                                    position: 'relative',
                                }}
                            >
                                <Box sx={{ width: '100%' }}>
                                    <TextInput
                                        placeholder={''}
                                        type={'text'}
                                        name={'verificationCode'}
                                        errorMessage={errors.verificationCode}
                                    />
                                </Box>
                                <Box sx={{ padding: '1em', display: 'flex', justifyContent: 'center', gap: 2 }}>
                                    <Button
                                        variant="outlined"
                                        onClick={resendCode}
                                        disabled={!isResendAvailable}
                                    >
                                        {isResendAvailable ? 'Resend code' : `Resend available in ${resendCountdown}s`}
                                    </Button>
                                    <Button variant="contained" type="submit">
                                        Submit
                                    </Button>
                                </Box>
                            </Form>
                        )}
                    </Formik>
                </Card>
            </Box>
    );
}

export default observer(VerificationPage);
