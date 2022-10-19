import React from 'react';
import { Row, Col, Input, CardBody, CardTitle } from 'reactstrap';

const WidgetEntities = ({ name, entities }) => {

    return (
        <CardBody>
            <CardTitle>
                <Row>
                    <Col><h4>{name}</h4></Col>
                    <Col><div className="form-check form-switch form-check-reverse h4"><Input type="switch" role="switch" /></div></Col>
                </Row>
            </CardTitle>
            <Row className="mb-2">
                <Input type="range" min="0" max="50" />
            </Row>
            {entities.sort((a, b) => a.name > b.name ? 1 : -1).map(e => (
                <Row key={e.id} className="mb-2">
                    <Col className="pt-1">{e.name}</Col>
                    <Col><div className="form-check form-switch form-check-reverse h4"><Input type="switch" role="switch" /></div></Col>
                </Row>
            ))}
        </CardBody>
    );
}

export default WidgetEntities;