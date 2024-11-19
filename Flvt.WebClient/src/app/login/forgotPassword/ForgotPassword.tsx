import {useStore} from "../../../stores/store.ts";
import {observer} from "mobx-react-lite";
import {useState} from "react";
import {useNavigate} from "react-router-dom";
import SetNewPasswordForm from "./SetNewPasswordForm.tsx";
import RequestNewPasswordForm from "./RequestNewPasswordForm.tsx";
import SpinnerLoader from "../../sharedComponents/SpinnerLoader.tsx";
import {Box} from "@mui/material";
import {toast} from "react-toastify";

interface ForgotPasswordProps {
}

function ForgotPassword({  }: ForgotPasswordProps) {
    const {modalStore, subscriberStore} = useStore()
    const navigate = useNavigate()
    const [isRequested, setIsRequested] = useState<boolean>(false)
    const [email, setEmail] = useState('')

    const requestHandleSubmit = async (values: {email: string}) => {
        const requestResult = await subscriberStore.requestNewPasswordAsync(values.email)

        if(requestResult){
            setIsRequested(true)
            setEmail(values.email)
            return
        }

        toast.dismiss()
        toast.error('Sorry we are not able to send you an email now. Please try again within next 5 minutes.')
    }
    const setHandleSubmit = async (values: {verificationCode: string, password: string}) => {
        const setResult = await subscriberStore.setNewPasswordAsync(values.password, email, values.verificationCode)

        if(!setResult){
            setIsRequested(false)
            return
        }

        modalStore.closeModal()
        navigate('/filters')
    }

    return ['setNewPassword', 'requestNewPassword'].some(action => action === subscriberStore.loading) ?
        <Box sx={{
            width: '100%',
            height: '100%',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center'
        }}>
            <SpinnerLoader size={'s'} />
        </Box> :
        isRequested ?
            <SetNewPasswordForm onSubmit={setHandleSubmit} /> :
            <RequestNewPasswordForm onSubmit={requestHandleSubmit} />
}

export default observer(ForgotPassword)