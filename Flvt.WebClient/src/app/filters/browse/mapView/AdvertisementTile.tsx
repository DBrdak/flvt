import {Advertisement} from "../../../../models/advertisement.ts";
import {
    Box,
    Button,
    Card,
    CardActions,
    CardContent,
    Divider,
    Paper,
    Tooltip,
    Typography
} from "@mui/material";
import ImageSlider from "../shared/ImageSilder.tsx";
import {keyframes} from "@emotion/react";
import {Theme} from "@mui/material/styles";
import {Pets} from "@mui/icons-material";
import './MapView.css'
import {colorSchemes} from "../../../theme/themePrimitives.ts";

const fadeIn = keyframes`
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
`

interface Props {
    ad: Advertisement
    isFocused: boolean
}

function AdvertisementTile({ad, isFocused}: Props) {

    const tileStyles = isFocused ? [
            (theme: Theme) => ({
                userSelect: 'none',
                width: '100%',
                padding: 2.5,
                marginY: 2,
                boxShadow: 3,
                borderRadius: '10px',
                animation: `${fadeIn} 0.4s ease`,
                backgroundImage:
                    'radial-gradient(ellipse at 100% 100%, hsl(210, 100%, 97%), hsl(0, 0%, 100%))',
                backgroundRepeat: 'no-repeat',
                ...theme.applyStyles('dark', {
                    backgroundImage:
                        'radial-gradient(at 50% 50%, hsla(210, 100%, 16%, 0.5), hsl(220, 30%, 5%))',
                })
            })
        ] :
        {
            userSelect: 'none',
            width: '100%',
            padding: 2.5,
            marginY: 2,
            boxShadow: 3,
            borderRadius: '10px',
            animation: `${fadeIn} 0.4s ease`,
            '&:hover': {
                backgroundImage:
                    'radial-gradient(at 50% 50%, hsla(210, 100%, 16%, 0.5), hsl(220, 30%, 5%))',
            }
        }

    return (
        <Card variant={'outlined'} sx={tileStyles}>
            <CardContent>
                <Box sx={{
                    display: 'flex',
                    justifyContent: 'center',
                    flexDirection: 'column',
                    alignItems: 'center',
                    overflowX: 'hidden',
                    gap: 1
                }}>
                    <ImageSlider photosLinks={ad.photos} />
                    <Typography variant="h6" sx={{lineBreak: 'normal', paddingTop: 1}}>
                        {ad.address.district} {ad.address.street && ad.address.street.length > 0 ? `, ${ad.address.street}` : ''}
                    </Typography>
                    <Typography variant="subtitle1" color="text.secondary">
                        {ad.price.amount} {ad.price.currency.code}
                    </Typography>
                </Box>

                <Divider sx={{ marginY: 1 }} />

                <Box sx={{
                    display: 'flex',
                    justifyContent: 'center',
                    flexDirection: 'row',
                    alignItems: 'center',
                    overflowX: 'hidden',
                    gap: 1,
                    margin: 1
                }}>
                    <Typography variant={'subtitle1'}>
                        {ad.area.amount.toFixed(0)} {ad.area.unit}
                    </Typography>
                    <Typography variant={'subtitle1'}>
                        {ad.roomsCount}
                    </Typography>
                    {
                        ad.pets &&
                        <Tooltip placement={'top'} title={'pets-friendly'}>
                            <Paper variant={'outlined'} className={'ad-attribute'} sx={{background: colorSchemes.dark.palette.success.light}}>
                                <Pets />
                            </Paper>
                        </Tooltip>
                    }
                    {
                        ad.pets === null &&
                        <Tooltip placement={'top'} title={'owner did not pass any information about pets'}>
                            <Paper variant={'outlined'} className={'ad-attribute'} sx={{background: colorSchemes.dark.palette.grey["700"]}}>
                                <Pets />
                            </Paper>
                        </Tooltip>
                    }
                    {
                        ad.pets === false &&
                        <Tooltip placement={'top'} title={'pets not allowed'}>
                            <Paper variant={'outlined'} className={'ad-attribute'} sx={{background: colorSchemes.dark.palette.error.light}}>
                                <Pets />
                            </Paper>
                        </Tooltip>
                    }
                </Box>

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