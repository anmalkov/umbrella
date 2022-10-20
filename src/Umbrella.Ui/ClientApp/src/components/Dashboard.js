import React, { useEffect, useState } from 'react';
import { Row } from 'reactstrap';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { useQueryClient } from 'react-query';
import Widget from './Widget';

const Dashboard = () => {

    const widgets = [
        { id: 'w1', col: 1, position: 1, target: { type: 'entity', id: 'light.hue.test' } },
        { id: 'w2', col: 2, position: 2, target: { type: 'group', id: 'group.kitchen' } },
        { id: 'w3', col: 2, position: 1, target: { type: 'entity', id: 'hue.light.test1' } },
        { id: 'w4', col: 3, position: 1, target: { type: 'area', id: 'area.table' } },
        { id: 'w5', col: 4, position: 1, target: { type: 'area', id: 'area.tv' } },
    ];

    const [widgetsList, setWidgetsList] = useState(widgets);
    const [connection, setConnection] = useState(null);

    const queryClient = useQueryClient();

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

    if (widgetsList.length === 0) {
        return (
            <div>
                <p>There are no widgets</p>
            </div>
        );
    }

    return (
        <Row>
            {[1, 2, 3, 4].map(col => (
                <div key={col} className="col-lg-3 col-md-4 col-sm-6 col-xs-12">
                    {widgetsList.filter(w => w.col === col).sort((a, b) => a.position > b.position ? 1 : -1).map(w => (
                        <Widget key={w.id} target={w.target} />
                     ))}
                </div>
            ))}
        </Row>
    );
}

export default Dashboard;
