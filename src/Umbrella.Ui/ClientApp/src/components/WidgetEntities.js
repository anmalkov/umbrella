import React from 'react';
import { Row, Col, Input, CardBody, CardTitle } from 'reactstrap';

const WidgetEntities = ({ name, entities, states }) => {

    const containsLights = entities.filter(e => e.type === 'light').length > 0;
    const anyTurnedOn = containsLights && states && states.length > 0 && states.filter(s => s.state && s.state.turnedOn).length > 0;
    const maxBrightness = containsLights && states && states.length > 0 && Math.max(...states.map(s => s.state.brightness)) || 0;
    console.log(states);
    console.log(maxBrightness);

    return (
        <CardBody>
            <CardTitle>
                <Row>
                    <Col><h4>{name}</h4></Col>
                    <Col>
                        {containsLights &&
                            <div className="form-check form-switch form-check-reverse h4"><Input type="switch" role="switch" defaultChecked={anyTurnedOn} /></div>
                        }
                    </Col>
                </Row>
            </CardTitle>
            {containsLights &&
                <Row className="mb-2">
                    <Input type="range" min="0" max="100" defaultValue={maxBrightness} />
                </Row>
            }
            {entities.sort((a, b) => a.name > b.name ? 1 : -1).map(e => {
                const state = states.find(s => s.id === e.id).state;
                const turnedOn = (state && state.turnedOn) || false;
                return (
                    <Row key={e.id} className="mb-2">
                        <Col className="pt-1">{e.name}</Col>
                        <Col>
                            {e.type === "light" &&
                                <div className="form-check form-switch form-check-reverse h4"><Input type="switch" role="switch" defaultChecked={turnedOn} /></div>
                            }
                        </Col>
                    </Row>
                );
            })}
        </CardBody>
    );
}

export default WidgetEntities;