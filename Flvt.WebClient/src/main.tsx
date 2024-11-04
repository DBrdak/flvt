import './index.css'
import {CssBaseline, ThemeProvider} from "@mui/material";
import {RouterProvider} from "react-router-dom";
import {router} from "./router/Routes.tsx";
import theme from "./app/theme.ts";
import {createRoot} from "react-dom/client";


const root = createRoot(
    document.getElementById('root') as HTMLElement
)

root.render(
    <ThemeProvider theme={theme}>
        <CssBaseline />
        <RouterProvider router={router} />
    </ThemeProvider>
)
