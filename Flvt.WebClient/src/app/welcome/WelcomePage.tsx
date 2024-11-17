import React, { useEffect, useState } from 'react'
import { Box, Typography } from '@mui/material'
import { useTheme } from '@mui/material/styles'
import './WelcomePage.css'
import {availableCities} from "../../utils/constants/bussinessRules.ts";
import {useNavigate} from "react-router-dom";
import StartButton from "./StartButton.tsx";
import {Logo} from "../sharedComponents/Logo.tsx";
import IconButton from "@mui/material/IconButton";
import {QuestionMark} from "@mui/icons-material";
import {useStore} from "../../stores/store.ts";
import {observer} from "mobx-react-lite";
import Faq from "./Faq.tsx";


const WelcomePage: React.FC = () => {
    const theme = useTheme()
    const [currentCityIndex, setCurrentCityIndex] = useState(0)
    const [displayedText, setDisplayedText] = useState('')
    const [isTyping, setIsTyping] = useState(true)
    const navigate = useNavigate()
    const {modalStore} = useStore()

    useEffect(() => {
        if (isTyping) {
            if (displayedText.length < availableCities[currentCityIndex].length) {
                setTimeout(() => {
                    setDisplayedText(availableCities[currentCityIndex].slice(0, displayedText.length + 1))
                }, 100)
            } else {
                setIsTyping(false)
                setTimeout(() => setIsTyping(true), 1000)
            }
        } else {
            setTimeout(() => {
                setDisplayedText('')
                setCurrentCityIndex((prevIndex) => (prevIndex + 1) % availableCities.length)
            }, 1000)
        }

    }, [displayedText, isTyping, currentCityIndex])

    return (
        <Box className="container"
             sx={{background: `radial-gradient(${theme.palette.background.paper}, ${theme.palette.background.default})`}}>
            <Box sx={{mb: 4}}>
                <Logo size={'xl'} />
            </Box>
            <Typography
                sx={{fontSize: 'min(4em, 40vw)', letterSpacing: '0.5rem'}}
                className="typewriter"
            >
                {displayedText}
            </Typography>
            <Box className={'start-btn-container'}>
                <StartButton onClick={() => navigate('/login')} />
            </Box>
            <Box sx={{
                position: 'absolute',
                bottom: 0, right: 0,
                padding: 3
            }}>
                <IconButton sx={{
                        borderRadius: '50px',
                        width: '60px',
                        height: '60px'
                    }}
                    onClick={() => modalStore.openModal(<Faq />)}
                >
                    <QuestionMark fontSize={'large'} />
                </IconButton>
            </Box>
        </Box>
    )
}

export default observer(WelcomePage)
