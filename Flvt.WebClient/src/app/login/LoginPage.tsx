import React, { useEffect } from 'react'
import {Stack} from '@mui/material'
import './LoginPage.css'
import {useStore} from "../../stores/store.ts";
import Content from "./Content.tsx";
import {observer} from "mobx-react-lite";
import LoginCard from "./LoginCard.tsx";


const LoginPage: React.FC = () => {
    const {subscriberStore} = useStore()

    useEffect(() => {
        const promise = async () => {
            return await subscriberStore.loadCurrentSubscriberAsync()
        }

        promise().then(() => /*subscriberStore.currentSubscriber !== null && navigate('/filters')*/ console.log(subscriberStore.currentSubscriber))
    }, [])

    return (
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
