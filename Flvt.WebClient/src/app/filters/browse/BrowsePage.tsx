import {observer} from "mobx-react-lite";
import {useStore} from "../../../stores/store.ts";
import {useEffect, useState} from "react";
import {Advertisement} from "../../../models/advertisement.ts";
import AdvertisementsMap from "./AdvertisementsMap.tsx";
import LoadingPage from "../../sharedComponents/LoadingPage.tsx";
import {useNavigate, useParams} from "react-router-dom";
import 'leaflet/dist/leaflet.css';
import {toast} from "react-toastify";

function BrowsePage() {
    const navigate = useNavigate();
    const filterId = useParams<{filterId: string}>().filterId
    const {advertisementStore} = useStore()
    const [advertisements, setAdvertisements] = useState<Advertisement[]>([])

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

            const ads = await advertisementStore.loadAdvertisementsAsync(filterId)
            setAdvertisements(ads)

            if(ads.length === 0) {
                noAdsFallback()
                return
            }
        }

        loadAds()
    }, [filterId]);

    return (
        advertisementStore.loading === 'init' || !filterId || advertisements.length === 0 ?
            <LoadingPage  variant={'logo'} /> :
            <AdvertisementsMap advertisements={advertisements} />
    )
}

export default observer(BrowsePage)