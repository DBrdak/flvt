import {observer} from "mobx-react-lite";
import {useStore} from "../../stores/store.ts";
import useSubscriber from "../../utils/hooks/useSubscriber.tsx";
import {Box} from "@mui/material";
import {styled} from "@mui/material/styles";
import MuiCard from "@mui/material/Card";
import {Filter} from "../../models/filter.ts";
import {Skeleton} from "@mui/lab";
import FilterCard from "./FilterCard.tsx";
import ConfirmModal from "../sharedComponents/ConfirmModal.tsx";

const Card = styled(MuiCard)(({ theme }) => ({
    flexDirection: 'column',
    alignSelf: 'center',
    width: '100%',
    minHeight: '50vh',
    maxHeight: '90vh',
    padding: theme.spacing(4),
    gap: theme.spacing(1),
    display: 'flex',
    alignItems: 'center',
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
                    minHeight: '100vh',
                    minWidth: '100vw',
                    display: 'flex',
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
            {
                ['init'].some(action => action === subscriberStore.loading) ?
                    <Card variant="outlined">
                        {[1, 2, 3].map(_ => <Skeleton sx={{width: '100%', minHeight: '200px'}}/>)}
                    </Card>
                    :
                    <Card variant="outlined">
                        {subscriber?.filters.map(filter => (
                            <FilterCard filter={filter} onRemove={handleRemove}/>
                        ))}
                    </Card>
            }
        </Box>
    )
}

export default observer(FiltersPage);