interface Props {
    size: 'xs' | 's' | 'm' | 'l' | 'xl'
}

export function Logo({size}: Props) {
    const widthMap: { [key in Props['size']]: string } = {
        xs: '10%',
        s: '25%',
        m: '50%',
        l: '75%',
        xl: '100%'
    }

    const width = widthMap[size]

    return (
        <img
            src="/public/logo.svg"
            alt="Flvt Logo"
            style={{ width: width, height: 'auto' }}
        />
    )
}