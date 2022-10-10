import React from 'react';
import { Card, CardBody, CardTitle, CardText, Button } from 'reactstrap';


const Extension = ({ id, displayName, entitiesCount, registered, htmlForRegistration, onSelect }) => {

    return (
        <div className="col-lg-3 col-md-6 col-sm-12">
            <Card className="widget">
                <CardBody>
                    <CardTitle>{displayName}</CardTitle>
                    { entitiesCount > 0 && registered && 
                        <CardText>
                            <a href="/Entities">Show {entitiesCount} entities</a>
                        </CardText>
                    }
                    {registered
                        ? <Button color="secondary" onClick={() => onSelect({ id, displayName, registered, htmlForRegistration })}>Unregister</Button>
                        : <Button color="primary" onClick={() => onSelect({ id, displayName, registered, htmlForRegistration })}>Register</Button>
                    }
                </CardBody>
            </Card>
        </div>
    );
}

export default Extension;
