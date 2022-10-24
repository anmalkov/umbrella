import React from 'react';
import { Row, Col, Button } from 'reactstrap';

const ToolBar = ({ dashboards, addHandler, selectHandler }) => {

    return (
        <Row>
            {!dashboards || dashboards.length === 0 ? (
                <Col>No dashboards yet</Col>
            ) : (
                <Col>
                    {dashboards.map(d => (
                        <Button key={d.id} onClick={() => selectHandler(d.id)}>{d.name}</Button>
                    ))}
                </Col>
            )}
            <Col><Button className="float-end" onClick={addHandler}>+</Button></Col>
        </Row>
    );
}

export default ToolBar;
