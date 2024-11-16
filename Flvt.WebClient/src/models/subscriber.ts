import {Filter} from "./filter.ts";

export interface Subscriber {
    email: string
    isEmailVerified: boolean
    tier: string
    country: string
    token: string | null
    filters: Filter[]
}
