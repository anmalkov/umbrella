import React from 'react';
import { Card, CardBody, CardText, Spinner, Alert } from 'reactstrap';
import { useQuery } from 'react-query';
import { fetchEntities } from '../fetchers/entities';
import { fetchGroups } from '../fetchers/groups';
import { fetchAreas } from '../fetchers/areas';
import WidgetLight from './WidgetLight';
import WidgetEntities from './WidgetEntities';

const Widget = ({ target }) => {

    const entitiesQuery = useQuery(['entities'], fetchEntities, { staleTime: 60000 });
    const entitiesList = entitiesQuery.data;

    const groupsQuery = useQuery(['groups'], fetchGroups, { staleTime: 60000 });
    const groupsList = groupsQuery.data;

    const areasQuery = useQuery(['areas'], fetchAreas, { staleTime: 60000 });
    const areasList = areasQuery.data
    
    const isError = entitiesQuery.isError || groupsQuery.isError || areasQuery.isError;
    const isLoading = entitiesQuery.isLoading || groupsQuery.isLoading || areasQuery.isLoading;

    const getWidget = () => {
        if (isLoading) {
            return;
        }
        switch (target.type) {
            default:
            case 'entity':
                if (!entitiesList) {
                    return;
                }
                const entity = entitiesList.find(e => e.id === target.id);
                if (!entity) {
                    return (
                        <CardBody>
                            <CardText>Entity {target.id} not found</CardText>
                        </CardBody>
                    );
                }
                switch (entity.type) {
                    default:
                    case 'light':
                        return (<WidgetLight entity={entity} />);
                }
            case 'group':
                if (!groupsList || !entitiesList) {
                    return;
                }
                const group = groupsList.find(e => e.id === target.id);
                if (!group) {
                    return (
                        <CardBody>
                            <CardText>Group {target.id} not found</CardText>
                        </CardBody>
                    );
                }
                const groupEntities = group.entities.map(id => entitiesList.find(e => e.id === id));
                return (<WidgetEntities name={group.name} entities={groupEntities} />);
            case 'area':
                if (!areasList || !entitiesList) {
                    return;
                }
                const area = areasList.find(e => e.id === target.id);
                if (!area) {
                    return (
                        <CardBody>
                            <CardText>Area {target.id} not found</CardText>
                        </CardBody>
                    );
                }
                const areaEntities = entitiesList.filter(e => e.areaId === area.id);
                return (<WidgetEntities name={area.name} entities={areaEntities} />);
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
        </Card>
    );
}

export default Widget;