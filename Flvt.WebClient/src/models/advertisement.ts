import {Address} from "./address.ts"
import {Coordinates} from "./coordinates.ts"
import {Money} from "./money.ts"
import {Area} from "./area.ts"

export interface Advertisement {
    link: string
    address: Address
    geolocation?: Coordinates
    description: string
    isPrivate: boolean
    price: Money
    deposit?: Money
    fee?: Money
    roomsCount: number
    area: Area
    addedAt?: Date
    updatedAt?: Date
    availableFrom?: string
    pets?: boolean
    photos: string[]
    isFlagged: boolean
    wasSeen: boolean
    isNew: boolean
    isFollowed: boolean
}

export class AdvertisementFunctions {
    public static follow(ad: Advertisement) {
        ad.isFollowed = !ad.isFollowed
    }

    public static flag(ad: Advertisement) {
        ad.isFlagged = true
    }

    public static see(ad: Advertisement) {
        ad.wasSeen = true
    }
}
