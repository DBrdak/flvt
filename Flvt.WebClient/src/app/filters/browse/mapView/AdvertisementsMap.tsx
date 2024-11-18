import { MapContainer, TileLayer, Marker, useMap } from 'react-leaflet';
import MarkerClusterGroup from 'react-leaflet-cluster';
import { Advertisement } from "../../../../models/advertisement.ts";
import { Coordinates } from "../../../../models/coordinates.ts";
import { useEffect, useState } from "react";
import { observer } from "mobx-react-lite";
import { useStore } from "../../../../stores/store.ts";
import {Box, useMediaQuery} from "@mui/material";
import {LeafletMouseEvent} from "leaflet";
import {createIcon} from "./AdvertisementMarker.tsx";
import AdvertisementTile from "../shared/AdvertisementTile.tsx";
import {useParams} from "react-router-dom";

function calculateCenter(advertisements: Advertisement[]): Coordinates {
    const markers = advertisements.filter(ad => ad.geolocation);
    if (markers.length === 0) {
        return { latitude: "52.505", longitude: "0" };
    }

    const totalLat = markers.reduce((sum, ad) => sum + parseFloat(ad.geolocation!.latitude), 0);
    const totalLng = markers.reduce((sum, ad) => sum + parseFloat(ad.geolocation!.longitude), 0);

    return {
        latitude: (totalLat / markers.length).toString(),
        longitude: (totalLng / markers.length).toString()
    };
}

function AdvertisementsMap() {
    const { advertisementStore } = useStore();
    const filterId = useParams<{filterId: string}>().filterId
    console.log(filterId)
    const [center, setCenter] = useState<Coordinates>(calculateCenter(advertisementStore.advertisements));
    const markers = advertisementStore.visibleAdvertisements.map(ad => ad.geolocation ? ad : { ...ad, geolocation: center });
    //@ts-ignore
    const isMobile = useMediaQuery(theme => theme.breakpoints.down('sm'))

    useEffect(() => {
        setCenter(calculateCenter(advertisementStore.advertisements));
    }, [advertisementStore.advertisements]);

    const MapEventHandler = () => {
        const map = useMap();

        useEffect(() => {

            const updateViewedAdvertisement = (e: LeafletMouseEvent) => {
                if(e.target.type !== Marker) {
                    advertisementStore.setViewedAdvertisement(null)
                    advertisementStore.setPreViewedAdvertisement(null)
                }
            }

            map.on('click', updateViewedAdvertisement);

        }, [map, advertisementStore.advertisements]);

        return null;
    };
    const isFocused = (ad: Advertisement) =>
        advertisementStore.preViewedAdvertisement?.link === ad.link ||
        advertisementStore.viewedAdvertisement?.link === ad.link

    return (
        <Box sx={{width: '100vw', height: '100vh'}}>
            <MapContainer
                center={[+center.latitude, +center.longitude]}
                zoom={10} maxZoom={18} minZoom={5}
                style={{ height: "100vh", width: "100vw" }}
                zoomControl={false}

            >
                <TileLayer
                    attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                    url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                />
                <MarkerClusterGroup>
                    {markers.map((ad, index) => {
                        const shouldBounce =
                            advertisementStore.preViewedAdvertisement?.link === ad.link ||
                            advertisementStore.viewedAdvertisement?.link === ad.link

                        const color =
                            advertisementStore.preViewedAdvertisement?.link === ad.link ||
                            advertisementStore.viewedAdvertisement?.link === ad.link
                                ? 'green'
                                : ad.isFollowed
                                    ? 'red'
                                    : ad.isNew
                                        ? 'blue'
                                        : ad.wasSeen
                                            ? 'grey'
                                            : 'black'

                        return (
                            <Marker
                                key={index}
                                position={[+ad.geolocation!.latitude, +ad.geolocation!.longitude]}
                                eventHandlers={{
                                    mouseover: () => {
                                        advertisementStore.setPreViewedAdvertisement(ad)
                                    },
                                    click: async () => {
                                        advertisementStore.setViewedAdvertisement(ad)
                                        await advertisementStore.seeAdvertisementAsync(ad, filterId!)
                                    },
                                }}
                                icon={createIcon(color, shouldBounce)}
                            />
                        )
                    })}
                </MarkerClusterGroup>

                <MapEventHandler />

            </MapContainer>
            {
                advertisementStore.viewedAdvertisement && !isMobile &&
                    <Box sx={{
                        position: 'absolute',
                        display: 'flex',
                        justifyContent: 'center',
                        alignItems: 'center',
                        zIndex: 1002,
                        bottom: 0, left: 0, top: 0,
                        width: '25vw',
                        minWidth: '400px',
                        maxWidth: '600px',
                        overflowY: 'auto',
                        overflowX: 'hidden',
                        borderRadius: '20px',
                        p: 1,
                    }}>
                        <AdvertisementTile
                            key={advertisementStore.viewedAdvertisement .link}
                            isFocused={isFocused(advertisementStore.viewedAdvertisement )}
                            ad={advertisementStore.viewedAdvertisement}
                            flagAdvertisement={ad => advertisementStore.flagAdvertisementAsync(ad, filterId!)}
                            followAdvertisement={ad => advertisementStore.followAdvertisementAsync(ad, filterId!)}
                            seeAdvertisement={ad => advertisementStore.seeAdvertisementAsync(ad, filterId!)}
                        />
                    </Box>
            }
            {
                advertisementStore.preViewedAdvertisement && !isMobile &&
                advertisementStore.preViewedAdvertisement.link !== advertisementStore.viewedAdvertisement?.link &&
                    <Box sx={{
                        position: 'absolute',
                        display: 'flex',
                        justifyContent: 'center',
                        alignItems: 'center',
                        zIndex: 1002,
                        bottom: 0, right: 0, top: 0,
                        width: '25vw',
                        minHeight: '800px',
                        minWidth: '400px',
                        maxWidth: '600px',
                        overflowY: 'auto',
                        overflowX: 'hidden',
                        borderRadius: '20px',
                        p: 1,
                    }}>
                            <AdvertisementTile
                                key={advertisementStore.preViewedAdvertisement .link}
                                isFocused={isFocused(advertisementStore.preViewedAdvertisement )}
                                ad={advertisementStore.preViewedAdvertisement}
                                enableHover
                                flagAdvertisement={ad => advertisementStore.flagAdvertisementAsync(ad, filterId!)}
                                followAdvertisement={ad => advertisementStore.followAdvertisementAsync(ad, filterId!)}
                                seeAdvertisement={ad => advertisementStore.seeAdvertisementAsync(ad, filterId!)}
                            />
                    </Box>
            }
            {
                advertisementStore.viewedAdvertisement && isMobile &&
                    <Box sx={{
                        position: 'absolute',
                        top: 0, bottom: 0, left: 0, right: 0,
                        zIndex: 1002,
                        overflowX: 'hidden',
                        borderRadius: '20px',
                        padding: 2
                    }}>
                        <AdvertisementTile
                            key={advertisementStore.viewedAdvertisement .link}
                            isFocused={true}
                            closeButton
                            ad={advertisementStore.viewedAdvertisement}
                            flagAdvertisement={ad => advertisementStore.flagAdvertisementAsync(ad, filterId!)}
                            followAdvertisement={ad => advertisementStore.followAdvertisementAsync(ad, filterId!)}
                            seeAdvertisement={ad => advertisementStore.seeAdvertisementAsync(ad, filterId!)}
                        />
                    </Box>
            }
        </Box>
    );
}

export default observer(AdvertisementsMap);
