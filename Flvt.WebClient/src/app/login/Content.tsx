import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import AutoFixHighRoundedIcon from '@mui/icons-material/AutoFixHighRounded';
import ConstructionRoundedIcon from '@mui/icons-material/ConstructionRounded';
import SettingsSuggestRoundedIcon from '@mui/icons-material/SettingsSuggestRounded';
import ThumbUpAltRoundedIcon from '@mui/icons-material/ThumbUpAltRounded';
import {Logo} from "../sharedComponents/Logo.tsx";

const items = [
    {
        icon: <SettingsSuggestRoundedIcon sx={{ color: 'text.secondary' }} />,
        title: 'Optimized Search Performance',
        description:
            'Find properties effortlessly with Flvt’s lightning-fast search, delivering all relevant listings in seconds.',
    },
    {
        icon: <ConstructionRoundedIcon sx={{ color: 'text.secondary' }} />,
        title: 'Reliable and Robust',
        description:
            'Built with cutting-edge technology, Flvt offers a reliable and smooth experience, designed to keep up with the demands of renters.',
    },
    {
        icon: <ThumbUpAltRoundedIcon sx={{ color: 'text.secondary' }} />,
        title: 'User-Friendly Interface',
        description:
            'Enjoy a seamless experience with Flvt’s intuitive design, making property search easier and more efficient than ever.',
    },
    {
        icon: <AutoFixHighRoundedIcon sx={{ color: 'text.secondary' }} />,
        title: 'AI-Powered Insights',
        description:
            'Get personalized recommendations with Flvt’s AI-driven technology, tailored to match your unique rental needs.',
    },
];

export default function Content() {
    return (
        <Stack
            sx={{ flexDirection: 'column', alignSelf: 'center', gap: 4, maxWidth: 450 }}
        >
            <Box sx={{display: {xs: 'none', md: 'flex'}}}>
                <Logo size={'s'} />
            </Box>
            {items.map((item, index) => (
                <Stack key={index} direction="row" sx={{gap: 2}}>
                    {item.icon}
                    <div>
                        <Typography gutterBottom sx={{ fontWeight: 'medium' }}>
                            {item.title}
                        </Typography>
                        <Typography variant="body2" sx={{ color: 'text.secondary' }}>
                            {item.description}
                        </Typography>
                    </div>
                </Stack>
            ))}
        </Stack>
    );
}
