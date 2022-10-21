import React, { useState } from 'react';
import { useQuery } from 'react-query';
import { fetchPhoto } from '../fetchers/photos';
import './Screensaver.css';

const Screensaver = ({ hideScreensaver }) => {

    const { isError, isLoading, data, error } = useQuery(['photo'], fetchPhoto, { refetchInterval: 5000 });

    return (
        <div id="screensaver" className="screensaver" onClick={hideScreensaver}>
            <img src={data} />
        </div>
    );
}

export default Screensaver;
