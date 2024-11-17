interface Props {
    onClick: () => void
}

const StartButton = ({onClick}: Props) => {
    return (
        <div className="wrapper">
            <div className="link_wrapper">
                <button className={'preview-btn'} onClick={() => onClick()}>Preview</button>
            </div>
        </div>
    )
}

export default StartButton