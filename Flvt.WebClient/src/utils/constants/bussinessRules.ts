
export const availableCities = [
    'Warszawa',
    'Piaseczno',
    'Wołomin',
    'Piastów',
    'Ząbki',
    'Łomianki',
    'Marki',
    'Pruszków',
    `Legionowo`
]

export const emailAddressPattern = /^[\w-\\.]+@([\w-]+\.)+[\w-]{2,4}$/
export const passwordPattern =  /(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}/
export const verificationCodePattern = /^\d{6}$/