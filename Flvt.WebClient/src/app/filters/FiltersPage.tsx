import {observer} from "mobx-react-lite";
import {useStore} from "../../stores/store.ts";
import useSubscriber from "../../utils/hooks/useSubscriber.tsx";
import {Box, Typography, Skeleton, Button} from "@mui/material";
import {styled} from "@mui/material/styles";
import MuiCard from "@mui/material/Card";
import {Filter} from "../../models/filter.ts";
import FilterCard from "./FilterCard.tsx";
import ConfirmModal from "../sharedComponents/ConfirmModal.tsx";
import {useNavigate} from "react-router-dom";
import NewFilterModal from "./NewBasicFilterModal.tsx";

const Card = styled(MuiCard)(({ theme }) => ({
    alignSelf: 'center',
    width: '100%',
    minHeight: '50vh',
    height: '500px',
    overflowY: 'auto',
    overflowX: 'hidden',
    maxHeight: '90vh',
    padding: theme.spacing(4),
    gap: theme.spacing(1),
    boxShadow:
        'hsla(220, 30%, 5%, 0.05) 0px 5px 15px 0px, hsla(220, 25%, 10%, 0.05) 0px 15px 35px -5px',
    [theme.breakpoints.up('sm')]: {
        width: '450px',
    },
    ...theme.applyStyles('dark', {
        boxShadow:
            'hsla(220, 30%, 5%, 0.5) 0px 5px 15px 0px, hsla(220, 25%, 10%, 0.08) 0px 15px 35px -5px',
    }),
}));

function FiltersPage() {
    const {subscriberStore, modalStore} = useStore()
    const subscriber = useSubscriber()
    const navigate = useNavigate()

    const reachedBasicLimit =
        subscriber?.tier.toLowerCase() === 'basic' &&
        subscriber.filters.length >= 1

    const handleRemove = (filter: Filter) => {
        const removeFilter = async () => {
            const removedFilter = subscriber!.filters.find(f => f.id === filter.id)

            if(!removedFilter){
                return
            }

            subscriber!.filters = subscriber!.filters.filter(f => f.id !== filter.id)
            const isRemoved = await subscriberStore.removeFilterAsync(filter.id)

            if(!isRemoved){
                subscriber!.filters.push(removedFilter)
            }
        }

        modalStore.openModal(<ConfirmModal onConfirm={removeFilter} important reversed text={'Please confirm if you would like to remove the filter'} />)
    }

    return (
        <Box
            component="main"
            sx={[
                (theme) => ({
                    height: '100vh',
                    overflowX: 'hidden',
                    minWidth: '100vw',
                    display: 'flex',
                    flexDirection: 'column',
                    justifyContent: 'center',
                    alignItems: 'center',
                    backgroundImage:
                        'radial-gradient(ellipse at 50% 50%, hsl(210, 100%, 97%), hsl(0, 0%, 100%))',
                    backgroundRepeat: 'no-repeat',
                    ...theme.applyStyles('dark', {
                        backgroundImage:
                            'radial-gradient(at 50% 50%, hsla(210, 100%, 16%, 0.5), hsl(220, 30%, 5%))',
                    })
                })
            ]}>
            <Typography variant="h1" sx={{ p: 3 }}>
                Your filters
            </Typography>
            {
                ['init'].some(action => action === subscriberStore.loading) ?
                    <Card variant="outlined">
                        {[1, 2, 3].map(i => <Skeleton key={i} sx={{width: '100%', minHeight: '200px'}}/>)}
                    </Card>
                    :
                    <Card variant="outlined">
                        {subscriberStore.currentSubscriber?.filters.map(filter => (
                            <FilterCard key={filter.id} filter={filter} onRemove={handleRemove}/>
                        ))}
                        <Button
                            fullWidth
                            variant={'contained'}
                            color={'secondary'}
                            onClick={() => modalStore.openModal(<NewFilterModal />)}
                            disabled={reachedBasicLimit}
                        >
                            Add basic filter
                        </Button>
                        {
                            reachedBasicLimit &&
                                <Typography sx={{width: '100%', textAlign: 'center'}} variant={'body2'} color={'text.secondary'}>
                                    You've reached a limit for your current {subscriber?.tier} tier, if you would like to add more filters, please upgrade your plan
                                </Typography>
                        }
                    </Card>
            }
            <Button sx={{width: '300px', marginY: 2}} variant={'contained'} color={'error'} onClick={() => {
                subscriberStore.logOut()
                navigate('/')
            }}>
                Log out
            </Button>
        </Box>
    )
}

export default observer(FiltersPage);