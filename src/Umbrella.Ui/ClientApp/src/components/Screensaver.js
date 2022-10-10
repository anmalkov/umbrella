import React from 'react';
import './Screensaver.css';

const Screensaver = ({ hideScreensaver }) => {
    return (
        <div id="screensaver" className="screensaver" onClick={hideScreensaver}>
            Screensaver
        </div>
    );
}

export default Screensaver;
