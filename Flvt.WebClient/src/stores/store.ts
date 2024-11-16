import { createContext, useContext } from "react";
import ModalStore from "./modalStore.ts";
import AdvertisementStore from "./advertisementStore.ts";
import SubscriberStore from "./subscriberStore.ts";

interface Store {
  modalStore: ModalStore
  advertisementStore: AdvertisementStore
  subscriberStore: SubscriberStore
}

export const store: Store = {
  modalStore: new ModalStore(),
  advertisementStore: new AdvertisementStore(),
  subscriberStore: new SubscriberStore()
}

export const StoreContext = createContext(store);

export function useStore() {
  return useContext(StoreContext)
}