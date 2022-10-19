import React from 'react';
import { Table, Spinner, Alert } from 'reactstrap';
import { useQuery } from 'react-query';
import { fetchGroups } from '../fetchers/groups';

const Groups = () => {

    const { isError, isLoading, data, error } = useQuery(['groups'], fetchGroups, { staleTime: 60000 });
    const groupsList = data

    if (isLoading) {
        return (
            <div className="text-center">
                <Spinner color="light">
                    Loading...
                </Spinner>
            </div>
        );
    }    

    if (groupsList.length === 0) {
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
                    </tr>
                </thead>
                <tbody>
                    {groupsList.sort((a, b) => a.name > b.name ? 1 : -1).map(group => (
                        <tr key={group.id}>
                            <td></td>
                            <td>{group.name}</td>
                            <td>{group.id}</td>
                        </tr>
                    ))}
                </tbody>
            </Table>
        </div>
    );
}

export default Groups;
