import React, { useEffect, useState } from 'react';
import { Row, Col, Spinner, Button, Input, Form, FormGroup, Label } from 'reactstrap';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { useQueryClient, useQuery, useMutation } from 'react-query';
import { fetchDashboards, createDashboard, updateDashboard } from '../fetchers/dashboards';
import ToolBar from './ToolBar';
import Widget from './Widget';

const Dashboard = () => {

    const { isError, isLoading, data, error } = useQuery(['dashboards'], fetchDashboards, { staleTime: 60000 });
    const dashboardsList = data

    const [currentDashboard, setCurrentDashboard] = useState(null);
    const [connection, setConnection] = useState(null);
    const [isEdit, setIsEdit] = useState(false);
    const [name, setName] = useState('New dashboard');
    const [currentWidget, setCurrentWidget] = useState(null);
    const [isEditWidget, setIsEditWidget] = useState(false);

    const queryClient = useQueryClient();

    const createMutation = useMutation(({ name, widgets }) => {
        return createDashboard(name, widgets);
    });

    const updateMutation = useMutation(({ id, name, widgets }) => {
        console.log('update');
        return updateDashboard(id, name, widgets);
    });

    useEffect(() => {
        const newConnection = new HubConnectionBuilder()
            .withUrl("/sr/stateHub")
            .withAutomaticReconnect()
            .build();
        setConnection(newConnection);
    }, []);

    useEffect(() => {
        const connect = async () => {
            if (connection) {
                try {
                    await connection.start();
                    console.log('Connected!');
                    connection.on('ReceiveStateUpdate', (id, state) => {
                        console.log('ReceiveStateUpdate', id, state);
                        queryClient.invalidateQueries(['states']);
                        queryClient.refetchQueries('states', { force: true });
                    });
                }
                catch (e) {
                    console.log('Connection failed: ', e);
                }
            }
        }
        connect();
    }, [connection]);        

    const addHandler = () => {
        setName('New dashboard');
        setCurrentDashboard({ id: null, name: name, widgets: [] });
        setIsEdit(true);
        setCurrentWidget(null);
        setIsEditWidget(false);
    }

    const editHandler = () => {
        setName(currentDashboard.name);
        setIsEdit(true);
        setCurrentWidget(null);
        setIsEditWidget(false);
    }

    const saveHandler = async () => {
        try {
            if (!currentDashboard.id) {
                await createMutation.mutateAsync({ name: name, widgets: [] });
            } else {
                await updateMutation.mutateAsync({ id: currentDashboard.id, name: name, widgets: [] });
            }
            queryClient.invalidateQueries(['dashboards']);
            queryClient.refetchQueries('dashboards', { force: true });
        }
        catch { }
        setCurrentDashboard(null);
        setIsEdit(false);
    }

    const cancelHandler = () => {
        setCurrentDashboard(null);
        setIsEdit(false);
    }

    const addWidgetHandler = () => {
        setCurrentWidget({});
        setIsEditWidget(true);
    }

    const cancelWidgetHandler = () => {
        setCurrentWidget(null);
        setIsEditWidget(false);
    }

    const selectDashboardHandler = id => {
        const dashboard = dashboardsList.find(d => d.id === id);
        if (dashboard) {
            setCurrentDashboard(dashboard);
        }
    }

    if (isLoading) {
        return (
            <div className="text-center">
                <Spinner color="light">
                    Loading...
                </Spinner>
            </div>
        );
    }

    console.log(dashboardsList.length);

    if (dashboardsList.length === 0 && !currentDashboard) {
        return (
            <ToolBar name="No dashboards yet" addHandler={addHandler} />
        )
    }

    if (currentDashboard && isEdit) {
        return (
            <div>
                <Row>
                    <Col>
                        <Button onClick={saveHandler}>Save</Button>
                        <Button onClick={cancelHandler}>Cancel</Button>
                    </Col>
                </Row>
                <Row>
                    <Input type="text" value={name} onChange={e => setName(e.target.value)} />
                </Row>
                {currentWidget && isEditWidget ? (
                    <Form>
                        <FormGroup>
                            <Col>
                                <Button onClick={addWidgetHandler}>Save</Button>
                                <Button onClick={cancelWidgetHandler}>Cancel</Button>
                            </Col>
                        </FormGroup>
                        <FormGroup>
                            <Label for="exampleEmail">Column</Label>
                            <Input type="select" value="1">
                                <option>1</option>
                                <option>2</option>
                                <option>3</option>
                                <option>4</option>
                            </Input>
                        </FormGroup>
                        <FormGroup>
                            <Label for="exampleEmail">Position in column</Label>
                            <Input type="text" value="1" />
                        </FormGroup>
                        <FormGroup>
                            <Label for="exampleEmail">Type</Label>
                            <Input type="select">
                                <option>Entity</option>
                                <option>Area</option>
                                <option>Group</option>
                            </Input>
                        </FormGroup>
                        <FormGroup>
                            <Label for="exampleEmail">Parameters</Label>
                            <Input type="text" value="Parameters" />
                        </FormGroup>
                    </Form>
                ) : (
                    <Row>
                        <Button onClick={addWidgetHandler}>Add widget</Button>
                    </Row >
                )}
                <Row>
                    {[1, 2, 3, 4].map(col => (
                        <Col key={col} className="col-lg-3 col-md-4 col-sm-6 col-xs-12">
                            {currentDashboard.widgets.filter(w => w.column === col).sort((a, b) => a.positionInColumn > b.positionInColumn ? 1 : -1).map(w => (
                                <Widget key={w.id} target={w.target} />
                            ))}
                        </Col>
                    ))}
                </Row>
            </div>
        );
    }

    return (
        <div>
            <ToolBar dashboards={dashboardsList} addHandler={addHandler} selectHandler={selectDashboardHandler} />
            {currentDashboard ? (
                <Row>
                    <h1>{currentDashboard.name}</h1><Button onClick={editHandler}>Edit</Button>
                    {[1, 2, 3, 4].map(col => (
                        <div key={col} className="col-lg-3 col-md-4 col-sm-6 col-xs-12">
                            {isEdit ? (
                                <Button>+</Button>
                            ) : null}
                            {currentDashboard.widgets.filter(w => w.column === col).sort((a, b) => a.positionInColumn > b.positionInColumn ? 1 : -1).map(w => (
                                <Widget key={w.id} target={w.target} />
                            ))}
                        </div>
                    ))}
                </Row>
            ) : null}
        </div>
    );
}

export default Dashboard;
