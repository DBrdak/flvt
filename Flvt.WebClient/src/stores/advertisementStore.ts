import {makeAutoObservable} from "mobx";
import agent from "../api/agent.ts";
import AdvertisementsDbContext from "../data/advertisementsDbContext.ts";
import {Advertisement, AdvertisementFunctions} from "../models/advertisement.ts";

export default class AdvertisementStore {
    loading: string | null = null
    dbContext: AdvertisementsDbContext | null = null
    advertisements: Advertisement[] = []
    preViewedAdvertisement: Advertisement | null = null
    viewedAdvertisement: Advertisement | null = null
    visibleAdvertisements: Advertisement[] = []
    showFollowedAds: boolean = true
    showSeenAds: boolean = true
    showNotSeenAds: boolean = true
    showNewAds: boolean = true

    constructor() {
        this.dbContext = new AdvertisementsDbContext()
        makeAutoObservable(this)
    }

    private setLoading(state: string | null) {
        this.loading = state
    }

    private setAdvertisements(ads: Advertisement[]) {
        this.advertisements = ads
        this.visibleAdvertisements = ads
    }

    public setViewedAdvertisement(ad: Advertisement | null) {
        this.viewedAdvertisement = ad
    }

    public setPreViewedAdvertisement(ad: Advertisement | null) {
        this.preViewedAdvertisement = ad
    }

    public setVisibleAdvertisements(ads: Advertisement[]) {

        const map = new Map<string, Advertisement>(ads.map(ad => [ad.link, ad]))

        this.visibleAdvertisements = [...map.values()]
    }

    public setShowFollowedAds(state?: boolean) {
        this.showFollowedAds = state || !this.showFollowedAds
        this.filterAds()
    }

    public setShowSeenAds(state?: boolean) {
        this.showSeenAds = state || !this.showSeenAds
        this.filterAds()
    }

    public setShowNotSeenAds(state?: boolean) {
        this.showNotSeenAds = state || !this.showNotSeenAds
        this.filterAds()
    }

    public setShowNewAds(state?: boolean) {
        this.showNewAds = state || !this.showNewAds
        this.filterAds()
    }

    private filterAds() {
        const ads: Advertisement[] = []

        this.showNotSeenAds && ads.push(...this.advertisements.filter(ad => !ad.wasSeen))

        this.showSeenAds && ads.push(...this.advertisements.filter(ad => ad.wasSeen))

        this.showNewAds && ads.push(...this.advertisements.filter(ad => ad.isNew))

        this.showFollowedAds && ads.push(...this.advertisements.filter(ad => ad.isFollowed))

        const adsMap = new Map<string, Advertisement>()
        ads.forEach(ad => adsMap.set(ad.link, ad))

        this.setVisibleAdvertisements([...adsMap.values()])
    }

    resetView() {
        this.setShowFollowedAds(true)
        this.setShowSeenAds(true)
        this.setShowNotSeenAds(true)
        this.setShowNewAds(true)
    }

    public async loadAdvertisementsAsync(filterId: string) {
        this.setLoading('init')

        try {
            const advertisementsFileUrl = filterId === 'preview' ?
                await agent.advertisements.getPreview() :
                await agent.advertisements.getByFilter(filterId)
            const fileGetResponse = await fetch(advertisementsFileUrl)

            if(fileGetResponse.status !== 200){
                 return false
            }

            const advertisements: Advertisement[] = await fileGetResponse.json()

            await this.dbContext!.saveAdvertisementsAsync(filterId, advertisements)

            this.setAdvertisements(await this.dbContext!.getAdvertisementsAsync(filterId))

            return true
        } catch(e) {
            console.log(e)
            return false
        } finally {
            this.setLoading(null)
        }
    }

    public async followAdvertisementAsync(advertisement: Advertisement, filterId: string) {
        this.setLoading('follow')

        try {
            AdvertisementFunctions.follow(advertisement)

            Promise.all([
                filterId !== 'preview' && await agent.advertisements.follow(advertisement.link, filterId, !advertisement.isFollowed),
                await this.dbContext!.updateAdvertisementAsync(filterId, advertisement)
            ])

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
            advertisement.wasSeen = true
            advertisement.isNew = false

            Promise.all([
                filterId !== 'preview' && await agent.advertisements.see(advertisement.link, filterId),
                await this.dbContext!.updateAdvertisementAsync(filterId, advertisement)
            ])

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
            advertisement.isFlagged = true

            Promise.all([
                filterId !== 'preview' && agent.advertisements.flag(advertisement.link),
                this.dbContext!.updateAdvertisementAsync(filterId, advertisement)
            ])

            return advertisement
        } catch(e) {
            advertisement.isFlagged = false
            console.log(e)
            return null
        } finally {
            this.setLoading(null)
        }
    }
}