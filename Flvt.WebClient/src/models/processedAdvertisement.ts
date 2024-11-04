import {Address} from "./address.ts";
import {Coordinates} from "./coordinates.ts";
import {Money} from "./money.ts";
import {RoomsCount} from "./roomsCount.ts";
import {Floor} from "./floor.ts";
import {Area} from "./area.ts";

export interface ProcessedAdvertisement {
    link: string;
    address: Address;
    geolocation?: Coordinates;
    description: string;
    contactType: string;
    price: Money;
    deposit?: Money;
    rooms: RoomsCount;
    floor: Floor;
    area: Area;
    facilities: string[];
    addedAt?: Date;
    updatedAt?: Date;
    availableFrom?: string;
    pets?: boolean;
    photos: string[];
    isFlagged: boolean;
    wasSeen: boolean;
    isNew: boolean;
    isFollowed: boolean;
}