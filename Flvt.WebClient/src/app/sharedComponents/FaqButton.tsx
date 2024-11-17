import {useStore} from "../../stores/store.ts";
import {Box} from "@mui/material";
import IconButton from "@mui/material/IconButton";
import Faq from "./Faq.tsx";
import {QuestionMark} from "@mui/icons-material";
import {observer} from "mobx-react-lite";

function FaqButton() {
    const {modalStore} = useStore()

    return (
        <Box sx={{
            padding: 3
        }}>
            <IconButton sx={{
                borderRadius: '50px',
                width: '60px',
                height: '60px'
            }}
                        onClick={() => modalStore.openModal(<Faq />)}
            >
                <QuestionMark fontSize={'large'} />
            </IconButton>
        </Box>
    )
}

export default observer(FaqButton)