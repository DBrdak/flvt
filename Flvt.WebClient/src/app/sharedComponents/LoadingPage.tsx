import LogoLoader from "./LogoLoader.tsx";
import {ReactElement} from "react";
import {Box} from "@mui/material";
import SpinnerLoader from "./SpinnerLoader.tsx";

interface Props {
    variant: 'logo' | 'spinner'
}

function LoadingPage({variant}: Props) {
    const variantMap: { [key in Props['variant']]: ReactElement } = {
        logo: <LogoLoader size={'m'} />,
        spinner: <SpinnerLoader size={'xl'} />
    }

    return (
        <Box sx={{display: 'flex', alignItems: 'center', justifyContent: 'center', width: '100vw', height: '100vh'}}>
            {variantMap[variant]}
        </Box>
    )
}

export default LoadingPage