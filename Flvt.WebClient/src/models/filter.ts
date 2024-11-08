export interface Filter {
    id: string;
    name: string;
    location: string;
    minPrice?: number;
    maxPrice?: number;
    minRooms?: number;
    maxRooms?: number;
    minArea?: number;
    maxArea?: number;
    frequency: string;
    lastUsed: Date;
    nextUse: Date;
    tier: string;
    advertisementsFilePath?: string;
    newAdvertisementsCount: number
    followedAdvertisementsCount: number
    allAdvertisementsCount: number
}