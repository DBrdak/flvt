import {Button, ButtonGroup} from "@mui/material";
import {GridView, Map} from "@mui/icons-material";

interface Props {
    currentView: 'list' | 'map'
    setCurrentView: (currentView: 'list' | 'map') => void
}

export default function ViewSelector({currentView, setCurrentView}: Props) {

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
                color={listStyles.color}
                variant={listStyles.variant}
                onClick={() => setCurrentView('list')}
            >
                <GridView />
            </Button>
        </ButtonGroup>
    )
}