import {Subscriber} from "../models/subscriber.ts";
import agent from "../api/agent.ts";
import {makeAutoObservable} from "mobx";
import {RegisterBody} from "../api/requestModels/register.ts";
import {LoginBody} from "../api/requestModels/login.ts";
import {VerifyEmailBody} from "../api/requestModels/verifyEmail.ts";
import {SetNewPasswordBody} from "../api/requestModels/setNewPassword.ts";
import {AddBasicFilterBody} from "../api/requestModels/addBasicFilter.ts";
import {Filter} from "../models/filter.ts";
import AdvertisementsDbContext from "../data/advertisementsDbContext.ts";

export default class SubscriberStore {
    public currentSubscriber: Subscriber | null = null
    private token: string | null = null
    public loading: boolean = false

    constructor() {
        makeAutoObservable(this);

        if(!this.token) {
            this.token = localStorage.getItem('jwt')

            this.setLoading(true)
            this.loadCurrentSubscriberAsync().then(() => this.setLoading(false))
        }
    }

    private setLoading(state: boolean) {
        this.loading = state
    }

    private setCurrentSubscriber(subscriber: Subscriber | null) {
        this.currentSubscriber = subscriber
        subscriber?.token !== null && this.setToken(subscriber!.token)
    }

    private setToken = (token: string | null) => {
        this.token === null ? localStorage.removeItem('jwt') : localStorage.setItem('jwt', token!)

        this.token = token
    }

    private addFilter(filter: Filter){
        this.currentSubscriber?.filters.push(filter)
    }

    private removeFilter(filterId: string){
        if(this.currentSubscriber !== null){
            this.currentSubscriber.filters = this.currentSubscriber.filters.filter(f => f.id !== filterId)
        }
    }

    public logOut() {
        this.setToken(null)
        this.setCurrentSubscriber(null)
    }

    public async registerAsync(request: RegisterBody) {
        this.setLoading(true)

        try {
            const subscriber = await agent.auth.register(request)
            this.setCurrentSubscriber(subscriber)

            return true
        } catch {
            return false
        } finally {
            this.setLoading(false)
        }
    }

    public async loginAsync(request: LoginBody) {
        this.setLoading(true)

        try {
            const subscriber = await agent.auth.login(request)
            this.setCurrentSubscriber(subscriber)

            return true
        } catch {
            return false
        } finally {
            this.setLoading(false)
        }
    }

    public async loadCurrentSubscriberAsync() {
        this.setLoading(true)

        try {
            const subscriber = await agent.subscribers.get()
            this.setCurrentSubscriber(subscriber)
        } catch {
            return
        } finally {
            this.setLoading(false)
        }
    }

    public async verifyEmailAsync(request: VerifyEmailBody) {
        this.setLoading(true)

        try {
            const subscriber = await agent.auth.verifyEmail(request)
            this.setCurrentSubscriber(subscriber)
        } catch (e) {

        } finally {
            this.setLoading(false)
        }
    }

    public async requestNewPasswordAsync(email: string){
        this.setLoading(true)

        try {
            await agent.auth.requestNewPassword(email)
            return true
        } catch (e) {
            return false
        } finally {
            this.setLoading(false)
        }
    }

    public async setNewPasswordAsync(request: SetNewPasswordBody){
        this.setLoading(true)

        try {
            const subscriber = await agent.auth.setNewPassword(request)
            this.setCurrentSubscriber(subscriber)
            return true
        } catch (e) {
            return false
        } finally {
            this.setLoading(false)
        }
    }

    public async resendVerificationEmailAsync(email: string) {
        this.setLoading(true)

        try {
            await agent.auth.resendEmail('verification', email)
            return true
        } catch (e) {
            return false
        } finally {
            this.setLoading(false)
        }
    }

    public async resendNewPasswordEmailAsync(email: string) {
        this.setLoading(true)

        try {
            await agent.auth.resendEmail('newPassword', email)
            return true
        } catch (e) {
            return false
        } finally {
            this.setLoading(false)
        }
    }

    public async addBasicFilterAsync(request: AddBasicFilterBody){
        this.setLoading(true)

        try {
            const filter = await agent.subscribers.addBasicFilter(request)
            this.addFilter(filter)
            return filter
        } catch (e) {
            return null
        } finally {
            this.setLoading(false)
        }
    }

    public async removeFilterAsync(filterId: string){
        this.setLoading(true)

        try {
            await agent.subscribers.removeFilter(filterId)
            this.removeFilter(filterId)
            await AdvertisementsDbContext.removeDbAsync(filterId)
            return true
        } catch (e) {
            return false
        } finally {
            this.setLoading(false)
        }
    }
}