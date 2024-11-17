import { Card, CardContent, CardActions, Typography, Button, Box, Divider } from "@mui/material";
import { Filter } from "../../models/filter";
import {format} from 'date-fns'
import {Delete} from "@mui/icons-material";
import {useNavigate} from "react-router-dom";

interface Props {
    filter: Filter;
    onRemove: (filter: Filter) => void;
}

function FilterCard({ filter, onRemove }: Props) {
    const navigate = useNavigate()

    return (
        <Card sx={{
            width: '100%',
            padding: 2,
            marginY: 2,
            boxShadow: 3,
            display: 'flex',
            flexDirection: 'column'
        }}>
            <CardContent sx={{}}>
                <Box sx={{ display: 'flex', justifyContent: 'center', flexDirection: 'column', alignItems: 'center', overflowX: 'hidden' }}>
                    <Typography variant="h6" sx={{lineBreak: 'anywhere'}}>
                        {filter.name}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                         {filter.tier}
                    </Typography>
                </Box>

                <Divider sx={{ marginY: 1 }} />

                <Typography variant="body2" color="text.secondary">
                    Location: {filter.location}
                </Typography>

                <Typography variant="body2" color="text.secondary">
                    Price Range: {filter.minPrice ?? 0} - {filter.maxPrice ?? '∞'}
                </Typography>

                <Typography variant="body2" color="text.secondary">
                    Rooms: {filter.minRooms ?? 1} - {filter.maxRooms ?? '∞'}
                </Typography>

                <Typography variant="body2" color="text.secondary">
                    Area: {filter.minArea ?? 0} - {filter.maxArea ?? '∞'}
                </Typography>

                <Typography variant="body2" color="text.secondary">
                    Frequency: {filter.frequency}
                </Typography>

                <Typography variant="body2" color="text.secondary">
                    Last Filtered At: {format(filter.lastUsed, 'Pp')}
                </Typography>

                <Typography variant="body2" color="text.secondary">
                    Next Filtering At: {format(filter.nextUse, 'Pp')}
                </Typography>

                <Divider sx={{ marginY: 1 }} />

                <Typography variant="body2" color="text.secondary">
                    New Advertisements: {filter.newAdvertisementsCount}
                </Typography>

                <Typography variant="body2" color="text.secondary">
                    Seen Advertisements: {filter.seenAdvertisementsCount}
                </Typography>

                <Typography variant="body2" color="text.secondary">
                    Followed Advertisements: {filter.followedAdvertisementsCount}
                </Typography>

                <Typography variant="body2" color="text.secondary">
                    Total Advertisements: {filter.allAdvertisementsCount}
                </Typography>
            </CardContent>

            <CardActions sx={{justifyContent: 'space-between'}}>
                <Button

                    onClick={() => onRemove(filter)}
                    size="small"
                    variant="outlined"
                >
                    <Delete color={'error'} />
                </Button>
                <Button
                    onClick={() => navigate(`${filter.id}/browse`)}
                    size="small"
                    variant="contained"
                >
                    Browse
                </Button>
            </CardActions>
        </Card>
    );
}

export default FilterCard;
