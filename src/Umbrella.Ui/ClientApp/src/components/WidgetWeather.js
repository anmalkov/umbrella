import React, { useEffect, useState } from 'react';
import { Row, Col, Input, CardBody, CardTitle, UncontrolledAlert } from 'reactstrap';
import { useMutation } from 'react-query';


const WidgetWeather = ({ entity, state }) => {

    const error = null;

    console.log(entity);
    console.log(state);
    
    return (
        <CardBody>
            {error ? <UncontrolledAlert color="danger">{error.message}</UncontrolledAlert> : null}
            <CardTitle>
                <Row>
                    <Col><h4>{entity.name}</h4></Col>
                </Row>
            </CardTitle>
            <Row className="mb-2">
                <Col>
                    <p>i</p>
                </Col>
                <Col>
                    <p>{state.condition}</p>
                </Col>
                <Col>
                    <p>{state.temperature} {state.temperatureUnit}</p>
                    <p>feels {state.temperatureFeelsLike}</p>
                </Col>
            </Row>
            <Row className="mb-2">
                <Col>
                    <p>1</p>
                </Col>
                <Col>
                    <p>2</p>
                </Col>
                <Col>
                    <p>3</p>
                </Col>
                <Col>
                    <p>4</p>
                </Col>
                <Col>
                    <p>5</p>
                </Col>
            </Row>
        </CardBody>
    );
}

export default WidgetWeather;