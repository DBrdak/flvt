export interface AddBasicFilterBody {
    name: string;
    city: string;
    minPrice: number | null;
    maxPrice: number | null;
    minRooms: number | null;
    maxRooms: number | null;
    minArea: number | null;
    maxArea: number | null;
}
