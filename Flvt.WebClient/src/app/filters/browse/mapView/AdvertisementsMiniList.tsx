import { observer } from "mobx-react-lite"
import { useStore } from "../../../../stores/store.ts"
import AdvertisementTile from "./AdvertisementTile.tsx"
import { useEffect, useState, useRef } from "react"
import { Advertisement } from "../../../../models/advertisement.ts"
import { styled } from "@mui/material/styles"
import MuiCard from "@mui/material/Card"
import {useParams} from "react-router-dom";
import {flow} from "mobx";


const Card = styled(MuiCard)(({ theme }) => ({
    alignSelf: 'center',
    width: '100%',
    minHeight: '50vh',
    height: '500px',
    overflowY: 'auto',
    overflowX: 'hidden',
    maxHeight: '90vh',
    padding: theme.spacing(4),
    gap: theme.spacing(1),
    scrollBehavior: 'smooth',
    boxShadow:
        'hsla(220, 30%, 5%, 0.05) 0px 5px 15px 0px, hsla(220, 25%, 10%, 0.05) 0px 15px 35px -5px',
    [theme.breakpoints.up('sm')]: {
        width: '450px',
    },
    ...theme.applyStyles('dark', {
        boxShadow:
            'hsla(220, 30%, 5%, 0.5) 0px 5px 15px 0px, hsla(220, 25%, 10%, 0.08) 0px 15px 35px -5px',
    }),
}))

function AdvertisementsMiniList() {
    const { advertisementStore } = useStore()
    const [ads, setAds] = useState<Advertisement[]>([])
    const cardRef = useRef<HTMLDivElement>(null)
    const filterId = useParams<{filterId: string}>().filterId

    useEffect(() => {
        setAds(advertisementStore.visibleAdvertisements)
    }, [advertisementStore.visibleAdvertisements])

    useEffect(() => {
        if (cardRef.current) {
            cardRef.current.scrollTop = 0
        }
    }, [ads])

    const isFocused = (ad: Advertisement) =>
        advertisementStore.preViewedAdvertisement?.link === ad.link ||
        advertisementStore.viewedAdvertisement?.link === ad.link

    return (
        <Card ref={cardRef} sx={{ overflowY: 'auto', overflowX: 'hidden', minHeight: '100%' }}>
            {ads.map(ad => (
                <AdvertisementTile
                    key={ad.link}
                    isFocused={isFocused(ad)}
                    ad={ad}
                    flagAdvertisement={ad => advertisementStore.flagAdvertisementAsync(ad, filterId!)}
                    followAdvertisement={ad => advertisementStore.followAdvertisementAsync(ad, filterId!)}
                    seeAdvertisement={ad => advertisementStore.seeAdvertisementAsync(ad, filterId!)}
                />
            ))}
        </Card>
    )
}

export default observer(AdvertisementsMiniList)
