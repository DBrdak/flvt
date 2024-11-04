import {Filter} from "./filter.ts";

export interface Subscriber {
    email: string;
    tier: string;
    country: string;
    token?: string;
    filters: ReadonlyArray<Filter>;
}
