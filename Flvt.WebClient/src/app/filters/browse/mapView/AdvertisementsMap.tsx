import { MapContainer, TileLayer, Marker, useMap } from 'react-leaflet';
import MarkerClusterGroup from 'react-leaflet-cluster';
import { Advertisement } from "../../../../models/advertisement.ts";
import { Coordinates } from "../../../../models/coordinates.ts";
import { useEffect, useState } from "react";
import { observer } from "mobx-react-lite";
import { useStore } from "../../../../stores/store.ts";
import { Box } from "@mui/material";
import AdvertisementsMiniList from "./AdvertisementsMiniList.tsx";
import {LeafletMouseEvent} from "leaflet";
import {createIcon} from "./AdvertisementMarker.tsx";
import advertisementsList from "../listView/AdvertisementsList.tsx";
import AdvertisementPreview from "./AdvertisementPreview.tsx";
import AdvertisementTile from "./AdvertisementTile.tsx";
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
    const [center, setCenter] = useState<Coordinates>(calculateCenter(advertisementStore.advertisements));
    const markers = advertisementStore.advertisements.map(ad => ad.geolocation ? ad : { ...ad, geolocation: center });

    useEffect(() => {
        setCenter(calculateCenter(advertisementStore.advertisements));
    }, [advertisementStore.advertisements]);

    const MapEventHandler = () => {
        const map = useMap();

        useEffect(() => {
            const updateVisibleAdvertisements = () => {
                const bounds = map.getBounds();
                const visibleAds = advertisementStore.advertisements.filter(ad => {
                    const { latitude, longitude } = ad.geolocation || {};
                    if (!latitude || !longitude) return false;
                    return bounds.contains([+latitude, +longitude]);
                }).slice(0,15);
                advertisementStore.setVisibleAdvertisements(visibleAds);
            };

            const updateViewedAdvertisement = (e: LeafletMouseEvent) => {
                if(e.target.type !== Marker) {
                    advertisementStore.setViewedAdvertisement(null)
                    advertisementStore.setPreViewedAdvertisement(null)
                }
            }

            updateVisibleAdvertisements();

            map.on('click', updateViewedAdvertisement);
            map.on("moveend", updateVisibleAdvertisements);
            map.on("zoomend", updateVisibleAdvertisements);

            return () => {
                map.off("moveend", updateVisibleAdvertisements);
                map.off("zoomend", updateVisibleAdvertisements);
            };
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
                zoom={10} maxZoom={17} minZoom={5}
                style={{ height: "100vh", width: "100vw" }}
                zoomControl={false}

            >
                <TileLayer
                    attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                    url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                />
                <MarkerClusterGroup>
                    {markers.map((ad, index) => {
                        // Determine if this marker should bounce
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
                                    mouseout: () => {
                                        advertisementStore.setPreViewedAdvertisement(null)
                                    },
                                    click: () => {
                                        advertisementStore.setViewedAdvertisement(ad)
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
                advertisementStore.viewedAdvertisement &&
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
                advertisementStore.preViewedAdvertisement &&
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
        </Box>
    );
}

export default observer(AdvertisementsMap);
