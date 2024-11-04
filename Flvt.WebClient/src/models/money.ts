import {Currency} from "./currency.ts";

export interface Money {
    amount: number;
    currency: Currency;
}