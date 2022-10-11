import React from 'react';
import { Card, CardBody, CardTitle, CardText } from 'reactstrap';

const Widget = () => {
    return (
        <Card className="widget">
            <CardBody>
                <CardTitle>Widget</CardTitle>
                <CardText>Widget content</CardText>
            </CardBody>
        </Card>
    );
}

export default Widget;