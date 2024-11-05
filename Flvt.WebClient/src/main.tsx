import './index.css'
import {CssBaseline, StyledEngineProvider} from "@mui/material";
import {RouterProvider} from "react-router-dom";
import {router} from "./router/Routes.tsx";
import {createRoot} from "react-dom/client";
import AppTheme from "./app/theme/AppTheme.tsx";


const root = createRoot(
    document.getElementById('root') as HTMLElement
)

root.render(
    <AppTheme>
        <CssBaseline />
        <StyledEngineProvider injectFirst>
            <RouterProvider router={router} />
        </StyledEngineProvider>
    </AppTheme>
)
