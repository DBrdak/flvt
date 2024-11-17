import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import MuiCard from '@mui/material/Card';
import FormLabel from '@mui/material/FormLabel';
import Link from '@mui/material/Link';
import Typography from '@mui/material/Typography';
import { styled } from '@mui/material/styles';
import ForgotPassword from '../forgotPassword/ForgotPassword.tsx';
import {Logo} from "../../sharedComponents/Logo.tsx";
import {useNavigate} from "react-router-dom";
import {useStore} from "../../../stores/store.ts";
import {observer} from "mobx-react-lite";
import {Form, Formik} from "formik";
import * as Yup from 'yup'
import TextInput from "../../sharedComponents/TextInput.tsx";
import {emailAddressPattern, passwordPattern} from "../../../utils/constants/bussinessRules.ts";
import PrivacyPolicyDialog from "../register/PrivacyPolicyDialog.tsx";
import PreviewButton from "../PreviewButton.tsx";

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
        width: '450px',
    },
    ...theme.applyStyles('dark', {
        boxShadow:
            'hsla(220, 30%, 5%, 0.5) 0px 5px 15px 0px, hsla(220, 25%, 10%, 0.08) 0px 15px 35px -5px',
    }),
}));

function LoginCard() {
    const navigate = useNavigate()
    const {subscriberStore, modalStore} = useStore()

    const handleSubmit = async (values: {email: string, password: string}) => {
        const loginResult = await subscriberStore.loginAsync(values)

        if(loginResult){
            navigate('/filters')
        }
    }

    const handleRegister = async (values: {email: string, password: string}) => {
        const registerResult = await subscriberStore.registerAsync(values)

        if(registerResult){
            modalStore.closeModal()
            navigate('/verification')
        }
    }

    const initialValues = {email: '', password: ''}

    const validationSchema = Yup.object({
        email: Yup.string().matches(emailAddressPattern, 'Invalid email address').required('Email is required'),
        password: Yup.string()
            .min(8, 'Password must be at least 8 characters')
            .matches(passwordPattern, 'Password must contain uppercase, lowercase, number, and special character')
            .required('Password is required'),
    });

    return (
        <Card variant="outlined">
            <Box sx={{ display: { xs: 'flex', md: 'none' } }}>
                <Logo size={'xs'} />
            </Box>
            <Typography
                component="h1"
                variant="h4"
                sx={{ width: '100%', fontSize: 'clamp(2rem, 10vw, 2.15rem)' }}
            >
                Sign in
            </Typography>
            <Formik sx={{width: '100%'}} initialValues={initialValues} onSubmit={handleSubmit} validationSchema={validationSchema}>
                {({errors, values, isValid, validateForm}) => (
                    <Form style={{
                        width: '100%',
                        display: 'flex',
                        justifyContent: 'center',
                        gap: '2em',
                        alignItems: 'center',
                        flexDirection: 'column',
                        position: 'relative',
                    }}>
                        <Box sx={{width: '100%'}}>
                            <FormLabel htmlFor="email">Email</FormLabel>
                            <TextInput
                                placeholder={''}
                                type={'email'}
                                autoComplete={'email'}
                                name={'email'}
                                fullwidth
                                errorMessage={errors.email}
                            />
                        </Box>
                        <Box sx={{width: '100%'}}>
                            <FormLabel htmlFor="email">Password</FormLabel>
                            <TextInput
                                placeholder={''}
                                type="password"
                                name={'password'}
                                autoComplete="current-password"
                                fullwidth
                                errorMessage={errors.password}
                            />
                        </Box>
                        <Button type="submit" fullWidth variant="contained">
                            Sign in
                        </Button>
                        <Button fullWidth variant={'outlined'} color={'secondary'}
                                onClick={async () => {
                                    await validateForm()

                                    isValid && modalStore.openModal(
                                        <PrivacyPolicyDialog registerBody={values}
                                                             onSubmit={handleRegister}
                                                             onReject={() => modalStore.closeModal()}/>
                                    )
                                }}>
                            Sign up
                        </Button>
                        <Box>
                            <PreviewButton onClick={() => navigate('/filters/preview/browse')} />
                        </Box>
                    </Form>
                )}
            </Formik>
            <Box sx={{ position: 'relative', bottom: 0, right: 0 }}>
                <Link
                    component="button"
                    type="button"
                    onClick={() => modalStore.openModal(<ForgotPassword />)}
                    variant="body2"
                    sx={{ alignSelf: 'baseline', textDecoration: 'none' }}
                >
                    Forgot your password?
                </Link>
            </Box>
        </Card>
    )
}


export default observer(LoginCard)