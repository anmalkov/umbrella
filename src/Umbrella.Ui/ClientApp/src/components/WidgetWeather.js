import React, { useEffect, useState } from 'react';
import { Row, Col, Input, CardBody, CardTitle, UncontrolledAlert } from 'reactstrap';
import { useMutation } from 'react-query';


const WidgetWeather = ({ entity, state }) => {

    const error = null;
    const weekDays = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];

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
                    <p></p>
                </Col>
                <Col>
                    <p>{state.condition}</p>
                </Col>
                <Col>
                    <p><b>{state.temperature.toFixed(0)} {state.temperatureUnit}</b><br />feels {state.temperatureFeelsLike.toFixed(0)}</p>
                </Col>
            </Row>
            <Row>
                {state.dailyForecast.slice(1, 6).map(forecast => (
                    <Col key={forecast.date}>
                        <p>{weekDays[(new Date(forecast.date)).getDay()]}<br />{forecast.temperatureDay.toFixed(0)}<br />{forecast.temperatureNight.toFixed(0)}</p>
                    </Col>

                ))}
            </Row>
        </CardBody>
    );
}

export default WidgetWeather;