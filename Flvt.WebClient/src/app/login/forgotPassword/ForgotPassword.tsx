import {useStore} from "../../../stores/store.ts";
import {observer} from "mobx-react-lite";
import {useState} from "react";
import {SetNewPasswordBody} from "../../../api/requestModels/setNewPassword.ts";
import {useNavigate} from "react-router-dom";
import SetNewPasswordForm from "./SetNewPasswordForm.tsx";
import RequestNewPasswordForm from "./RequestNewPasswordForm.tsx";
import SpinnerLoader from "../../sharedComponents/SpinnerLoader.tsx";

interface ForgotPasswordProps {
}

function ForgotPassword({  }: ForgotPasswordProps) {
    const {modalStore, subscriberStore} = useStore()
    const navigate = useNavigate()
    const [isRequested, setIsRequested] = useState<boolean>(false)

    const requestHandleSubmit = async (values: {email: string}) => {
        const requestResult = await subscriberStore.requestNewPasswordAsync(values.email)

        if(requestResult){
            setIsRequested(true)
        }
    }
    const setHandleSubmit = async (values: SetNewPasswordBody) => {
        const setResult = await subscriberStore.setNewPasswordAsync(values)

        if(setResult){
            modalStore.closeModal()
            navigate('/filters')
        }
    }

    return ['setNewPassword', 'requestNewPassword'].some(action => action === subscriberStore.loading) ?
        <SpinnerLoader size={'s'} /> :
        isRequested ?
            <SetNewPasswordForm onSubmit={setHandleSubmit} /> :
            <RequestNewPasswordForm onSubmit={requestHandleSubmit} />
}

export default observer(ForgotPassword)