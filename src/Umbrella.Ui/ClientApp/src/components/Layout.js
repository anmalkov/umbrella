import React, { useState } from 'react';
import { Container } from 'reactstrap';
import NavMenu from './NavMenu';
import Screensaver from './Screensaver';

const Layout = (props) => {

    const [screensaverShown, setScreensaverShown] = useState(false);
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
        const timeout = setTimeout(showScreensaver, 2000);
        setScreensaverTimeout(timeout);
    }

    const appTouched = (e) => {
        if (e.target.id !== "screensaver") {
            startTimeout();
        }
    }

    return (
        <div onClick={appTouched}>
            {screensaverShown &&
                <Screensaver hideScreensaver={hideScreensaver} />
            }
            <NavMenu />
            <Container tag="main" fluid>
                {props.children}
            </Container>
        </div>
    );
}

export default Layout;