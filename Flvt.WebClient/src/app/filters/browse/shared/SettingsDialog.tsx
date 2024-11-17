import {observer} from "mobx-react-lite";
import {useStore} from "../../../../stores/store.ts";
import {Button, Checkbox, FormControlLabel, FormGroup, Stack, Typography} from "@mui/material";

function SettingsDialog() {
    const {advertisementStore} = useStore()

    return (
        <Stack direction="column" sx={{display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 3, p: 2}}>
            <Stack direction={'column'} sx={{ flexGrow: 1, textAlign: 'center' }}>
                <Typography variant="h6" component="div">
                    Settings
                </Typography>
                <Typography variant="body2" color="text.secondary">
                    Select which advertisements you would like to browse
                </Typography>
            </Stack>
            <Stack direction="column">
                <FormGroup>
                    <FormControlLabel
                        control={
                            <Checkbox
                                size={'large'}
                                checked={advertisementStore.showNewAds}
                                onClick={() => advertisementStore.setShowNewAds()}
                            />
                        }
                        label="New advertisements"
                        sx={{userSelect: 'none'}}
                    />
                    <FormControlLabel
                        control={
                            <Checkbox
                                checked={advertisementStore.showNotSeenAds}
                                onClick={() => advertisementStore.setShowNotSeenAds()}
                            />
                        }
                        label="Not seen advertisements"
                        sx={{userSelect: 'none'}}
                    />
                    <FormControlLabel
                        control={
                            <Checkbox
                                checked={advertisementStore.showSeenAds}
                                onClick={() => advertisementStore.setShowSeenAds()}
                            />
                        }
                        label="Seen advertisements"
                        sx={{userSelect: 'none'}}
                    />
                    <FormControlLabel
                        control={
                            <Checkbox
                                checked={advertisementStore.showFollowedAds}
                                onClick={() => advertisementStore.setShowFollowedAds()}
                            />
                        }
                        label="Followed advertisements"
                        sx={{userSelect: 'none'}}
                    />
                </FormGroup>
            </Stack>

            <Button
                variant={'outlined'}
                onClick={() => advertisementStore.resetView()}
            >
                Reset
            </Button>
        </Stack>
    )
}

export default observer(SettingsDialog);