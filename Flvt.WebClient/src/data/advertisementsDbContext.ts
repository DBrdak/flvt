import {IDBPDatabase, openDB} from "idb";
import {Advertisement} from "../models/advertisement.ts";

export default class AdvertisementsDbContext {

    private readonly dbPromise: Promise<IDBPDatabase>
    public readonly filterId: string
    private readonly dbName: string
    private readonly rwMode = 'readwrite'

    constructor(filterId: string) {
        this.filterId = filterId
        const dbName = `advertisements/${this.filterId}`
        this.dbName = dbName

        this.dbPromise = openDB('Flvt', 1, {
            upgrade(db) {
                if (!db.objectStoreNames.contains(dbName)) {
                    db.createObjectStore(dbName, { keyPath: 'link' })
                }
            }
        });
    }

    public async saveAdvertisementsAsync(advertisements: Advertisement[]): Promise<void> {
        const db = await this.dbPromise
        const tx = db.transaction(this.dbName, this.rwMode)
        const store = tx.objectStore(this.dbName)

        await store.clear()

        const addPromises = advertisements.map(ad => store.add(ad))

        await Promise.all(addPromises)

        await tx.done
    }

    public async getAdvertisementsAsync(): Promise<Advertisement[]> {
        const db = await this.dbPromise
        return db.getAll(this.dbName)
    }

    public async updateAdvertisementAsync(advertisement: Advertisement): Promise<void> {
        const db = await this.dbPromise
        const tx = db.transaction(this.dbName, this.rwMode)
        const store = tx.objectStore(this.dbName)

        await store.put(advertisement)

        await tx.done
    }

    public static async removeDbAsync(filterId: string): Promise<void> {
        const dbName = `advertisements/${filterId}`

        const db = await openDB('Flvt', 1, {
            upgrade(db) {
                if (!db.objectStoreNames.contains(dbName)) {
                    db.createObjectStore(dbName, { keyPath: 'link' })
                }
            }
        })

        const tx = db.transaction(dbName, 'readwrite')
        await tx.objectStore(dbName).clear()
        await tx.done
    }
}