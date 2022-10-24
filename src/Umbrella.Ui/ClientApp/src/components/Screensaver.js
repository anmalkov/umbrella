import React, { useEffect, useState } from 'react';
import { useQuery } from 'react-query';
import { fetchPhoto } from '../fetchers/photos';
import './Screensaver.css';

const Screensaver = ({ hideScreensaver }) => {

    const { isError, isLoading, data, error } = useQuery(['photo'], fetchPhoto, { refetchInterval: 5000 });

    const [currentDate, setCurrentDate] = useState(new Date());

    useEffect(() => {
        const timer = setInterval(() => setCurrentDate(new Date(), 1000));
        return () => {
            clearInterval(timer);
        }
    }, []);

    return (
        <div id="screensaver" className="screensaver" onClick={hideScreensaver}>
            {data ? (
                <img src={data} />
            ) : null
            }
            <div className="position-absolute bottom-0 start-0 mb-4 mx-5">
                <h1 className="display-1 p-0 px-1 m-0">{currentDate.toLocaleTimeString('en-us', { hour: "2-digit", minute: "2-digit", hour12: false })}</h1>
                <h1 className="display-6 p-0 px-1 m-0 mb-5">{currentDate.toLocaleDateString('en-us', { weekday: "long", month: "long", day: "numeric" })}</h1>
            </div>
        </div>
    );
}

export default Screensaver;
