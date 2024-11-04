import {Address} from "./address.ts"
import {Coordinates} from "./coordinates.ts"
import {Money} from "./money.ts"
import {RoomsCount} from "./roomsCount.ts"
import {Floor} from "./floor.ts"
import {Area} from "./area.ts"

export class Advertisement {
    link: string
    address: Address
    geolocation?: Coordinates
    description: string
    contactType: string
    price: Money
    deposit?: Money
    rooms: RoomsCount
    floor: Floor
    area: Area
    facilities: string[]
    addedAt?: Date
    updatedAt?: Date
    availableFrom?: string
    pets?: boolean
    photos: string[]
    isFlagged: boolean
    wasSeen: boolean
    isNew: boolean
    isFollowed: boolean

    constructor(
        link: string,
        address: Address,
        description: string,
        contactType: string,
        price: Money,
        rooms: RoomsCount,
        floor: Floor,
        area: Area,
        facilities: string[],
        photos: string[],
        isFlagged: boolean,
        wasSeen: boolean,
        isNew: boolean,
        isFollowed: boolean,
        geolocation?: Coordinates,
        deposit?: Money,
        addedAt?: Date,
        updatedAt?: Date,
        availableFrom?: string,
        pets?: boolean
    ) {
        this.link = link
        this.address = address
        this.geolocation = geolocation
        this.description = description
        this.contactType = contactType
        this.price = price
        this.deposit = deposit
        this.rooms = rooms
        this.floor = floor
        this.area = area
        this.facilities = facilities
        this.addedAt = addedAt
        this.updatedAt = updatedAt
        this.availableFrom = availableFrom
        this.pets = pets
        this.photos = photos
        this.isFlagged = isFlagged
        this.wasSeen = wasSeen
        this.isNew = isNew
        this.isFollowed = isFollowed
    }

    follow() {
        this.isFollowed = !this.isFollowed
    }

    flag() {
        this.isFlagged = true
    }

    see() {
        this.wasSeen = true
    }
}
