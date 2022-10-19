import React from 'react';
import { Row, Col, Input, CardBody, CardTitle } from 'reactstrap';

const WidgetLight = ({ entity }) => {
    const { name } = entity;
    return (
        <CardBody>
            <CardTitle>
                <Row>
                    <Col><h4>{name}</h4></Col>
                    <Col><div className="form-check form-switch form-check-reverse h4"><Input type="switch" role="switch" /></div></Col>
                </Row>
            </CardTitle>
            <Input type="range" min="0" max="50" />
        </CardBody>
    );
}

export default WidgetLight;