import { MapContainer, TileLayer, Marker } from 'react-leaflet';
import MarkerClusterGroup from 'react-leaflet-cluster';
import { Advertisement } from "../../../models/advertisement";
import {Coordinates} from "../../../models/coordinates.ts";
import AdvertisementPopup from "./AdvertisementPopup.tsx";
import {useEffect, useState} from "react";

function calculateCenter(advertisements: Advertisement[]): Coordinates {
    const markers = advertisements.filter(ad => ad.geolocation);
    if (markers.length === 0) {
        return {latitude: "52.505", longitude: "0"};
    }

    const totalLat = markers
        .map(marker => marker.geolocation!.latitude)
        .reduce((sum, latitude) => sum + +latitude, 0);
    const totalLng = markers
        .map(marker => marker.geolocation!.longitude)
        .reduce((sum, longitude) => sum + +longitude, 0);

    const centerLat = totalLat / markers.length;
    const centerLng = totalLng / markers.length;

    return {latitude: centerLat.toString(), longitude: centerLng.toString()};
}

interface Props {
    advertisements: Advertisement[];
}

function AdvertisementsMap({ advertisements }: Props) {
    const [center, setCenter] = useState<Coordinates>(calculateCenter(advertisements))
    const markers = advertisements.map(ad => ad.geolocation ? ad : {...ad, geolocation: center})

    useEffect(() => {
        setCenter(calculateCenter(advertisements))
    }, [advertisements]);

    return (
        <MapContainer center={[+center.latitude, +center.longitude]} zoom={10} maxZoom={17} minZoom={5} style={{ height: "100vh", width: "100vw" }}>
            <TileLayer
                attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            />
            <MarkerClusterGroup>
                {markers.map((ad, index) => (
                    <Marker
                        key={index}
                        position={[+ad.geolocation!.latitude, +ad.geolocation!.longitude]}
                    >
                        <AdvertisementPopup advertisement={ad} />
                    </Marker>
                ))}
            </MarkerClusterGroup>
        </MapContainer>
    );
}

export default AdvertisementsMap;
