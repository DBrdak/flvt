import { Card, CardContent, CardActions, Typography, Button, Box, Divider } from "@mui/material";
import { Filter } from "../../models/filter";
import {format} from 'date-fns'

interface Props {
    filter: Filter;
    onRemove: (filter: Filter) => void;
}

function FilterCard({ filter, onRemove }: Props) {
    return (
        <Card sx={{ width: '100%', display: 'flex', flexDirection: 'column', padding: 2, boxShadow: 3 }}>
            <CardContent>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <Typography variant="h6">{filter.name}</Typography>
                    <Typography variant="body2" color="text.secondary">
                         {filter.tier}
                    </Typography>
                </Box>

                <Divider sx={{ marginY: 1 }} />

                <Typography variant="body2" color="text.secondary">
                    Location: {filter.location}
                </Typography>

                <Typography variant="body2" color="text.secondary">
                    Price Range: {filter.minPrice ?? 'N/A'} - {filter.maxPrice ?? 'N/A'}
                </Typography>

                <Typography variant="body2" color="text.secondary">
                    Rooms: {filter.minRooms ?? 'N/A'} - {filter.maxRooms ?? 'N/A'}
                </Typography>

                <Typography variant="body2" color="text.secondary">
                    Area: {filter.minArea ?? 'N/A'} - {filter.maxArea ?? 'N/A'} sq ft
                </Typography>

                <Typography variant="body2" color="text.secondary">
                    Frequency: {filter.frequency}
                </Typography>

                <Typography variant="body2" color="text.secondary">
                    Last Filtered At: {format(filter.lastUsed, 'Pp')}
                </Typography>

                <Typography variant="body2" color="text.secondary">
                    Next Filter At: {format(filter.nextUse, 'Pp')}
                </Typography>

                <Divider sx={{ marginY: 1 }} />

                <Typography variant="body2" color="text.secondary">
                    New Advertisements: {filter.newAdvertisementsCount}
                </Typography>

                <Typography variant="body2" color="text.secondary">
                    Followed Advertisements: {filter.followedAdvertisementsCount}
                </Typography>

                <Typography variant="body2" color="text.secondary">
                    Total Advertisements: {filter.allAdvertisementsCount}
                </Typography>
            </CardContent>

            <CardActions>
                <Button
                    color="error"
                    onClick={() => onRemove(filter)}
                    size="small"
                    variant="outlined"
                >
                    Remove
                </Button>
            </CardActions>
        </Card>
    );
}

export default FilterCard;
