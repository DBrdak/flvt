import { MapContainer, TileLayer, Marker, useMap } from 'react-leaflet';
import MarkerClusterGroup from 'react-leaflet-cluster';
import { Advertisement } from "../../../models/advertisement";
import { Coordinates } from "../../../models/coordinates";
import AdvertisementPopup from "./AdvertisementPopup.tsx";
import { useEffect, useState } from "react";
import { observer } from "mobx-react-lite";
import { useStore } from "../../../stores/store.ts";
import { Box } from "@mui/material";
import AdvertisementsMiniList from "./AdvertisementsMiniList.tsx";

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

interface Props {
    advertisements: Advertisement[];
}

function AdvertisementsMap({ advertisements }: Props) {
    const [center, setCenter] = useState<Coordinates>(calculateCenter(advertisements));
    const { advertisementStore } = useStore();
    const markers = advertisements.map(ad => ad.geolocation ? ad : { ...ad, geolocation: center });

    useEffect(() => {
        setCenter(calculateCenter(advertisements));
    }, [advertisements]);

    const MapEventHandler = () => {
        const map = useMap();

        useEffect(() => {
            const updateVisibleAdvertisements = () => {
                const bounds = map.getBounds();
                const visibleAds = advertisements.filter(ad => {
                    const { latitude, longitude } = ad.geolocation || {};
                    if (!latitude || !longitude) return false;
                    return bounds.contains([+latitude, +longitude]);
                }).slice(0,50);
                advertisementStore.setVisibleAdvertisements(visibleAds);
            };

            updateVisibleAdvertisements();

            map.on("moveend", updateVisibleAdvertisements);
            map.on("zoomend", updateVisibleAdvertisements);

            return () => {
                map.off("moveend", updateVisibleAdvertisements);
                map.off("zoomend", updateVisibleAdvertisements);
            };
        }, [map, advertisements]);

        return null;
    };

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
                    {markers.map((ad, index) => (
                        <Marker
                            key={index}
                            position={[+ad.geolocation!.latitude, +ad.geolocation!.longitude]}
                            eventHandlers={{
                                mouseover: () => advertisementStore.setPreViewedAdvertisement(ad),
                                mouseout: () => advertisementStore.setPreViewedAdvertisement(null),
                                click: () => advertisementStore.setViewedAdvertisement(ad),
                            }}
                        />
                    ))}
                </MarkerClusterGroup>

                {/* Invoke map bounds tracking within MapContainer context */}
                <MapEventHandler />

            </MapContainer>
            <Box sx={{
                position: 'absolute',
                zIndex: 1001,
                bottom: 0, top: 0, left: 0,
                width: '25vw',
                minWidth: '500px',
                overflowY: 'auto',
                overflowX: 'hidden',
                borderRadius: '20px',
                m: 3
            }}>
                <AdvertisementsMiniList />
            </Box>
        </Box>
    );
}

export default observer(AdvertisementsMap);
