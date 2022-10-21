import React, { useEffect, useState } from 'react';
import { Row, Col, Input, CardBody, CardTitle, UncontrolledAlert } from 'reactstrap';
import { useMutation } from 'react-query';
import { setLightsStates, setLightState } from '../fetchers/entities';

const WidgetEntities = ({ name, entities, states }) => {

    const containsLights = entities.filter(e => e.type === 'light').length > 0;

    const [anyConnected, setAnyConnected] = useState(false);
    const [anyTurnedOn, setAnyTurnedOn] = useState(false);
    const [brightness, setBrightness] = useState(0);
    const [error, setError] = useState(null);
    const [timer, setTimer] = useState(null);

    useEffect(() => {
        if (!containsLights || !states || states.length === 0) {
            return;
        }
        setAnyConnected(states.filter(s => s.state && s.state.connected).length > 0);
        setAnyTurnedOn(states.filter(s => s.state && s.state.turnedOn).length > 0);
        var maxBrightness = Math.max(...states.filter(s => s.state.connected && s.state.turnedOn).map(s => s.state.brightness)) || 0;
        setBrightness(maxBrightness);
    }, [states, containsLights]);

    const setStateMutation = useMutation(({ id, turnedOn, brightness, colorTemperature }) => {
        return setLightState(id, turnedOn, brightness, colorTemperature);
    });

    const setState = async (id, turnedOn, brightness, colorTemperature) => {
        try {
            setError(null);
            await setStateMutation.mutateAsync({ id, turnedOn, brightness, colorTemperature });
        }
        catch (e) {
            setError(e);
        }
    }

    const handleAllTurnedOnChange = (e) => {
        const turnOn = e.target.checked;
        //states.filter(s => s.state.connected && s.state.turnedOn === !turnOn).forEach(s => setState(s.id, turnOn, null, null));
        var props = states.filter(s => s.state.connected && s.state.turnedOn === !turnOn)
            .map(s => ({ id: s.id, turnedOn: turnOn }));
        console.log(props);
        setLightsStates(props);
    }

    const handleTurnedOnChange = (id, e) => {
        setState(id, e.target.checked, null, null);
    }

    const handleAllBrightnessChange = (event) => {
        var newBrightness = event.target.value;
        setBrightness(newBrightness);
        if (timer) {
            clearTimeout(timer);
            setTimer(null);
        }
        setTimer(setTimeout(() => {
            states.filter(s => s.state.connected && s.state.turnedOn).forEach(s => setState(s.id, null, newBrightness, null));
        }, 100));
    }    

    return (
        <CardBody>
            {error && <UncontrolledAlert color="danger">{error.message}</UncontrolledAlert>}
            <CardTitle>
                <Row>
                    <Col><h4 className={!anyConnected ? 'disabled' : ''}>{name}</h4></Col>
                    <Col>
                        {containsLights &&
                            <div className="form-check form-switch form-check-reverse h4"><Input type="switch" role="switch" checked={anyTurnedOn} onChange={handleAllTurnedOnChange} disabled={!anyConnected} /></div>
                        }
                    </Col>
                </Row>
            </CardTitle>
            {containsLights &&
                <Row className="mb-2">
                    <Input type="range" min="0" max="100" value={brightness} disabled={!anyConnected || !anyTurnedOn} onChange={handleAllBrightnessChange} />
                </Row>
            }
            {entities.sort((a, b) => a.name > b.name ? 1 : -1).map(e => {
                const state = states.find(s => s.id === e.id).state;
                const turnedOn = (state && state.turnedOn) || false;
                const connected = (state && state.connected) || false;
                return (
                    <Row key={e.id} className="mb-2">
                        <Col className={`pt-1 ${!connected ? 'disabled' : ''}`}>{e.name}</Col>
                        <Col>
                            {e.type === "light" &&
                                <div className="form-check form-switch form-check-reverse h4"><Input type="switch" role="switch" checked={turnedOn} disabled={!connected} onChange={(event) => handleTurnedOnChange(e.id, event)} /></div>
                            }
                        </Col>
                    </Row>
                );
            })}
        </CardBody>
    );
}

export default WidgetEntities;