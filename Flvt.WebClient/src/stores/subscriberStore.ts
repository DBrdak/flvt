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
    public loading: string | null = null

    constructor() {
        makeAutoObservable(this);

        if(!this.token) {
            this.token = localStorage.getItem('jwt')

            if(!this.token) {
                this.loadCurrentSubscriberAsync()
            }
        }
    }

    public getToken() {
        return this.token || localStorage.getItem('jwt');
    }

    private setLoading(action: string | null): void {
        this.loading = action
    }

    private setCurrentSubscriber(subscriber: Subscriber | null) {
        this.currentSubscriber = subscriber

        if (subscriber?.token !== null) {
            this.setToken(subscriber!.token)
        }
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
        this.setLoading('register')

        try {
            const subscriber = await agent.auth.register(request)
            this.setCurrentSubscriber(subscriber)

            return true
        } catch {
            return false
        } finally {
            this.setLoading(null)
        }
    }

    public async loginAsync(request: LoginBody) {
        this.setLoading('login')

        try {
            const subscriber = await agent.auth.login(request)
            this.setCurrentSubscriber(subscriber)

            return true
        } catch {
            return false
        } finally {
            this.setLoading(null)
        }
    }

    public async loadCurrentSubscriberAsync() {
        this.setLoading('init')

        try {
            const subscriber = await agent.subscribers.get()
            this.setCurrentSubscriber(subscriber)

            return subscriber
        } catch {
            return null
        } finally {
            this.setLoading(null)
        }
    }

    public async verifyEmailAsync(verificationCode: string) {
        this.setLoading('verify')

        try {
            if(!this.currentSubscriber?.email){
                await this.loadCurrentSubscriberAsync()
            }
            if(!this.currentSubscriber?.email){
                return false
            }

            const request: VerifyEmailBody = {email: this.currentSubscriber!.email, verificationCode: verificationCode}
            const subscriber = await agent.auth.verifyEmail(request)
            this.setCurrentSubscriber(subscriber)
        } catch (e) {
            return false
        } finally {
            this.setLoading(null)
        }
    }

    public async requestNewPasswordAsync(email: string){
        this.setLoading('requestNewPassword')

        try {
            await agent.auth.requestNewPassword(email)
            return true
        } catch (e) {
            return false
        } finally {
            this.setLoading(null)
        }
    }

    public async setNewPasswordAsync(request: SetNewPasswordBody){
        this.setLoading('setNewPassword')

        try {
            const subscriber = await agent.auth.setNewPassword(request)
            this.setCurrentSubscriber(subscriber)
            return true
        } catch (e) {
            return false
        } finally {
            this.setLoading(null)
        }
    }

    public async resendVerificationEmailAsync(email: string) {
        this.setLoading('resendCode')

        try {
            await agent.auth.resendEmail('verification', email)
            return true
        } catch (e) {
            return false
        } finally {
            this.setLoading(null)
        }
    }

    public async resendNewPasswordEmailAsync(email: string) {
        this.setLoading('resendPassword')

        try {
            await agent.auth.resendEmail('newPassword', email)
            return true
        } catch (e) {
            return false
        } finally {
            this.setLoading(null)
        }
    }

    public async addBasicFilterAsync(request: AddBasicFilterBody){
        this.setLoading('addBasicFilter')

        try {
            const filter = await agent.subscribers.addBasicFilter(request)
            this.addFilter(filter)
            return filter
        } catch (e) {
            return null
        } finally {
            this.setLoading(null)
        }
    }

    public async removeFilterAsync(filterId: string){
        this.setLoading('removeFilter')

        try {
            await agent.subscribers.removeFilter(filterId)
            this.removeFilter(filterId)
            await AdvertisementsDbContext.removeDbAsync(filterId)
            return true
        } catch (e) {
            return false
        } finally {
            this.setLoading(null)
        }
    }
}