import {makeAutoObservable} from "mobx";
import agent from "../api/agent.ts";
import AdvertisementsDbContext from "../data/advertisementsDbContext.ts";
import {Advertisement, AdvertisementFunctions} from "../models/advertisement.ts";

export default class AdvertisementStore {
    loading: string | null = null
    dbContext: AdvertisementsDbContext | null = null

    constructor() {
        this.dbContext = new AdvertisementsDbContext()
        makeAutoObservable(this)
    }

    private setLoading(state: string | null) {
        this.loading = state
    }

    public async loadAdvertisementsAsync(filterId: string) {
        this.setLoading('init')

        try {
            const advertisementsFileUrl = await agent.advertisements.getByFilter(filterId)
            const fileGetResponse = await fetch(advertisementsFileUrl)

            if(fileGetResponse.status !== 200){
                 return []
            }

            const advertisements: Advertisement[] = await fileGetResponse.json()

            await this.dbContext!.saveAdvertisementsAsync(filterId, advertisements)

            return await this.dbContext!.getAdvertisementsAsync(filterId)
        } catch(e) {
            console.log(e)
            return []
        } finally {
            this.setLoading(null)
        }
    }

    public async followAdvertisementAsync(advertisement: Advertisement, filterId: string) {
        this.setLoading('follow')

        try {
            AdvertisementFunctions.follow(advertisement)
            await agent.advertisements.follow(advertisement.link, filterId)
            await this.dbContext!.updateAdvertisementAsync(filterId, advertisement)

            return advertisement
        } catch(e) {
            console.log(e)
            return null
        } finally {
            this.setLoading(null)
        }
    }

    public async seeAdvertisementAsync(advertisement: Advertisement, filterId: string) {
        this.setLoading('see')

        try {
            AdvertisementFunctions.see(advertisement)
            await agent.advertisements.see(advertisement.link, filterId)
            await this.dbContext!.updateAdvertisementAsync(filterId, advertisement)

            return advertisement
        } catch(e) {
            console.log(e)
            return null
        } finally {
            this.setLoading(null)
        }
    }

    public async flagAdvertisementAsync(advertisement: Advertisement, filterId: string) {
        this.setLoading('flag')

        try {
            AdvertisementFunctions.flag(advertisement)
            await agent.advertisements.flag(advertisement.link)
            await this.dbContext!.updateAdvertisementAsync(filterId, advertisement)

            return advertisement
        } catch(e) {
            console.log(e)
            return null
        } finally {
            this.setLoading(null)
        }
    }
}