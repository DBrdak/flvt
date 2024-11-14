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
import {Business, Favorite, Flag, GridView, Person, Pets} from "@mui/icons-material";
import './MapView.css'
import {colorSchemes} from "../../../theme/themePrimitives.ts";
import {observer} from "mobx-react-lite";
import {useStore} from "../../../../stores/store.ts";

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
    enableHover?: boolean
    flagAdvertisement: (ad: Advertisement) => void
    followAdvertisement: (ad: Advertisement) => void
    seeAdvertisement: (ad: Advertisement) => void
}

function AdvertisementTile({ad, isFocused, enableHover, flagAdvertisement, followAdvertisement, seeAdvertisement}: Props) {
    const {advertisementStore} = useStore()

    const tileStyles = isFocused ? [
            (theme: Theme) => ({
                userSelect: 'none',
                width: '100%',
                padding: 2.5,
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
            boxShadow: 3,
            borderRadius: '10px',
            animation: `${fadeIn} 0.4s ease`,
            '&:hover': {
                backgroundImage:
                    'radial-gradient(at 50% 50%, hsla(210, 100%, 16%, 0.5), hsl(220, 30%, 5%))',
            }
        }
    /* @ts-ignore*/
    return (
        <Card
            variant={'outlined'}
            sx={tileStyles}
            onMouseOver={() => enableHover && advertisementStore.setPreViewedAdvertisement(ad)}
            onMouseLeave={() => enableHover && advertisementStore.setPreViewedAdvertisement(null)}
        >
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
                    <Typography variant="h6" sx={{lineBreak: 'normal', paddingTop: 1, textAlign: 'center'}}>
                        {ad.address.district} {ad.address.street && ad.address.street.length > 0 ? `, ${ad.address.street}` : ''}
                    </Typography>
                    <Typography variant="body2" sx={{textAlign: 'center'}}>
                        {ad.description}
                    </Typography>
                    <Typography variant="subtitle1" color="text.secondary">
                        Price {ad.price.amount} {ad.price.currency.code}
                    </Typography>
                    <Typography variant="subtitle1" color="text.secondary">
                        {ad.deposit ? `Deposit ${ad.deposit?.amount} ${ad.deposit?.currency.code}` : ''}
                    </Typography>
                    <Typography variant="subtitle1" color="text.secondary">
                        {ad.fee ? `Fee ${ad.fee?.amount} ${ad.fee?.currency.code}` : ''}
                    </Typography>
                </Box>

                <Divider sx={{ marginY: 1 }} />

                <Box sx={{
                    display: 'flex',
                    justifyContent: 'space-between',
                    flexDirection: 'row',
                    alignItems: 'center',
                    overflowX: 'hidden',
                    gap: 1,
                    margin: 1
                }}>
                    <Typography variant={'subtitle1'}>
                        {ad.area.amount.toFixed(0)} {ad.area.unit}
                    </Typography>

                    <Tooltip title={`${ad.roomsCount} room${ad.roomsCount && 's'}`}>
                        <Typography variant={'subtitle1'} sx={{display: 'flex', flexDirection: 'row', alignItems: 'center'}}>
                            {ad.roomsCount} <GridView />
                        </Typography>
                    </Tooltip>

                    {
                        ad.isPrivate ?
                            <Tooltip placement={'top'} title={'private offer'}>
                                <Paper variant={'outlined'} className={'ad-attribute'} sx={{background: colorSchemes.dark.palette.success.light}}>
                                    <Person />
                                </Paper>
                            </Tooltip>
                            :
                            <Tooltip placement={'top'} title={'real estate agency offer'}>
                                <Paper variant={'outlined'} className={'ad-attribute'} sx={{background: colorSchemes.dark.palette.error.light}}>
                                    <Business />
                                </Paper>
                            </Tooltip>
                    }
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

            <Divider sx={{ marginY: 1 }} />

            <CardActions sx={{display: 'flex', flexDirection: 'column', gap: 2, marginTop: 1}}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', width: '100%', gap: 2 }}>
                    <Button
                        fullWidth
                        variant={ad.isFollowed ? 'outlined' : "contained"}
                        color={'secondary'}
                        onClick={() => followAdvertisement(ad)}
                    >
                        <Favorite color={ad.isFollowed ? 'error': 'inherit'} />
                    </Button>
                    <Button
                        fullWidth
                        variant={'contained'}
                        color={'warning'}
                        onClick={() => flagAdvertisement(ad)}
                        disabled={ad.isFlagged}
                    >
                        <Flag />
                    </Button>
                </Box>
                <Button
                    fullWidth
                    variant={ad.wasSeen ? 'outlined' : "contained"}
                    href={ad.link}
                    target="_blank"
                    onClick={() => seeAdvertisement(ad)}
                >
                    Check
                </Button>
            </CardActions>
        </Card>
    )
}

export default observer(AdvertisementTile)