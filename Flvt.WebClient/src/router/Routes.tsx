import { RouteObject } from "react-router"
import {createBrowserRouter} from 'react-router-dom'
import App from "../app/App";
import RedirectPage from "../app/sharedComponents/RedirectPage.tsx";
import LoginPage from "../app/login/LoginPage.tsx";
import WelcomePage from "../app/welcome/WelcomePage.tsx";

export const routes: RouteObject[] = [
    {
        path: '/',
        element: <App />,
        children: [
            {path: '/', element: <WelcomePage />},
            {path: '/login', element: <LoginPage />},
            {path: '*', element: <RedirectPage text={'We are sorry, the content you are looking for does not exist 😔'} />},
        ]
    }
]

export const router = createBrowserRouter(routes);