import {makeAutoObservable} from "mobx";
import agent from "../api/agent.ts";
import AdvertisementsDbContext from "../data/advertisementsDbContext.ts";
import {Advertisement} from "../models/advertisement.ts";

export default class AdvertisementStore {
    loading: boolean = false
    dbContext: AdvertisementsDbContext | null = null

    constructor() {
        makeAutoObservable(this)
    }

    private setLoading(state: boolean) {
        this.loading = state
    }

    private initializeDbContext(filterId: string) {
        if(this.dbContext === null || this.dbContext.filterId !== filterId) {
            this.dbContext = new AdvertisementsDbContext(filterId);
        }
    }

    public async loadAdvertisementsAsync(filterId: string) {
        this.setLoading(true)

        try {
            this.initializeDbContext(filterId)
            const advertisementsFileUrl = await agent.advertisements.getByFilter(filterId)
            const fileGetResponse = await fetch(advertisementsFileUrl)

            if(!fileGetResponse.ok){
                return []
            }

            const advertisements: Advertisement[] = await fileGetResponse.json()

            await this.dbContext!.saveAdvertisementsAsync(advertisements)

            return await this.dbContext!.getAdvertisementsAsync()
        } catch(e) {
            console.log(e)
            return []
        } finally {
            this.setLoading(false)
        }
    }

    public async followAdvertisementAsync(advertisement: Advertisement, filterId: string) {
        this.setLoading(true)

        try {
            advertisement.follow()
            await agent.advertisements.follow(advertisement.link, filterId)
            await this.dbContext!.updateAdvertisementAsync(advertisement)

            return advertisement
        } catch(e) {
            console.log(e)
            return null
        } finally {
            this.setLoading(false)
        }
    }

    public async seeAdvertisementAsync(advertisement: Advertisement, filterId: string) {
        this.setLoading(true)

        try {
            advertisement.see()
            await agent.advertisements.see(advertisement.link, filterId)
            await this.dbContext!.updateAdvertisementAsync(advertisement)

            return advertisement
        } catch(e) {
            console.log(e)
            return null
        } finally {
            this.setLoading(false)
        }
    }

    public async flagAdvertisementAsync(advertisement: Advertisement) {
        this.setLoading(true)

        try {
            advertisement.flag()
            await agent.advertisements.flag(advertisement.link)
            await this.dbContext!.updateAdvertisementAsync(advertisement)

            return advertisement
        } catch(e) {
            console.log(e)
            return null
        } finally {
            this.setLoading(false)
        }
    }
}