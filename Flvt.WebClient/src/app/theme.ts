import { createTheme } from '@mui/material/styles';

const theme = createTheme({
    palette: {
        mode: 'dark',
        primary: {
            main: '#FF69B4',
            light: '#FFA7C4',
            dark: '#C71585'
        },
        secondary: {
            main: '#00FFFF',
            light: '#B2FFFF',
            dark: '#008B8B'
        },
        error: {
            main: '#ee0000',
            dark: '#8d0000',
            light: '#ff4343'
        },
        info: {
            main: '#ff9800',
        },
        background: {
            default: '#121212',
            paper: '#1E1E1E'
        },
        text: {
            primary: '#ffffff',
            secondary: '#d2d2d2'
        },
        success: {
            main: '#0aa901',
            light: '#83ff00',
            dark: '#006405'
        }
    },
    breakpoints: {
        values: {
            xs: 0,
            sm: 600,
            md: 900,
            lg: 1500,
            xl: 2200,
        }
    },
    components: {
        MuiCssBaseline: {
            styleOverrides: {
                '*::-webkit-scrollbar': {
                    width: '10px',
                },
                '*::-webkit-scrollbar-track': {
                    background: 'transparent',
                },
                '*::-webkit-scrollbar-thumb': {
                    backgroundColor: '#888',
                    borderRadius: '15px',
                    '&:hover': {
                        backgroundColor: '#555',
                    },
                },
            },
        },
        MuiListItemButton:{
            styleOverrides: {
                root:{
                    "&.Mui-disabled": {
                        opacity: 1,
                    }
                }
            }
        },
        MuiButton: {
            styleOverrides: {
                root: {
                    borderRadius: 50
                },
            },
        },
    },
});

export default theme;