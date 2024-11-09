import { Advertisement } from "../../../models/advertisement";
import { Popup } from "react-leaflet";
import { Box, Typography, Button, CardMedia, Divider } from "@mui/material";

interface Props {
    advertisement: Advertisement;
}

export default function AdvertisementPopup({ advertisement }: Props) {
    const {
        description,
        link,
        address,
        price,
        deposit,
        rooms,
        floor,
        area,
        facilities,
        photos,
        pets,
        availableFrom,
    } = advertisement;

    return (
        <Popup>
            <Box sx={{ width: 300, fontFamily: "Arial, sans-serif", color: "#333" }}>
                {/* Image Section */}
                {photos.length > 0 && (
                    <CardMedia
                        component="img"
                        height="150"
                        image={photos[0]}
                        alt="Property"
                        sx={{ borderRadius: 1, mb: 1 }}
                    />
                )}

                {/* Title and Price */}
                <Typography variant="h6" sx={{ mb: 0.5, color: "primary.main", fontWeight: 600 }}>
                    {description}
                </Typography>
                <Typography variant="body1" sx={{ fontWeight: "bold", color: "text.secondary" }}>
                    Price: {price.amount} {price.currency.code}
                </Typography>
                {deposit && (
                    <Typography variant="body2" color="text.secondary">
                        Deposit: {deposit.amount} {deposit.currency.code}
                    </Typography>
                )}

                {/* Divider */}
                <Divider sx={{ my: 1 }} />

                {/* Property Details */}
                <Typography variant="body2">
                    <strong>Rooms:</strong> {rooms.value}
                </Typography>
                <Typography variant="body2">
                    <strong>Floor:</strong> {floor.specific} {floor.total}
                </Typography>
                <Typography variant="body2">
                    <strong>Area:</strong> {area.amount} {area.unit}
                </Typography>

                {/* Address */}
                <Typography variant="body2" sx={{ fontStyle: "italic", mt: 1 }}>
                    {address.street} {address.houseNumber}, {address.subdistrict} {address.district} {address.city}
                </Typography>

                {/* Facilities */}
                {facilities.length > 0 && (
                    <Box sx={{ mt: 1 }}>
                        <Typography variant="body2" sx={{ fontWeight: "bold" }}>
                            Facilities:
                        </Typography>
                        <Box component="ul" sx={{ pl: 2, my: 0 }}>
                            {facilities.map((facility, index) => (
                                <li key={index}>
                                    <Typography variant="body2" sx={{ fontSize: "0.9em" }}>
                                        {facility}
                                    </Typography>
                                </li>
                            ))}
                        </Box>
                    </Box>
                )}

                {/* Pets and Availability */}
                <Typography variant="body2">
                    <strong>Pets Allowed:</strong> {pets ? "Yes" : "No"}
                </Typography>
                {availableFrom && (
                    <Typography variant="body2">
                        <strong>Available From:</strong> {new Date(availableFrom).toLocaleDateString()}
                    </Typography>
                )}

                {/* View More Button */}
                <Button
                    variant="contained"
                    color="primary"
                    href={link}
                    target="_blank"
                    rel="noopener noreferrer"
                    fullWidth
                    sx={{ mt: 2, textTransform: "none" }}
                >
                    View More
                </Button>
            </Box>
        </Popup>
    );
}
