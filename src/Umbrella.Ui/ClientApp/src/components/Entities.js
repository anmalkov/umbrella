import React, { useState } from 'react';
import { Table } from 'reactstrap';

const Entities = () => {

    const entities = [
        { id: 'hue.light.test1', name: 'Test 1', owner: 'hue' },
        { id: 'hue.light.test2', name: 'Test 2', owner: 'hue' },
        { id: 'hue.light.test3', name: 'Test 3', owner: 'hue' },
        { id: 'hue.light.test4', name: 'Test 4', owner: 'hue' },
        { id: 'hue.light.test5', name: 'Test 5', owner: 'hue' },
    ];

    const [entitiesList, setEntitiesList] = useState(entities);

    if (entitiesList.length === 0) {
        return (
            <div>
                <p>There are no entities registered yet</p>
            </div>
        );
    }

    return (
        <div>
            <Table dark hover>
                <thead>
                    <tr>
                        <th scope="col"></th>
                        <th scope="col">Name</th>
                        <th scope="col">ID</th>
                        <th scope="col">Owner</th>
                    </tr>
                </thead>
                <tbody>
                    { entitiesList.map(entity => (
                        <tr key={entity.id}>
                            <td></td>
                            <td>{entity.name}</td>
                            <td>{entity.id}</td>
                            <td>{entity.owner}</td>
                        </tr>
                    ))}
                </tbody>
            </Table>
        </div>
    );
}

export default Entities;
