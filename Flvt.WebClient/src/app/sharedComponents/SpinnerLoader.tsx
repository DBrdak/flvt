import { keyframes } from '@emotion/react'
import { Box } from '@mui/material'

interface Props {
    size: 'xs' | 's' | 'm' | 'l' | 'xl'
}

// Define the rotation keyframes
const rotation = keyframes`
    0% {
        transform: rotate(0deg);
    }
    100% {
        transform: rotate(360deg);
    }
`

export default function SpinnerLoader({ size }: Props) {
    const widthMap: { [key in Props['size']]: string } = {
        xs: '30px',
        s: '70px',
        m: '100px',
        l: '150px',
        xl: '250px',
    }

    const width = widthMap[size]

    return (
        <Box
            sx={{
                width: width,
                height: width,
                aspectRatio: '1',
                borderRadius: '50%',
                display: 'inline-block',
                borderTop: '3px solid hsl(210, 98%, 55%)',
                borderRight: '3px solid transparent',
                boxSizing: 'border-box',
                animation: `${rotation} 1s linear infinite`,
            }}
        />
    )
}
