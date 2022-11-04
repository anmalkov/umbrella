import React from 'react';
import { Card, CardBody, CardText, CardFooter, Spinner, Alert, Button } from 'reactstrap';
import { useQuery } from 'react-query';
import { fetchEntities } from '../fetchers/entities';
import { fetchGroups } from '../fetchers/groups';
import { fetchAreas } from '../fetchers/areas';
import { fetchStates } from '../fetchers/states';
import WidgetLight from './WidgetLight';
import WidgetEntities from './WidgetEntities';
import WidgetWeather from './WidgetWeather';

const Widget = ({ widget, isEdit, deleteHandler, editHandler }) => {

    const entitiesQuery = useQuery(['entities'], fetchEntities, { staleTime: 60000 });
    const entitiesList = entitiesQuery.data;

    const statesQuery = useQuery(['states'], fetchStates);
    const statesList = statesQuery.data

    const groupsQuery = useQuery(['groups'], fetchGroups, { staleTime: 60000 });
    const groupsList = groupsQuery.data;

    const areasQuery = useQuery(['areas'], fetchAreas, { staleTime: 60000 });
    const areasList = areasQuery.data


    const isError = entitiesQuery.isError || statesQuery.isError || groupsQuery.isError || areasQuery.isError;
    const isLoading = entitiesQuery.isLoading || statesQuery.isLoading || groupsQuery.isLoading || areasQuery.isLoading;

    const getWidget = () => {
        if (isLoading) {
            return;
        }
        switch (widget.type) {
            default:
            case 'entity':
                if (!entitiesList || !statesList) {
                    return;
                }
                const entityId = widget.parameters.find(p => p.key = 'id').value;
                const entity = entitiesList.find(e => e.id === entityId);
                if (!entity) {
                    return (
                        <CardBody>
                            <CardText>Entity {entityId} not found</CardText>
                        </CardBody>
                    );
                }
                const state = (statesList && statesList.find(s => s.id === entity.id).state) || {};
                switch (entity.type) {
                    default:
                    case 'light':
                        return <WidgetLight entity={entity} state={state} />;
                }
            case 'group':
                if (!groupsList || !entitiesList || !statesList) {
                    return;
                }
                const groupId = widget.parameters.find(p => p.key = 'id').value;
                const group = groupsList.find(e => e.id === groupId);
                if (!group) {
                    return (
                        <CardBody>
                            <CardText>Group {groupId} not found</CardText>
                        </CardBody>
                    );
                }
                const groupEntities = group.entities.map(id => entitiesList.find(e => e.id === id));
                const groupStates = group.entities.map(id => (statesList && statesList.find(s => s.id === id)) || {});
                return <WidgetEntities name={group.name} entities={groupEntities} states={groupStates} />;
            case 'area':
                if (!areasList || !entitiesList || !statesList) {
                    return;
                }
                const areaId = widget.parameters.find(p => p.key = 'id').value;
                const area = areasList.find(e => e.id === areaId);
                if (!area) {
                    return (
                        <CardBody>
                            <CardText>Area {areaId} not found</CardText>
                        </CardBody>
                    );
                }
                const areaEntities = entitiesList.filter(e => e.areaId === area.id);
                const areaStates = areaEntities.map(e => (statesList && statesList.find(s => s.id === e.id)) || {});
                return <WidgetEntities name={area.name} entities={areaEntities} states={areaStates} />;
            case 'weather':
                return <WidgetWeather />;
        }
    }

    if (isLoading) {
        return (
            <Card className="widget">
                <CardBody>
                    <Spinner color="light">
                        Loading...
                    </Spinner>
                </CardBody>
            </Card>
        );
    }

    if (isError) {
        return (
            <Card className="widget">
                <CardBody>
                    <Alert color="danger">
                        {entitiesQuery.error ? entitiesQuery.error.message
                            : statesQuery.error ? statesQuery.error.message
                                : groupsQuery.error ? groupsQuery.error.message
                                    : areasQuery.error ? areasQuery.error.message
                                        : 'Unknown error'}
                    </Alert>
                </CardBody>
            </Card>
        );
    }

    return (
        <Card className="widget">
            {getWidget()}
            {isEdit ? (
                <CardFooter>
                    <Button onClick={() => editHandler(widget)}>Edit</Button>
                    <Button color="danger" onClick={() => deleteHandler(widget.id)}>Delete</Button>
                </CardFooter>
            ): null}
        </Card>
    );
}

export default Widget;