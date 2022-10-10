import React, { useState, useCallback } from 'react';
import { Container } from 'reactstrap';
import NavMenu from './NavMenu';
import Screensaver from './Screensaver';

const Layout = (props) => {

    const [screensaverShown, setScreensaverShown] = useState(true);
    const [screensaverTimeout, setScreensaverTimeout] = useState(-1);

    const hideScreensaver = () => {
        setScreensaverShown(false);
        startTimeout();
    }
    const showScreensaver = () => {
        setScreensaverShown(true);
    }

    const startTimeout = () => {
        clearTimeout(screensaverTimeout);
        console.log("new timeout");
        const timeout = setTimeout(showScreensaver, 60000);
        setScreensaverTimeout(timeout);
    }

    const appTouched = (e) => {
        if (e.target.id !== "screensaver") {
            console.log("clicked");
            startTimeout();
        }
    }

    return (
        <div onClick={appTouched}>
            {screensaverShown &&
                <Screensaver hideScreensaver={hideScreensaver} />
            }
            <NavMenu />
            <Container tag="main">
                {props.children}
            </Container>
        </div>
    );
}

export default Layout;