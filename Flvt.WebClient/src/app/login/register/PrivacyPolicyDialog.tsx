import {Box, Button, Paper} from "@mui/material";
import { RegisterBody } from "../../../api/requestModels/register";
import PrivacyPolicy from "../../../utils/constants/privacyPolicy.tsx";
import SpinnerLoader from "../../sharedComponents/SpinnerLoader.tsx";
import {useState} from "react";

interface Props {
    registerBody: RegisterBody
    onSubmit: (values: RegisterBody) => Promise<void>
    onReject: () => void
}

function PrivacyPolicyDialog({ registerBody, onSubmit, onReject }: Props) {
    const [registerLoading, setRegisterLoading] = useState(false)

    function handleSubmit() {
        setRegisterLoading(true)
        onSubmit(registerBody).then(() => setRegisterLoading(false))
    }

    return (
        <Box sx={{overflow: 'hidden', maxHeight: '90vh', maxWidth: '80vw', padding: 2}}>
            <Paper variant={'outlined'} sx={{overflow: 'auto', height: '70%', maxHeight: '50vh', margin: 5, boxShadow: 'hsl(220, 30%, 6%) 0 0 50px 20px inset'}}>
                <PrivacyPolicy />
            </Paper>
            <Box sx={{display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 3}}>
                <Button
                    onClick={handleSubmit}
                    variant="contained"
                    color="primary"
                >
                    {registerLoading ? <SpinnerLoader size={'xs'} /> : 'Accept'}
                </Button>
                <Button
                    onClick={onReject}
                    variant="outlined"
                    color="secondary"
                >
                    Reject
                </Button>
            </Box>
        </Box>
    )
}

export default PrivacyPolicyDialog;
