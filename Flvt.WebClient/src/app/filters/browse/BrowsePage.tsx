import {observer} from "mobx-react-lite";
import {useStore} from "../../../stores/store.ts";
import {useEffect, useState} from "react";
import AdvertisementsMap from "./mapView/AdvertisementsMap.tsx";
import LoadingPage from "../../sharedComponents/LoadingPage.tsx";
import {useNavigate, useParams} from "react-router-dom";
import 'leaflet/dist/leaflet.css';
import {toast} from "react-toastify";
import {Box} from "@mui/material";
import ViewSelector from "./shared/ViewSelector.tsx";
import AdvertisementsList from "./listView/AdvertisementsList.tsx";

function BrowsePage() {
    const navigate = useNavigate();
    const filterId = useParams<{filterId: string}>().filterId
    const {advertisementStore} = useStore()
    const [currentView, setCurrentView] = useState<'list' | 'map'>('map')

    useEffect(() => {
        const noAdsFallback = () =>  {
            toast.warning('Sorry, no advertisements found yet, try again later')
            navigate('/filters')
        }

        const loadAds = async () => {
            if(!filterId) {
                noAdsFallback()
                return
            }

            const isLoaded = await advertisementStore.loadAdvertisementsAsync(filterId)

            if(!isLoaded || advertisementStore.advertisements.length === 0) {
                noAdsFallback()
                return
            }
        }

        loadAds()
    }, [filterId]);

    return (
        advertisementStore.loading === 'init' || !filterId || advertisementStore.advertisements.length === 0 ?
            <LoadingPage  variant={'logo'} /> :
            <Box sx={{width: '100vw', height: '100vh', overflowX: 'hidden'}}>
                {
                    currentView == 'map' ?
                        <AdvertisementsMap /> :
                        <AdvertisementsList />
                }
                <Box sx={{
                    position: 'absolute',
                    zIndex: 1000,
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    top: 0, left: 0, right: 0, p: 4
                }}>
                    <ViewSelector currentView={currentView} setCurrentView={setCurrentView} />
                </Box>
            </Box>
    )
}

export default observer(BrowsePage)