import axios, {AxiosResponse} from "axios"
import {toast} from "react-toastify"
import {Subscriber} from "../models/subscriber.ts";
import {Filter} from "../models/filter.ts";
import {AddBasicFilterBody} from "./requestModels/addBasicFilter.ts";
import {LoginBody} from "./requestModels/login.ts";
import {RegisterBody} from "./requestModels/register.ts";
import {VerifyEmailBody} from "./requestModels/verifyEmail.ts";
import {SetNewPasswordBody} from "./requestModels/setNewPassword.ts";
import {store} from "../stores/store.ts";

const sleep = (delay: number) => {
    return new Promise((resolve) => {
        setTimeout(resolve, delay)
    })
}

axios.defaults.baseURL = import.meta.env.API_URL

axios.interceptors.request.use(config => {
    const token = store.subscriberStore.getToken()

    config.headers.Authorization = `Bearer ${token}`

    return config
})

const responseBody = <T> (response: AxiosResponse<T>) => response.data;

axios.interceptors.response.use(async(response) => {
        if(import.meta.env.NODE_ENV === "development") {
            await sleep(1000)
        }

        return response
    }, (error) => {
        switch(error.response.status) {
            case 400:
                toast.error(error.response.data.error.message)
                break
            case 401:
                toast.error('Unauthorized')
                toast.clearWaitingQueue()
                break
            case 403:
                toast.error(error.response.data.error.message)
                break
            case 404:
                toast.error(error.response.data.error.message)
                return Promise.reject();
                break
            default:
                toast.error('Oops, something went wrong, please try again later')
                break
        }
        console.log(error)
        return Promise.reject(error);
    }
);

const subscribers = {
    get: () => axios.get<Subscriber>('/v1/subscribers').then(responseBody),
    addBasicFilter: (body: AddBasicFilterBody) => axios.post<Filter>('/v1/subscribers/filters/basic', body).then(responseBody),
    removeFilter: (filterId: string) => axios.delete(`/v1/subscribers/filters/${filterId}`)
}

const auth = {
    login: (body: LoginBody) => axios.post<Subscriber>('/v1/auth/login', body).then(responseBody),
    register: (body: RegisterBody) => axios.post<Subscriber>(`/v1/auth/register`, body).then(responseBody),
    verifyEmail: (body: VerifyEmailBody) => axios.post<Subscriber>(`/v1/auth/verify`, body).then(responseBody),
    resendEmail: (purpose: string, email: string) => axios.put('/v1/auth/resend', {URLSearchParams: {purpose, email}}),
    requestNewPassword: (email: string) => axios.put('/v1/auth/new-password/request', {URLSearchParams: {email}}),
    setNewPassword: (body: SetNewPasswordBody) => axios.post<Subscriber>('/v1/auth/new-password/set', body).then(responseBody),
}

const advertisements = {
    getByFilter: (filterId: string) => axios.get<string>(`/v1/advertisements?filterId=${filterId}`).then(responseBody),
    follow: (advertisementId: string, filterId: string) =>
        axios.put('/v1/advertisements/follow', {URLSearchParams: {advertisementId, filterId}}),
    flag: (advertisementId: string) =>
        axios.put('/v1/advertisements/flag', {URLSearchParams: {advertisementId}}),
    see: (advertisementId: string, filterId: string) =>
        axios.put('/v1/advertisements/see', {URLSearchParams: {advertisementId, filterId}}),
}

const agent = {
    subscribers,
    auth,
    advertisements
}

export default agent;