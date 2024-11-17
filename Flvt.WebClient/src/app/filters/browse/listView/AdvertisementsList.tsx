import { useState, useEffect, useRef } from 'react'
import { Grid, Container, Box, CircularProgress } from '@mui/material'
import { observer } from 'mobx-react-lite'
import { useStore } from '../../../../stores/store.ts'
import AdvertisementTile from '../mapView/AdvertisementTile.tsx'
import { useParams } from 'react-router-dom'

function AdvertisementsList() {
    const { advertisementStore } = useStore()
    const filterId = useParams<{ filterId: string }>().filterId
    const [page, setPage] = useState(1)
    const loaderRef = useRef<HTMLDivElement | null>(null)

    const rowSize = (() => {
        if (window.innerWidth < 600) return 1 // xs screens
        if (window.innerWidth < 960) return 3 // sm screens
        return 4 // md and above
    })()

    const pageSize = rowSize * 8
    const slicedAdvertisements = advertisementStore.visibleAdvertisements.slice(0, page * pageSize)

    useEffect(() => {
        if (!loaderRef.current) return

        const observer = new IntersectionObserver(
            (entries) => {
                if (entries[0].isIntersecting && page * pageSize < advertisementStore.visibleAdvertisements.length) {
                    setPage((prevPage) => prevPage + 1)
                }
            },
            { threshold: 1.0 }
        )

        observer.observe(loaderRef.current)

        return () => {
            if (loaderRef.current) observer.unobserve(loaderRef.current)
        }
    }, [page, pageSize, advertisementStore.visibleAdvertisements.length])

    return (
        <Container sx={{ paddingY: 10, overflowX: 'hidden'}}>
            <Grid container spacing={3}>
                {slicedAdvertisements.map((ad) => (
                    <Grid
                        item
                        key={ad.link}
                        xs={12}
                        sm={6}
                        lg={4}
                    >
                        <AdvertisementTile
                            ad={ad}
                            isFocused={false}
                            enableHover
                            flagAdvertisement={(ad) => advertisementStore.flagAdvertisementAsync(ad, filterId!)}
                            followAdvertisement={(ad) => advertisementStore.followAdvertisementAsync(ad, filterId!)}
                            seeAdvertisement={(ad) => advertisementStore.seeAdvertisementAsync(ad, filterId!)}
                        />
                    </Grid>
                ))}
            </Grid>

            {/* Loader */}
            {page * pageSize < advertisementStore.visibleAdvertisements.length && (
                <Box
                    ref={loaderRef}
                    sx={{
                        display: 'flex',
                        justifyContent: 'center',
                        alignItems: 'center',
                        paddingY: 2
                    }}
                >
                    <CircularProgress />
                </Box>
            )}
        </Container>
    )
}

export default observer(AdvertisementsList)
