import React from 'react';
import { Row, Col, Input, CardBody, CardTitle } from 'reactstrap';

const WidgetEntities = ({ name, entities, states }) => {

    const containsLights = entities.filter(e => e.type === 'light').length > 0;
    const anyConnected = containsLights && states && states.length > 0 && states.filter(s => s.state && s.state.connected).length > 0;
    const anyTurnedOn = containsLights && states && states.length > 0 && states.filter(s => s.state && s.state.turnedOn).length > 0;
    const maxBrightness = containsLights && states && states.length > 0 && Math.max(...states.filter(s => s.state.connected && s.state.turnedOn).map(s => s.state.brightness)) || 0;

    return (
        <CardBody>
            <CardTitle>
                <Row>
                    <Col><h4 className={!anyConnected ? 'disabled' : ''}>{name}</h4></Col>
                    <Col>
                        {containsLights &&
                            <div className="form-check form-switch form-check-reverse h4"><Input type="switch" role="switch" checked={anyTurnedOn} disabled={!anyConnected} /></div>
                        }
                    </Col>
                </Row>
            </CardTitle>
            {containsLights &&
                <Row className="mb-2">
                    <Input type="range" min="0" max="100" value={maxBrightness} disabled={!anyConnected} />
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
                                <div className="form-check form-switch form-check-reverse h4"><Input type="switch" role="switch" checked={turnedOn} disabled={!connected} /></div>
                            }
                        </Col>
                    </Row>
                );
            })}
        </CardBody>
    );
}

export default WidgetEntities;