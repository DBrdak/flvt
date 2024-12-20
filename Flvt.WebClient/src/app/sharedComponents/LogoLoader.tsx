import './styles/LogoLoader.css'

interface Props {
    size: 'xs' | 's' | 'm' | 'l' | 'xl'
}

export default function LogoLoader({size}: Props) {

    const widthMap: { [key in Props['size']]: string } = {
        xs: '10%',
        s: '25%',
        m: '50%',
        l: '75%',
        xl: '100%'
    }

    const width = widthMap[size]

    // @ts-ignore
    return (
        <div className="loader" style={{width: width, height:'auto'}}>
            <svg xmlns="http://www.w3.org/2000/svg" xmlnsXlink="http://www.w3.org/1999/xlink" width="498"
                 zoomAndPan="magnify" viewBox="0 0 373.5 299.999988" height="400" preserveAspectRatio="xMidYMid meet"
                 version="1.0">
                <defs>
                    <clipPath id="5c0bbf9857">
                        <path d="M 166 30.199219 L 278.535156 30.199219 L 278.535156 169 L 166 169 Z M 166 30.199219 "
                              clipRule="nonzero"/>
                    </clipPath>
                    <clipPath id="f42672f8c9">
                        <path
                            d="M 37.660156 30.199219 L 186 30.199219 L 186 230.328125 L 37.660156 230.328125 Z M 37.660156 30.199219 "
                            clipRule="nonzero"/>
                    </clipPath>
                    <clipPath id="b9382d5a3f">
                        <path
                            d="M 90.550781 30.160156 L 206.390625 30.160156 L 206.390625 74.6875 L 90.550781 74.6875 Z M 90.550781 30.160156 "
                            clipRule="nonzero"/>
                    </clipPath>
                    <clipPath id="af85392980">
                        <path
                            d="M 37.417969 30.160156 L 154 30.160156 L 154 150 L 37.417969 150 Z M 37.417969 30.160156 "
                            clipRule="nonzero"/>
                    </clipPath>
                    <clipPath id="c9a8e003d7">
                        <path
                            d="M 37.484375 158.25 L 82 158.25 L 82 219.507812 L 37.484375 219.507812 Z M 37.484375 158.25 "
                            clipRule="nonzero"/>
                    </clipPath>
                    <clipPath id="5a4d5caa95">
                        <path
                            d="M 221.492188 30.160156 L 335.339844 30.160156 L 335.339844 63.714844 L 221.492188 63.714844 Z M 221.492188 30.160156 "
                            clipRule="nonzero"/>
                    </clipPath>
                    <clipPath id="08a261494e">
                        <path d="M 42 225 L 157.253906 225 L 157.253906 269.359375 L 42 269.359375 Z M 42 225 "
                              clipRule="nonzero"/>
                    </clipPath>
                    <clipPath id="1c138e1d8a">
                        <path
                            d="M 37.417969 100.386719 L 83 100.386719 L 83 265 L 37.417969 265 Z M 37.417969 100.386719 "
                            clipRule="nonzero"/>
                    </clipPath>
                </defs>
                <g className={'element element element element1'} clipPath="url(#5c0bbf9857)">
                    <path fill="#027af2"
                          d="M 194.402344 168.597656 L 166.09375 121.730469 L 221.597656 30.511719 L 278.511719 30.511719 Z M 194.402344 168.597656 "
                          fillOpacity="1" fillRule="nonzero"/>
                </g>
                <g className={'element element2'} clipPath="url(#f42672f8c9)">
                    <path fill="#027af2"
                          d="M 35.699219 30.160156 L 92.613281 30.160156 L 104.363281 49.601562 L 185.296875 183.695312 L 156.988281 230.328125 Z M 35.699219 30.160156 "
                          fillOpacity="1" fillRule="nonzero"/>
                </g>
                <g className={'element element3'} clipPath="url(#b9382d5a3f)">
                    <path fill="#002952"
                          d="M 90.65625 30.160156 L 90.65625 74.601562 L 175.253906 74.601562 L 206.617188 30.160156 L 90.65625 30.160156 "
                          fillOpacity="1" fillRule="nonzero"/>
                </g>
                <g className={'element element4'} clipPath="url(#af85392980)">
                    <path fill="#002952"
                          d="M 81.925781 105.554688 L 81.925781 30.160156 L 37.484375 30.160156 L 37.484375 149.996094 L 122.050781 149.996094 L 153.414062 105.554688 L 81.925781 105.554688 "
                          fillOpacity="1" fillRule="nonzero"/>
                </g>
                <g className={'element element5'} clipPath="url(#c9a8e003d7)">
                    <path fill="#002952"
                          d="M 37.484375 269.832031 L 81.925781 206.855469 L 81.925781 158.328125 L 37.484375 158.328125 L 37.484375 269.832031 "
                          fillOpacity="1" fillRule="nonzero"/>
                </g>
                <g className={'element element6'} clipPath="url(#5a4d5caa95)">
                    <path fill="#027af2"
                          d="M 325.902344 47.179688 L 316.226562 30.429688 L 219.519531 30.429688 L 209.847656 47.179688 L 200.183594 63.933594 L 190.511719 80.6875 L 180.835938 97.441406 L 171.164062 114.183594 L 161.5 130.933594 L 151.8125 147.699219 L 142.152344 164.441406 L 122.804688 197.945312 L 161.488281 197.945312 L 171.164062 181.195312 L 180.835938 164.441406 L 190.511719 147.6875 L 200.183594 130.933594 L 209.847656 114.183594 L 219.519531 97.441406 L 229.195312 80.6875 L 238.859375 63.933594 L 335.574219 63.933594 Z M 325.902344 47.179688 "
                          fillOpacity="1" fillRule="nonzero"/>
                </g>
                <g className={'element element7'} clipPath="url(#08a261494e)">
                    <path fill="#002952"
                          d="M 87.484375 225.128906 L 157.449219 225.128906 L 157.449219 269.832031 L 42.78125 269.832031 Z M 87.484375 225.128906 "
                          fillOpacity="1" fillRule="evenodd"/>
                </g>
                <g className={'element element8'} clipPath="url(#1c138e1d8a)">
                    <path fill="#002952"
                          d="M 37.464844 100.386719 L 82.167969 100.386719 L 82.167969 219.8125 L 37.464844 264.515625 Z M 37.464844 100.386719 "
                          fillOpacity="1" fillRule="evenodd"/>
                </g>
            </svg>
        </div>
    )
}