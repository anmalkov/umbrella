import React, { useState } from 'react';
import { Row, Col, Spinner, Button, Input, Form, FormGroup, Label } from 'reactstrap';
import { useQuery } from 'react-query';
import { fetchEntities } from '../fetchers/entities';
import { fetchAreas } from '../fetchers/areas';
import { fetchGroups } from '../fetchers/groups';

const GetNewWidget = () => { return { id: 0, name: '', column: 1, positionInColumn: 10, type: 'entity', parameters: [] } };

const EditWidget = ({ oldWidget, saveHandler, cancelHandler }) => {

    const widgetTypes = [
        { id: 'entity', title: 'Entity' },
        { id: 'area', title: 'Area' },
        { id: 'group', title: 'Group' },
        { id: 'weather', title: 'Weather' }
    ];

    const [widget, setWidget] = useState({ ...oldWidget });

    const entitiesQuery = useQuery(['entities'], fetchEntities, { staleTime: 60000 });
    const entitiesList = entitiesQuery.data;
    const groupsQuery = useQuery(['groups'], fetchGroups, { staleTime: 60000 });
    const groupsList = groupsQuery.data;
    const areasQuery = useQuery(['areas'], fetchAreas, { staleTime: 60000 });
    const areasList = areasQuery.data

    const columnChangeHandler = (e) => {
        setWidget(old => {
            return { ...old, column: Number(e.target.value) };
        });
    }

    const positionChangeHandler = (e) => {
        setWidget(old => {
            return { ...old, positionInColumn: Number(e.target.value) };
        });
    }

    const typeChangeHandler = (e) => {
        setWidget(old => {
            return { ...old, type: e.target.value };
        });
    }

    const targetIdChangeHandler = (e) => {
        const parameters = [];
        switch (widget.type) {
            case 'weather':
                parameters.push({ key: 'city', value: e.target.value });
            default:
                parameters.push({ key: 'id', value: e.target.value });
        }
        setWidget(old => {
            return { ...old, parameters: parameters };
        });
    }

    const getParameter = name => {
        const parameter = widget.parameters.find(p => p.key === name);
        return parameter ? parameter.value : '';
    }

    return (
        <Form>
            <FormGroup>
                <Col>
                    <Button onClick={() => saveHandler(widget)} disabled={!entitiesList || entitiesList.length === 0}>Save</Button>
                    <Button onClick={cancelHandler}>Cancel</Button>
                </Col>
            </FormGroup>
            <FormGroup>
                <Label for="exampleEmail">Column</Label>
                <Input type="select" value={widget.column} onChange={columnChangeHandler}>
                    <option>1</option>
                    <option>2</option>
                    <option>3</option>
                    <option>4</option>
                </Input>
            </FormGroup>
            <FormGroup>
                <Label for="exampleEmail">Position in column</Label>
                <Input type="text" value={widget.positionInColumn} onChange={positionChangeHandler} />
            </FormGroup>
            <FormGroup>
                <Label for="exampleEmail">Type</Label>
                <Input type="select" value={widget.type} onChange={typeChangeHandler}>
                    {widgetTypes.map(w => (
                        <option key={w.id} value={w.id}>{w.title}</option>
                    ))}
                </Input>
            </FormGroup>
            {widget.type === 'entity' ?
                (
                    <FormGroup>
                        <Label for="exampleEmail">Select entity</Label>
                        <Input type="select" value={getParameter('id')} disabled={!entitiesList} onChange={targetIdChangeHandler}>
                            {entitiesList && entitiesList.length > 0
                                ? entitiesList.map(e => (
                                    <option key={e.id} value={e.id}>{e.name} [{e.type}]</option>
                                ))
                                : (
                                    <option>There are no entities yet</option>
                                )
                            }
                        </Input>
                    </FormGroup>
                )
                : widget.type === 'area' ?
                (
                    <FormGroup>
                        <Label for="exampleEmail">Select area</Label>
                        <Input type="select" value={getParameter('id')} disabled={!areasList} onChange={targetIdChangeHandler}>
                            {areasList && areasList.length > 0
                                ? areasList.map(a => (
                                    <option key={a.id} value={a.id}>{a.name}</option>
                                ))
                                : (
                                    <option>There are no areas yet</option>
                                )
                            }
                        </Input>
                    </FormGroup>
                )
                : widget.type === 'group' ?
                (
                    <FormGroup>
                        <Label for="exampleEmail">Select group</Label>
                        <Input type="select" value={getParameter('id')} disabled={!groupsList} onChange={targetIdChangeHandler}>
                            {groupsList && groupsList.length > 0
                                ? groupsList.map(g => (
                                    <option key={g.id} value={g.id}>{g.name}</option>
                                ))
                                : (
                                    <option>There are no groups yet</option>
                                )
                            }
                        </Input>
                    </FormGroup>
                )
                : widget.type === 'weather' ?
                (
                    <FormGroup>
                        <Label for="exampleEmail">City</Label>
                        <Input type="text" value={getParameter('city')} onChange={targetIdChangeHandler} />
                    </FormGroup>
                ) : null
            }
        </Form>
    );
    
}

export default EditWidget;
export { GetNewWidget };
