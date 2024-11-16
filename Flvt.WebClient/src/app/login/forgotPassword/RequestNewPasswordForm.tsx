import Button from '@mui/material/Button';
import {Box, Typography} from "@mui/material";
import {useStore} from "../../../stores/store.ts";
import {observer} from "mobx-react-lite";
import {Form, Formik} from "formik";
import FormLabel from "@mui/material/FormLabel";
import TextInput from "../../sharedComponents/TextInput.tsx";
import * as Yup from "yup";
import {emailAddressPattern} from "../../../utils/constants/bussinessRules.ts";

interface Props {
    onSubmit: (values: {email: string}) => void
}

function RequestNewPasswordForm({ onSubmit }: Props) {
    const {modalStore} = useStore()

    const requestInitialValues = {email: ''}
    const requestValidationSchema = Yup.object({
        email: Yup.string().matches(emailAddressPattern, 'Invalid email address').required('Email is required')
    })

    return (
        <Formik autocomplete={'off'} sx={{width: '100%'}} initialValues={requestInitialValues} onSubmit={onSubmit} validationSchema={requestValidationSchema}>
            {({errors}) => (
                <Form style={{
                    width: '100%',
                    display: 'flex',
                    justifyContent: 'center',
                    gap: '2em',
                    alignItems: 'center',
                    flexDirection: 'column',
                    position: 'relative',
                }}>
                    <Typography variant={'subtitle1'} sx={{padding: '1rem'}}>
                        Enter your account&apos;s email address, and we&apos;ll send you a link to
                        reset your password.
                    </Typography>
                    <Box sx={{width: '100%'}}>
                        <FormLabel htmlFor="email">Email</FormLabel>
                        <TextInput
                            placeholder={''}
                            type={'email'}
                            name={'email'}
                            errorMessage={errors.email}
                        />
                    </Box>
                    <Box sx={{ padding: '1em', display: 'flex', justifyContent: 'space-around' }}>
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

export default observer(RequestNewPasswordForm)