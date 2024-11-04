import {keyframes, styled} from "@mui/material";
import MuiBox from "@mui/material/Box";

const fadeIn = keyframes`
            0% { opacity: 0; }
            100% { opacity: 1; }
        `

export const FadeContainer = styled(MuiBox)(
    () => ({
        animation: `${fadeIn} 2s`,
        minWidth: '99svw',
        minHeight: '100svh',
        overflowX: 'hidden',
    })
)