import React, { useState, useRef } from 'react'
import { Box, IconButton, useMediaQuery } from '@mui/material'
import { ArrowBackIos, ArrowForwardIos } from '@mui/icons-material'
import { useTheme } from '@mui/material/styles'

interface ImageSliderProps {
    photosLinks: string[]
}

const ImageSlider: React.FC<ImageSliderProps> = ({ photosLinks }) => {
    const [currentIndex, setCurrentIndex] = useState(0)
    const theme = useTheme()
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'))

    const touchStartX = useRef<number | null>(null)
    const touchEndX = useRef<number | null>(null)
    const slideWidth = 100

    const handleNext = () => {
        setCurrentIndex((prevIndex) => (prevIndex + 1) % photosLinks.length)
    }

    const handleBack = () => {
        setCurrentIndex((prevIndex) =>
            prevIndex === 0 ? photosLinks.length - 1 : prevIndex - 1
        )
    }

    const handleTouchStart = (e: React.TouchEvent) => {
        touchStartX.current = e.touches[0].clientX
    }

    const handleTouchMove = (e: React.TouchEvent) => {
        touchEndX.current = e.touches[0].clientX
    }

    const handleTouchEnd = () => {
        if (!touchStartX.current || !touchEndX.current) return

        const distance = touchStartX.current - touchEndX.current
        const minSwipeDistance = 50

        if (distance > minSwipeDistance) handleNext()
        else if (distance < -minSwipeDistance) handleBack()

        touchStartX.current = null
        touchEndX.current = null
    }

    return (
        <Box
            sx={{
                position: 'relative',
                width: '100%',
                overflow: 'hidden',
                borderRadius: 2,
                boxShadow: 3,
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
            }}
            onTouchStart={handleTouchStart}
            onTouchMove={handleTouchMove}
            onTouchEnd={handleTouchEnd}
        >
            <Box
                sx={{
                    display: 'flex',
                    transform: `translateX(-${currentIndex * slideWidth}%)`,
                    transition: 'transform 0.5s ease',
                    width: `${photosLinks.length * slideWidth}%`,
                    borderRadius: 2
                }}
            >
                {photosLinks.map((link, index) => (
                    <img
                        key={index}
                        alt={`Slide ${index + 1}`}
                        src={link}
                        style={{
                            width: `${slideWidth}%`,
                            aspectRatio: 1,
                            height: '100%',
                            objectFit: 'cover',
                            borderRadius: 2
                        }}
                    />
                ))}
            </Box>

            {!isMobile && (
                <>
                    <IconButton
                        onClick={handleBack}
                        sx={{
                            position: 'absolute',
                            top: '50%',
                            left: '10px',
                            transform: 'translateY(-50%)',
                        }}
                    >
                        <ArrowBackIos />
                    </IconButton>
                    <IconButton
                        onClick={handleNext}
                        sx={{
                            position: 'absolute',
                            top: '50%',
                            right: '10px',
                            transform: 'translateY(-50%)',
                        }}
                    >
                        <ArrowForwardIos />
                    </IconButton>
                </>
            )}
        </Box>
    )
}

export default ImageSlider
