import React, { useEffect, useState } from 'react'
import { Box, Typography } from '@mui/material'
import { useTheme } from '@mui/material/styles'
import './WelcomePage.css'
import {availableCities} from "../../utils/constants/bussinessRules.ts";
import {useNavigate} from "react-router-dom";
import StartButton from "./StartButton.tsx";
import {Logo} from "../sharedComponents/Logo.tsx";


const WelcomePage: React.FC = () => {
    const theme = useTheme()
    const [currentCityIndex, setCurrentCityIndex] = useState(0)
    const [displayedText, setDisplayedText] = useState('')
    const [isTyping, setIsTyping] = useState(true)
    const navigate = useNavigate()

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
            <Box sx={{margin: '2%'}}>
                <StartButton onClick={() => navigate('/login')} />
            </Box>
        </Box>
    )
}

export default WelcomePage
