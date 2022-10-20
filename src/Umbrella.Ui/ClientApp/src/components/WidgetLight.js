import React from 'react';
import { Row, Col, Input, CardBody, CardTitle } from 'reactstrap';

const WidgetLight = ({ entity, state }) => {
    const { name } = entity;
    const turnedOn = (state && state.turnedOn) || false;
    const brightness = (state && state.brightness) || 0;
    return (
        <CardBody>
            <CardTitle>
                <Row>
                    <Col><h4>{name}</h4></Col>
                    <Col><div className="form-check form-switch form-check-reverse h4"><Input type="switch" role="switch" checked={turnedOn} /></div></Col>
                </Row>
            </CardTitle>
            <Input type="range" min="0" max="100" value={brightness} />
        </CardBody>
    );
}

export default WidgetLight;