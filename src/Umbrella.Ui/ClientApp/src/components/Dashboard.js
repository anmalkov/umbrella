import React, { useEffect, useState } from 'react';
import { Row, Col, Spinner, Button, Input, Form, FormGroup, Label } from 'reactstrap';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { useQueryClient, useQuery, useMutation } from 'react-query';
import { fetchDashboards, createDashboard, updateDashboard, deleteDashboard } from '../fetchers/dashboards';
import ToolBar from './ToolBar';
import Widget from './Widget';
import EditDashboard, { GetNewDashboard } from './EditDashboard';

const Dashboard = () => {

    const { isError, isLoading, data, error } = useQuery(['dashboards'], fetchDashboards, { staleTime: 60000 });
    const dashboardsList = data

    const [currentDashboard, setCurrentDashboard] = useState(null);
    const [isEdit, setIsEdit] = useState(false);
    const [connection, setConnection] = useState(null);

    const queryClient = useQueryClient();

    const createMutation = useMutation(dashboard => {
        return createDashboard(dashboard);
    });

    const updateMutation = useMutation(dashboard => {
        return updateDashboard(dashboard);
    });

    const deleteMutation = useMutation((id) => {
        return deleteDashboard(id);
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

    useEffect(() => {
        if (!currentDashboard && data && data.length > 0) {
            setCurrentDashboard(data[0])
        }
    }, [data])

    const addHandler = () => {
        setCurrentDashboard(GetNewDashboard());
        setIsEdit(true);
    }

    const editHandler = () => {
        if (currentDashboard) {
            setIsEdit(true);
        }
    }

    const saveHandler = async (dashboard) => {
        try {
            if (!dashboard.id) {
                await createMutation.mutateAsync(dashboard);
            } else {
                await updateMutation.mutateAsync(dashboard);
            }
            queryClient.invalidateQueries(['dashboards']);
            queryClient.refetchQueries('dashboards', { force: true });
        }
        catch { }
        setCurrentDashboard(null);
        setIsEdit(false);
    }

    const deleteHandler = async () => {
        if (!currentDashboard || !window.confirm(`Do you want to delete dashboard '${currentDashboard.name}' ?`)) {
            return;
        }
        try {
            await deleteMutation.mutateAsync(currentDashboard.id);
            queryClient.invalidateQueries(['dashboards']);
            queryClient.refetchQueries('dashboards', { force: true });
        }
        catch { }
        setCurrentDashboard(null);
    }

    const cancelHandler = () => {
        setCurrentDashboard(null);
        setIsEdit(false);
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

    if ((!dashboardsList || dashboardsList.length === 0) && !currentDashboard) {
        return (
            <ToolBar name="No dashboards yet" addHandler={addHandler} />
        )
    }

    if (currentDashboard && isEdit) {
        return (
            <EditDashboard oldDashboard={currentDashboard} saveHandler={saveHandler} cancelHandler={cancelHandler} />
        );
    }

    return (
        <div>
            <ToolBar dashboards={dashboardsList} currentDashboard={currentDashboard} addHandler={addHandler} selectHandler={selectDashboardHandler} editHandler={editHandler} deleteHandler={deleteHandler} />
            {currentDashboard ? (
                <Row>
                    {[1, 2, 3, 4].map(col => (
                        <div key={col} className="col-lg-3 col-md-4 col-sm-6 col-xs-12">
                            {isEdit ? (
                                <Button>+</Button>
                            ) : null}
                            {currentDashboard.widgets.filter(w => w.column === col).sort((a, b) => a.positionInColumn > b.positionInColumn ? 1 : -1).map(w => (
                                <Widget key={w.id} widget={w} />
                            ))}
                        </div>
                    ))}
                </Row>
            ) : null}
        </div>
    );
}

export default Dashboard;
