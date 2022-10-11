import React, { useState, useEffect } from 'react';
import { Table, Spinner, Alert } from 'reactstrap';

const Entities = () => {

    const [entitiesList, setEntitiesList] = useState([]);
    const [error, setError] = useState(null);
    const [isLoading, setIsLoading] = useState(true);

    const getEntities = () => {
        fetch('api/entities')
            .then(response => response.json())
            .then(
                result => {
                    setIsLoading(false);
                    setEntitiesList(result);
                },
                error => {
                    setIsLoading(false);
                    setError(error)
                }
            )
    }

    useEffect(() => {
        getEntities();
    }, []);

    if (isLoading) {
        return (
            <div className="text-center">
                <Spinner color="light">
                    Loading...
                </Spinner>
            </div>
        );
    }    

    if (entitiesList.length === 0) {
        return (
            <div>
                <p>There are no entities registered yet</p>
            </div>
        );
    }

    return (
        <div>
            { error &&
                <Alert color="danger">
                    {error.message}
                </Alert>
            }
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
                    {entitiesList.sort((a, b) => a.name > b.name ? 1 : -1).map(entity => (
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
