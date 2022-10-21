import React, { useState } from 'react';
import { Row, Col, Input, CardBody, CardTitle, UncontrolledAlert } from 'reactstrap';
import { useMutation } from 'react-query';
import { setLightState } from '../fetchers/entities';


const WidgetLight = ({ entity, state }) => {

    const turnedOn = (state && state.turnedOn) || false;
    const connected = (state && state.connected) || false;
    const brightness = (state && state.brightness) || 0;

    const [error, setError] = useState(null);
    const [timer, setTimer] = useState(null);

    const setStateMutation = useMutation(({ id, turnedOn, brightness, colorTemperature }) => {
        return setLightState(id, turnedOn, brightness, colorTemperature);
    });

    const setState = async (turnedOn, brightness, colorTemperature) => {
        try {
            setError(null);
            await setStateMutation.mutateAsync({ id: entity.id, turnedOn, brightness, colorTemperature });
        }
        catch (e) {
            setError(e);
        }
    }

    const handleTurnedOnChange = (e) => {
        setState(e.target.checked, null, null);
    }

    const handleBrightnessChange = (e) => {
        if (timer) {
            clearTimeout(timer);
            setTimer(null);
        }
        setTimer(setTimeout(() => {
            setState(null, e.target.value, null);
        }, 100));
    }

    console.log("redraw light widget");
    
    return (
        <CardBody>
            {error && <UncontrolledAlert color="danger">{error.message}</UncontrolledAlert>}
            <CardTitle>
                <Row>
                    <Col><h4 className={!connected ? 'disabled' : ''}>{entity.name}</h4></Col>
                    <Col><div className="form-check form-switch form-check-reverse h4"><Input type="switch" role="switch" defaultChecked={turnedOn} disabled={!connected} onChange={handleTurnedOnChange} /></div></Col>
                </Row>
            </CardTitle>
            <Input type="range" min="0" max="100" defaultValue={brightness} disabled={!connected || !turnedOn} onChange={handleBrightnessChange} />
        </CardBody>
    );
}

export default WidgetLight;