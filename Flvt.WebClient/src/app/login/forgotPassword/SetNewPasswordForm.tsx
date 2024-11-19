import Button from '@mui/material/Button';
import {Box, Typography} from "@mui/material";
import {useStore} from "../../../stores/store.ts";
import {observer} from "mobx-react-lite";
import {Form, Formik} from "formik";
import FormLabel from "@mui/material/FormLabel";
import TextInput from "../../sharedComponents/TextInput.tsx";
import * as Yup from "yup";
import {passwordPattern, verificationCodePattern} from "../../../utils/constants/bussinessRules.ts";

interface Props {
    onSubmit: (values: {password: string, verificationCode: string}) => void,
}

function SetNewPasswordForm({onSubmit}: Props) {
    const {modalStore} = useStore()

    const setInitialValues = {password: '', verificationCode: ''}
    const setValidationSchema = Yup.object({
        verificationCode: Yup.string().required('Verification code is required').matches(verificationCodePattern, 'Invalid verification code'),
        password: Yup.string().required('New password is required').matches(passwordPattern, 'Password is too weak')
    })

    return (
        <Formik sx={{width: '100%'}} initialValues={setInitialValues} onSubmit={onSubmit}
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
                        <TextInput
                            placeholder={''}
                            type={'text'}
                            name={'verificationCode'}
                            errorMessage={errors.verificationCode}
                        />
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