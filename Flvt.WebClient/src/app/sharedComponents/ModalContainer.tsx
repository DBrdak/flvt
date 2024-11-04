import {Box, Modal} from "@mui/material";
import {observer} from "mobx-react-lite";
import {useStore} from "../../stores/store.ts";
import theme from "../theme.ts";

function ModalContainer() {
    const { modalStore } = useStore();

    return (
        <Modal
            open={modalStore.modal.open}
            onClose={modalStore.closeModal}
            aria-labelledby="modal-modal-title"
            aria-describedby="modal-modal-description"
        >
            <Box
                sx={{
                    position: 'absolute',
                    top: '50%',
                    left: '50%',
                    transform: 'translate(-50%, -50%)',
                    minWidth: '25%',
                    width: '100vw',
                    maxWidth: '700px',
                    backgroundColor: theme.palette.background.paper,
                    border: '1px solid #000',
                    boxShadow: 24,
                    borderRadius: '20px',
                    p: 4
                }}
            >
                {modalStore.modal.body}
            </Box>
        </Modal>
    );
}

export default observer(ModalContainer);