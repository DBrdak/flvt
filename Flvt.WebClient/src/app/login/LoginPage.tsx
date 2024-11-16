import React, { useEffect } from 'react'
import {Stack} from '@mui/material'
import './LoginPage.css'
import {useStore} from "../../stores/store.ts";
import Content from "./shared/Content.tsx";
import {observer} from "mobx-react-lite";
import LoginCard from "./login/LoginCard.tsx";
import {useNavigate} from "react-router-dom";
import LoadingPage from "../sharedComponents/LoadingPage.tsx";


const LoginPage: React.FC = () => {
    const {subscriberStore} = useStore()
    const navigate = useNavigate()

    useEffect(() => {
        const loadSubscriber = async () => {
            await subscriberStore.loadCurrentSubscriberAsync()

            if (subscriberStore.currentSubscriber !== null) {
                navigate('/filters')
            }
        }

        loadSubscriber()
    }, [])

    return (
        ['init', 'login'].some(action => action == subscriberStore.loading) ?
            <LoadingPage variant={'spinner'} />
            :
            <Stack
                direction="column"
                component="main"
                sx={[
                    (theme) => ({
                        minHeight: '100vh',
                        minWidth: '100vw',
                        justifyContent: 'center',
                        alignItems: 'center',
                        backgroundImage:
                            'radial-gradient(ellipse at 50% 50%, hsl(210, 100%, 97%), hsl(0, 0%, 100%))',
                        backgroundRepeat: 'no-repeat',
                        ...theme.applyStyles('dark', {
                            backgroundImage:
                                'radial-gradient(at 50% 50%, hsla(210, 100%, 16%, 0.5), hsl(220, 30%, 5%))',
                        })
                    })
                ]}
            >
                <Stack
                    direction={{ xs: 'column-reverse', md: 'row' }}
                    sx={{
                        justifyContent: 'center',
                        gap: { xs: 6, sm: 12 },
                        p: 2,
                        mx: 'auto',
                    }}
                >
                    <Stack
                        direction={{ xs: 'column-reverse', md: 'row' }}
                        sx={{
                            justifyContent: 'center',
                            gap: { xs: 6, sm: 12 },
                            p: { xs: 2, sm: 4 },
                            m: 'auto',
                        }}
                    >
                        <Content />
                        <LoginCard />
                    </Stack>
                </Stack>
            </Stack>
    )
}

export default observer(LoginPage)
