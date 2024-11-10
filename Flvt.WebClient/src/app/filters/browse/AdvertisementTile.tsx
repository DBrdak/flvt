import {Advertisement} from "../../../models/advertisement.ts";
import {Box, Button, Card, CardActions, CardContent, Divider, Typography} from "@mui/material";
import {useState} from "react";

interface Props {
    ad: Advertisement
    isFocused: boolean
}

function AdvertisementTile({ad, isFocused}: Props) {
    const [photoIndex, setPhotoIndex] = useState(0)

    return (
        <Card variant={'outlined'} sx={{
            width: '100%',
            padding: 2,
            marginY: 2,
            boxShadow: 3,
            borderRadius: '10px',
            border: isFocused ? '1px solid green' : ''
        }}>
            <CardContent sx={{}}>
                <Box sx={{ display: 'flex', justifyContent: 'center', flexDirection: 'column', alignItems: 'center', overflowX: 'hidden' }}>
                    <img src={ad.photos[photoIndex]} style={{maxHeight: '200px', borderRadius: '10px'}}  alt={ad.link} />
                    <Typography variant="h6" sx={{lineBreak: 'normal', py: 1}}>
                        {ad.description}
                    </Typography>
                    <Typography variant="subtitle1" color="text.secondary">
                        {ad.price.amount} {ad.price.currency.code} {`${isFocused}`}
                    </Typography>
                </Box>

                <Divider sx={{ marginY: 1 }} />
            </CardContent>

            <CardActions sx={{justifyContent: 'space-between'}}>
                <Button fullWidth variant="contained" href={ad.link} target="_blank">
                    Check
                </Button>
            </CardActions>
        </Card>
    )
}

export default AdvertisementTile