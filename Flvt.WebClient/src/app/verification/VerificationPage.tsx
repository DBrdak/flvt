import {observer} from "mobx-react-lite";
import {useStore} from "../../stores/store.ts";
import * as Yup from "yup";
import {verificationCodePattern} from "../../utils/constants/bussinessRules.ts";
import {toast} from "react-toastify";
import {useNavigate} from "react-router-dom";
import {Form, Formik} from "formik";
import {Box} from "@mui/material";
import FormLabel from "@mui/material/FormLabel";
import TextInput from "../sharedComponents/TextInput.tsx";
import Button from "@mui/material/Button";
import {useEffect} from "react";

function VerificationPage() {
    const {subscriberStore} = useStore()
    const navigate = useNavigate();

    const initialValues = {verificationCode: ''}
    const validationSchema = Yup.object({
        verificationCode: Yup.string().required('Verification code is required').matches(verificationCodePattern, 'Invalid verification code')
    })

    const handleSubmit = async (values: {verificationCode: string}) => {
        const verificationResult = await subscriberStore.verifyEmailAsync(values.verificationCode)

        if (!verificationResult) {
            toast.error('Verification failed')
            return
        }

        navigate('/filters')
    }

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
    }, [])

    return (
        <Box
            component="main"
            sx={[
                (theme) => ({
                    minHeight: '100vh',
                    minWidth: '100vw',
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
            <Formik sx={{width: '100%'}} initialValues={initialValues} onSubmit={handleSubmit} validationSchema={validationSchema}>
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
                        <Box sx={{width: '100%'}}>
                            <FormLabel>Verification Code</FormLabel>
                            <TextInput
                                placeholder={''}
                                type={'text'}
                                name={'verificationCode'}
                                errorMessage={errors.verificationCode}
                            />
                        </Box>
                        <Box sx={{ padding: '1em', display: 'flex', justifyContent: 'center' }}>
                            <Button variant="contained" type="submit">
                                Submit
                            </Button>
                        </Box>
                    </Form>
                )}
            </Formik>
        </Box>
    )
}

export default observer(VerificationPage)