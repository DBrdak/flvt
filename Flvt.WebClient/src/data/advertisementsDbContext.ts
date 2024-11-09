import {openDB} from "idb";
import {Advertisement} from "../models/advertisement.ts";

export default class AdvertisementsDbContext {
    private readonly dbName: string = "Flvt";
    private readonly rwMode = 'readwrite'

    constructor() {
    }

    private getAdvertisementsStoreName(filterId: string) {
        return `advertisements/${filterId}`
    }

    private async getOrCreateStore(storeName: string) {
        let db = await openDB(this.dbName);

        if (db.objectStoreNames.contains(storeName)) {
            return db;
        }

        const newVersion = db.version + 1;
        db.close();

        db = await openDB(this.dbName, newVersion, {
            upgrade(db) {
                if (!db.objectStoreNames.contains(storeName)) {
                    db.createObjectStore(storeName, { keyPath: 'link' });
                }
            }
        });

        return db;
    }

    public async saveAdvertisementsAsync(filterId: string, advertisements: Advertisement[]): Promise<void> {
        const storeName = this.getAdvertisementsStoreName(filterId);

        const currentAds = await this.getAdvertisementsAsync(filterId);

        if (currentAds.length === advertisements.length) {
            return;
        }

        const db = await this.getOrCreateStore(storeName);
        const tx = db.transaction(storeName, this.rwMode);
        const store = tx.objectStore(storeName);

        await store.clear();
        const addPromises = advertisements.map(ad => store.add(ad));
        await Promise.all(addPromises);

        await tx.done;
    }

    public async getAdvertisementsAsync(filterId: string): Promise<Advertisement[]> {
        const storeName = this.getAdvertisementsStoreName(filterId)
        const db = await this.getOrCreateStore(storeName)
        return db.getAll(this.getAdvertisementsStoreName(filterId))
    }

    public async updateAdvertisementAsync(filterId: string, advertisement: Advertisement): Promise<void> {
        const storeName = this.getAdvertisementsStoreName(filterId)
        const db = await this.getOrCreateStore(storeName)
        const tx = db.transaction(storeName, this.rwMode)
        const store = tx.objectStore(storeName)

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