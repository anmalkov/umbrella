import React, { useEffect, useState } from 'react';
import { Row, Col, Input, CardBody, CardTitle, UncontrolledAlert } from 'reactstrap';
import { useMutation } from 'react-query';


const WidgetWeather = ({ city }) => {

    const error = null;
    
    return (
        <CardBody>
            {error ? <UncontrolledAlert color="danger">{error.message}</UncontrolledAlert> : null}
            <CardTitle>
                <Row>
                    <Col><h4>Weather</h4></Col>
                </Row>
            </CardTitle>
        </CardBody>
    );
}

export default WidgetWeather;