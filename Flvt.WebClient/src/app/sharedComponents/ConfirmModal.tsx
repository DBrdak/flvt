import { Button, Stack, Typography } from '@mui/material'
import {observer} from "mobx-react-lite";
import {useStore} from "../../stores/store.ts";

interface Props {
    text: string
    onConfirm: () => void
    reversed?: boolean
    important?: boolean
}

function ConfirmModal({text, important, reversed, onConfirm}: Props) {
    const {modalStore} = useStore()

    const handleConfirm = () => {
        onConfirm()
        modalStore.closeModal()
    }

    return (
        <Stack textAlign="center" width={'100%'} direction={'column'} spacing={4}>
            <Typography variant="h5" gutterBottom>
                {text}
            </Typography>
            {important && <Typography variant='body2'>This action cannot be undone</Typography>}
            <Stack direction={'column'} spacing={2} justifyContent={'center'} width={'100%'}>
                <Button variant={reversed ? "outlined" : 'contained'} color="primary" onClick={handleConfirm} >
                    Yes
                </Button>
                <Button variant={reversed ? "contained" : 'outlined'} color="primary" onClick={() => modalStore.closeModal()} >
                    No
                </Button>
            </Stack>
        </Stack>
    )
}

export default observer(ConfirmModal)