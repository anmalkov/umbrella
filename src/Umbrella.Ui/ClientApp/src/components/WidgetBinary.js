import React from 'react';
import { Row, Col, CardBody, CardTitle } from 'reactstrap';

const WidgetBinary = ({ entity, state }) => {

    const connected = (state && state.connected) || false;

    const getStateText = () => {
        switch (entity.fullObject.binaryType) {
            case 1: return state.isOn === true ? 'Open' : 'Close';
            case 2: return state.isOn === true ? 'Motion' : 'Clear';
            default: return '';
        }
    }

    const getStateColor = () => {
        switch (entity.fullObject.binaryType) {
            case 1: return state.isOn === true ? 'red' : 'green';
            case 2: return state.isOn === true ? 'red' : 'green';
            default: return '';
        }
    }

    return (
        <CardBody>
            <CardTitle>
                <h4 className={!connected ? 'disabled' : ''}>{entity.name}</h4>
            </CardTitle>
            <div className="text-end"><h4 style={{ color: getStateColor() }}>{getStateText()}</h4></div>
            <div className="text-end fs-7">Battery: {state.batteryLevel} %</div>
        </CardBody>
    );
}

export default WidgetBinary;