import React, { useEffect, useState } from 'react';
import { Row, Col, Input, CardBody, CardTitle, UncontrolledAlert } from 'reactstrap';
import { useMutation } from 'react-query';
import { setLightState } from '../fetchers/entities';


const WidgetLight = ({ entity, state }) => {

    const connected = (state && state.connected) || false;

    const [brightness, setBrightness] = useState(0);
    const [turnedOn, setTurnedOn] = useState(false);
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

    useEffect(() => {
        setBrightness(state ? state.brightness : 0);
        setTurnedOn(state ? state.turnedOn : false);
    }, [state]);

    const handleTurnedOnChange = (e) => {
        var turnOn = e.target.checked;
        setTurnedOn(turnOn);
        setState(turnOn, null, null);
    }

    const handleBrightnessChange = (e) => {
        var newBrightness = e.target.value;
        setBrightness(newBrightness);

        if (timer) {
            clearTimeout(timer);
            setTimer(null);
        }
        setTimer(setTimeout(() => {
            setState(null, newBrightness, null);
        }, 100));
    }

    return (
        <CardBody>
            {error ? <UncontrolledAlert color="danger">{error.message}</UncontrolledAlert> : null}
            <CardTitle>
                <Row>
                    <Col><h4 className={!connected ? 'disabled' : ''}>{entity.name}</h4></Col>
                    <Col><div className="form-check form-switch form-check-reverse h4"><Input type="switch" role="switch" checked={turnedOn} disabled={!connected} onChange={handleTurnedOnChange} /></div></Col>
                </Row>
            </CardTitle>
            <Input type="range" min="0" max="100" value={brightness} disabled={!connected || !turnedOn} onChange={handleBrightnessChange} />
        </CardBody>
    );
}

export default WidgetLight;