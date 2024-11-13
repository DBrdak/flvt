import L from 'leaflet'
import {colorSchemes} from "../../../theme/themePrimitives.ts";
import {alpha} from "@mui/material/styles";

const getMarkerIcon = (color: string, bounceClass: string) => `
      <svg
        class="${bounceClass}"
        xmlns="http://www.w3.org/2000/svg"
        height="50"
        viewBox="0 0 24 24"
        width="50"
        fill="${color}"
      >
        <path d="M12 2C8.13 2 5 5.13 5 9c0 5.25 7 13 7 13s7-7.75 7-13c0-3.87-3.13-7-7-7zm0 9.5c-1.38 0-2.5-1.12-2.5-2.5s1.12-2.5 2.5-2.5 2.5 1.12 2.5 2.5-1.12 2.5-2.5 2.5z"/>
      </svg>
`

export const createIcon = (colorType: 'red' | 'blue' | 'grey' | 'green' | 'black', isBouncing: boolean) => {
    const bounceClass = isBouncing ? 'bounce' : ''

    const color = colorType === 'red'
        ? colorSchemes.dark.palette.error.light
        : colorType === 'blue'
            ? colorSchemes.dark.palette.primary.main
            : colorType === 'grey'
                ? alpha(colorSchemes.dark.palette.grey[900], 0.6)
                : colorType === 'green'
                    ? colorSchemes.dark.palette.success.light
                    : colorSchemes.dark.palette.grey[900]

    const iconHtml = getMarkerIcon(color, bounceClass)

    return L.divIcon({
        html: `<div style="transform: translate(-50%, -50%)">${iconHtml}</div>`,
        className: '',
        iconSize: [40, 40],
        iconAnchor: [20, 40],
        shadowSize: [25, 25]
    })
}
