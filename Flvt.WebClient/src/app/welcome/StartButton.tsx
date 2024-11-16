interface Props {
    onClick: () => void
}

const StartButton = ({onClick}: Props) => {
    return (
        <div className="wrapper">
            <div className="link_wrapper">
                <button className={'start-btn'} onClick={() => onClick()}>Start here</button>
            </div>
        </div>
    )
}

export default StartButton