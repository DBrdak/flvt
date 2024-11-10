import {makeAutoObservable} from "mobx";
import agent from "../api/agent.ts";
import AdvertisementsDbContext from "../data/advertisementsDbContext.ts";
import {Advertisement, AdvertisementFunctions} from "../models/advertisement.ts";

export default class AdvertisementStore {
    loading: string | null = null
    dbContext: AdvertisementsDbContext | null = null
    preViewedAdvertisement: Advertisement | null = null
    viewedAdvertisement: Advertisement | null = null
    visibleAdvertisements: Advertisement[] = []

    constructor() {
        this.dbContext = new AdvertisementsDbContext()
        makeAutoObservable(this)
    }

    private setLoading(state: string | null) {
        this.loading = state
    }

    public setViewedAdvertisement(ad: Advertisement | null) {
        this.viewedAdvertisement = ad

        if(ad !== null) {
            const tempAds = this.visibleAdvertisements.filter(vAd => vAd.link !== ad.link)
            this.setVisibleAdvertisements([ad, ...tempAds])
        }
    }

    public setPreViewedAdvertisement(ad: Advertisement | null) {
        this.preViewedAdvertisement = ad

        if(ad !== null) {
            const tempAds = this.visibleAdvertisements.filter(vAd => vAd.link !== ad.link)
            this.setVisibleAdvertisements([ad, ...tempAds])
        }
    }

    public setVisibleAdvertisements(ads: Advertisement[]) {
        ads.filter(ad => ad.link !== this.preViewedAdvertisement?.link || ad.link !== this.viewedAdvertisement?.link)

        this.viewedAdvertisement && ads
            .unshift(this.viewedAdvertisement)
        this.preViewedAdvertisement && ads
            .unshift(this.preViewedAdvertisement)

        const map = new Map<string, Advertisement>(ads.map(ad => [ad.link, ad]))

        this.visibleAdvertisements = [...map.values()]
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