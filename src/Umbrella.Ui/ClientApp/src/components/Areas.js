import React from 'react';
import { Table, Spinner, Alert } from 'reactstrap';
import { useQuery } from 'react-query';
import { fetchAreas } from '../fetchers/areas';

const Areas = () => {

    const { isError, isLoading, data, error } = useQuery(['areas'], fetchAreas, { staleTime: 60000 });
    const areasList = data

    if (isLoading) {
        return (
            <div className="text-center">
                <Spinner color="light">
                    Loading...
                </Spinner>
            </div>
        );
    }    

    if (areasList.length === 0) {
        return (
            <div>
                <p>There are no areas yet</p>
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
                    </tr>
                </thead>
                <tbody>
                    {areasList.sort((a, b) => a.name > b.name ? 1 : -1).map(area => (
                        <tr key={area.id}>
                            <td></td>
                            <td>{area.name}</td>
                            <td>{area.id}</td>
                        </tr>
                    ))}
                </tbody>
            </Table>
        </div>
    );
}

export default Areas;
