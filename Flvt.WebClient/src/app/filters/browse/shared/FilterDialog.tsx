import {observer} from "mobx-react-lite";
import {useStore} from "../../../../stores/store.ts";

function FilterDialog() {
    const {modalStore, advertisementStore} = useStore()

    return <div />
}

export default observer(FilterDialog)