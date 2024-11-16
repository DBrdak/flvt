import {Button, ButtonGroup} from "@mui/material";
import {GridView, Map, Settings} from "@mui/icons-material";
import {useStore} from "../../../../stores/store.ts";
import {observer} from "mobx-react-lite";
import FilterDialog from "./SettingsDialog.tsx";

interface Props {
    currentView: 'list' | 'map'
    setCurrentView: (currentView: 'list' | 'map') => void
}

function ViewSelector({currentView, setCurrentView}: Props) {
    const {modalStore} = useStore()

    const styles = (viewName: string): {variant: 'outlined' | 'contained', color: 'secondary' | 'primary'} => (
        {
            variant: viewName === currentView ? 'contained' : 'outlined',
            color: viewName === currentView ? 'secondary' : 'primary',
        }
    );

    const mapStyles = styles('map');
    const listStyles = styles('list');

    return (
        <ButtonGroup sx={{zIndex: 2000}}>
            <Button
                color={mapStyles.color}
                variant={mapStyles.variant}
                onClick={() => setCurrentView('map')}
            >
                <Map />
            </Button>
            <Button
                variant={'contained'}
                onClick={() => modalStore.openModal(<FilterDialog />)}
            >
                <Settings />
            </Button>
            <Button
                color={listStyles.color}
                variant={listStyles.variant}
                onClick={() => setCurrentView('list')}
            >
                <GridView />
            </Button>
        </ButtonGroup>
    )
}

export default observer(ViewSelector)