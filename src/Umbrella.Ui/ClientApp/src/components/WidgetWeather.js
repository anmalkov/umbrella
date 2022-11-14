import React, { useEffect, useState } from 'react';
import { Row, Col, Input, CardBody, CardTitle, UncontrolledAlert } from 'reactstrap';
import { useMutation } from 'react-query';
import WeatherIcon from './WeatherIcon';
import './WidgetWeather.css'

const WidgetWeather = ({ entity, state }) => {

    const error = null;
    const weekDays = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];

    const currentWeatherHour = (new Date(state.updatedAt)).getHours();
    const isCurrentWeatherNight = state.sunrise && state.sunset
        ? !(currentWeatherHour > (new Date(state.sunrise)).getHours() && currentWeatherHour < (new Date(state.sunset)).getHours())
        : !(currentWeatherHour > 7 && currentWeatherHour < 20)

    return (
        <CardBody>
            {error ? <UncontrolledAlert color="danger">{error.message}</UncontrolledAlert> : null}
            <CardTitle>
                <Row>
                    <Col><h4>{entity.name}</h4></Col>
                    <Col className="text-end"><h4>{state.temperature.toFixed(0)} {state.temperatureUnit}</h4></Col>
                </Row>
            </CardTitle>
            <Row className="mb-2">
                <Col>
                    <WeatherIcon conditionCode={state.conditionCode} isNight={isCurrentWeatherNight} />
                </Col>
                <Col>
                    <h5>{state.condition}</h5>
                </Col>
                <Col className="text-end fs-7">
                    Feels {state.temperatureFeelsLike.toFixed(0)} {state.temperatureUnit}<br/>
                    Wind {state.windSpeed.toFixed(0)} {state.windSpeedUnit}<br />
                    {state.precipitationProbability.toFixed(0)} %
                </Col>
            </Row>
            <Row className="row-cols-5">
                {state.dailyForecast.slice(1, 6).map(forecast => (
                    <Col key={forecast.date} className="text-center">
                        <p className="text-center">
                            {weekDays[(new Date(forecast.date)).getDay()]}<br />
                            <WeatherIcon conditionCode={forecast.conditionCode} isNight={forecast.isNight} className="weather-small" /><br />
                            {forecast.temperatureDay.toFixed(0)}<br />
                            {forecast.temperatureNight.toFixed(0)}
                        </p>
                    </Col>
                ))}
            </Row>
        </CardBody>
    );
}

export default WidgetWeather;