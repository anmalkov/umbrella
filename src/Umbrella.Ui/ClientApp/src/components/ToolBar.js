import React from 'react';
import { Row, Col, Button, List, ListInlineItem } from 'reactstrap';

const ToolBar = ({ dashboards, currentDashboard, addHandler, selectHandler, editHandler, deleteHandler }) => {

    return (
        <Row>
            {!dashboards || dashboards.length === 0 ? (
                <Col>No dashboards yet</Col>
            ) : (
                <Col>
                    <List type="inline">
                        {dashboards.map(d => (
                            <ListInlineItem>
                                <Button key={d.id} onClick={() => selectHandler(d.id)} color={currentDashboard && d.id === currentDashboard.id ? 'info' : 'secondary'}>{d.name}</Button>
                            </ListInlineItem>
                        ))}
                    </List>
                </Col>
            )}
            <Col>
                <List type="inline float-end">
                    <ListInlineItem>
                        <Button className="" onClick={addHandler}>+</Button>
                    </ListInlineItem>
                    {currentDashboard ? (
                        <ListInlineItem>
                            <Button className="" onClick={editHandler}>Edit</Button>
                            {' '}
                            <Button className="" onClick={deleteHandler}>Delete</Button> 
                        </ListInlineItem>
                    ) : null}
                </List>
            </Col>
        </Row>
    );
}

export default ToolBar;
