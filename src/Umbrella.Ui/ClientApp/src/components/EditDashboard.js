import React, { useState } from 'react';
import { Row, Col, Spinner, Button, Input, Form, FormGroup, Label } from 'reactstrap';
import Widget from './Widget';
import EditWidget, { GetNewWidget } from './EditWidget';

const GetNewDashboard = () => { return { id: null, name: '', widgets: [] } };

const EditDashboard = ({ oldDashboard, saveHandler, cancelHandler }) => {

    const [dashboard, setDashboard] = useState({ ...oldDashboard });
    const [currentWidget, setCurrentWidget] = useState(null);

    const addWidgetHandler = () => {
        setCurrentWidget(GetNewWidget());
    }

    const saveWidgetHandler = (widget) => {
        if (widget.id === 0) {
            setDashboard(old => {
                return { ...old, widgets: [...old.widgets, widget] }
            });
        } else {
            setDashboard(old => {
                return { ...old, widgets: old.widgets.map(w => w.id === widget.id ? widget : w) }
            });
        }
        setCurrentWidget(null);
    }

    const cancelWidgetHandler = () => {
        setCurrentWidget(null);
    }

    const nameChangeHandler = (e) => {
        setDashboard(old => {
            return { ...old, name: e.target.value };
        });
    }

    return (
        <div>
            <Row>
                <Col>
                    <Button onClick={() => saveHandler(dashboard)}>Save</Button>
                    <Button onClick={cancelHandler}>Cancel</Button>
                </Col>
            </Row>
            <Row>
                <Input type="text" value={dashboard.name} onChange={nameChangeHandler} />
            </Row>
            {currentWidget ? (
                <EditWidget oldWidget={currentWidget} saveHandler={saveWidgetHandler} cancelHandler={cancelWidgetHandler} />
            ) : (
                <Row>
                    <Button onClick={addWidgetHandler}>Add widget</Button>
                </Row >
            )}
            <Row>
                {[1, 2, 3, 4].map(col => (
                    <Col key={col} className="col-lg-3 col-md-4 col-sm-6 col-xs-12">
                        {dashboard.widgets.filter(w => w.column === col).sort((a, b) => a.positionInColumn > b.positionInColumn ? 1 : -1).map(w => (
                            <Widget key={w.id} widget={w} />
                        ))}
                    </Col>
                ))}
            </Row>
        </div>
    );
}

export default EditDashboard;
export { GetNewDashboard };
