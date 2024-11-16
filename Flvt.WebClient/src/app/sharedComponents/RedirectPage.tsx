import {useEffect, useState} from "react";
import {Box, LinearProgress, Typography} from "@mui/material";
import {useNavigate} from "react-router-dom";

interface NotFoundPageProps {
    text?: string;
    redirectTo?: string
}

const RedirectPage: React.FC<NotFoundPageProps> = ({ text, redirectTo }) => {
    const [progress, setProgress] = useState(0);
    const navigate = useNavigate();

    useEffect(() => {
        const timer = setInterval(() => {
            setProgress((oldProgress) => {
                if (oldProgress === 100) {
                    clearInterval(timer)
                    return 100;
                }
                return Math.min(oldProgress + 1, 100);
            });
        }, 25);

        return () => {
            clearInterval(timer);
        };
    }, []);

    useEffect(() => {
        if(progress === 100) {
            navigate(redirectTo || '/')
        }
    }, [navigate, progress])

    return (
        <Box
            display="flex"
            flexDirection="column"
            alignItems="center"
            justifyContent="center"
            height="100vh"
        >
            <Typography variant="h6" marginBottom={2}>
                {text}
            </Typography>
            <LinearProgress variant='determinate' color='primary' value={progress} style={{ width: '500px' }} />
        </Box>
    );
};

export default RedirectPage;