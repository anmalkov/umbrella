import React from 'react';
import { Row, Col, CardBody, CardTitle } from 'reactstrap';

const WidgetTemperature = ({ entity, state }) => {

    const connected = (state && state.connected) || false;

    return (
        <CardBody>
            <CardTitle>
                <Row>
                    <Col><h4 className={!connected ? 'disabled' : ''}>{entity.name}</h4></Col>
                    <Col className="text-end"><h4>{state.temperature} &deg;C</h4></Col>
                </Row>
            </CardTitle>
            <div className="text-end fs-7">
                Humidity: {state.humidity} %<br />
                Battery: {state.batteryLevel} %
            </div>
        </CardBody>
    );
}

export default WidgetTemperature;