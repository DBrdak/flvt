interface Props {
    currentView: 'list' | 'map'
    setCurrentView: (currentView: 'list' | 'map') => void
}

export default function ViewSelector({currentView}: Props) {

    return (
        <div />
    )
}