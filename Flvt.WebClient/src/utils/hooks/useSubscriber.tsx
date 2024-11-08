import {useEffect, useState} from "react";
import {useStore} from "../../stores/store.ts";
import {Subscriber} from "../../models/subscriber.ts";
import {useNavigate} from "react-router-dom";

const useSubscriber = () => {
    const {subscriberStore} = useStore()
    const [subscriber, setSubscriber] = useState<Subscriber | null>(null)
    const navigate = useNavigate()

    useEffect(() => {
        const loadSubscriber = async () => {
            const sub = await subscriberStore.loadCurrentSubscriberAsync()
            setSubscriber(sub)

            if(!subscriberStore.currentSubscriber) {
                navigate('/')
                return
            }

            if(!subscriberStore.currentSubscriber.isEmailVerified) {
                navigate('/verification')
                return
            }
        }

        loadSubscriber()
    }, [])

    return subscriber
}

export default useSubscriber