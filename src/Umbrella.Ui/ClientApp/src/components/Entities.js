import React from 'react';
import { Table, Spinner, Alert } from 'reactstrap';
import { useQuery } from 'react-query';
import { fetchEntities } from '../fetchers/entities';

const Entities = () => {

    const { isError, isLoading, data, error } = useQuery(['entities'], fetchEntities, { staleTime: 60000 });
    const entitiesList = data

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
            { isError &&
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
