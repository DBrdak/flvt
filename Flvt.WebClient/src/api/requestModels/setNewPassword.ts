export interface SetNewPasswordBody {
    subscriberEmail: string
    verificationCode: string
    newPassword: string
}