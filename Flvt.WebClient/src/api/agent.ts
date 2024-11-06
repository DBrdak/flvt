import axios, { AxiosResponse } from "axios"
import { toast } from "react-toastify"
import { Subscriber } from "../models/subscriber.ts"
import { Filter } from "../models/filter.ts"
import { AddBasicFilterBody } from "./requestModels/addBasicFilter.ts"
import { LoginBody } from "./requestModels/login.ts"
import { RegisterBody } from "./requestModels/register.ts"
import { VerifyEmailBody } from "./requestModels/verifyEmail.ts"
import { SetNewPasswordBody } from "./requestModels/setNewPassword.ts"
import { store } from "../stores/store.ts"

axios.defaults.baseURL = import.meta.env.VITE_API_URL

axios.interceptors.request.use(config => {
    const token = store.subscriberStore.getToken()
    if (token) {
        config.headers.Authorization = `Bearer ${token}`
    }
    return config
})

const responseBody = <T>(response: AxiosResponse<T>) => response.data
const resultValue = <T>(response: Result<T>) => response.value

axios.interceptors.response.use(
    async (response) => {
        return response
    },
    (error) => {
        switch (error.response.status) {
            case 400:
                toast.error(error.response.data.error.message)
                break
            case 401:
                break
            case 403:
                toast.error(error.response.data.error.message)
                break
            case 404:
                toast.error(error.response.data.error.message)
                return Promise.reject()
            default:
                toast.error('Oops, something went wrong, please try again later')
                break
        }

        if (import.meta.env.NODE_ENV === "development") {
            console.log(error)
        }
        return Promise.reject(error)
    }
)

interface Error {
    message: string
}

interface Result<T> {
    error: Error
    isFailure: boolean
    isSuccess: boolean
    value: T
}

const requests = {
    get: <T>(path: string) => axios.get<Result<T>>(path).then(responseBody),
    post: <T>(path: string, body?: {}) => axios.post<Result<T>>(path, body).then(responseBody),
    put: <T>(path: string, body?: {}) => axios.put<Result<T>>(path, body).then(responseBody),
    delete: (path: string) => axios.delete(path).then(responseBody)
}

const subscribers = {
    get: () => requests.get<Subscriber>('/v1/subscribers').then(resultValue),
    addBasicFilter: (body: AddBasicFilterBody) =>
        requests.post<Filter>('/v1/subscribers/filters/basic', body).then(resultValue),
    removeFilter: (filterId: string) =>
        requests.delete(`/v1/subscribers/filters/${filterId}`)
}

const auth = {
    login: (body: LoginBody) =>
        requests.post<Subscriber>('/v1/auth/login', body).then(resultValue),
    register: (body: RegisterBody) =>
        requests.post<Subscriber>('/v1/auth/register', body).then(resultValue),
    verifyEmail: (body: VerifyEmailBody) =>
        requests.post<Subscriber>('/v1/auth/verify', body).then(resultValue),
    resendEmail: (purpose: string, email: string) =>
        requests.put(`/v1/auth/resend?purpose=${purpose}&email=${email}`),
    requestNewPassword: (email: string) =>
        requests.put(`/v1/auth/new-password/request?email=${email}`),
    setNewPassword: (body: SetNewPasswordBody) =>
        requests.post<Subscriber>('/v1/auth/new-password/set', body).then(resultValue),
}

const advertisements = {
    getByFilter: (filterId: string) =>
        requests.get<string>(`/v1/advertisements?filterId=${filterId}`).then(resultValue),
    follow: (advertisementId: string, filterId: string) =>
        requests.put(`/v1/advertisements/follow?advertisementId=${advertisementId}&filterId=${filterId}`),
    flag: (advertisementId: string) =>
        requests.put(`/v1/advertisements/flag?advertisementId=${advertisementId}`),
    see: (advertisementId: string, filterId: string) =>
        requests.put(`/v1/advertisements/see?advertisementId=${advertisementId}&filterId=${filterId}`),
}

const agent = {
    subscribers,
    auth,
    advertisements
}

export default agent
