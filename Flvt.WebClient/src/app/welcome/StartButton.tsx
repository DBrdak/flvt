import {useTheme} from "@mui/material/styles";

interface Props {
    onClick: () => void
}

const StartButton = ({onClick}: Props) => {
    const theme = useTheme()

    return (
        <div className="wrapper">
            <div className="link_wrapper">
                <button className={'start-btn'} style={{
                        boxShadow: theme.palette.primary["800"],
                        background: theme.palette.primary["800"]
                    }}
                    onClick={() => onClick()}
                >
                    Start here
                </button>
            </div>
        </div>
    )
}

export default StartButton