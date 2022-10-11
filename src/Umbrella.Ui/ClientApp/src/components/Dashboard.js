import React, { useState } from 'react';
import { Row } from 'reactstrap';
import Widget from './Widget';

const Dashboard = () => {

    const widgets = [
        { id: 'w1', col: 1, position: 1, enities: ['hue.light.test1'] },
        { id: 'w2', col: 2, position: 2, enities: ['hue.light.test2'] },
        { id: 'w3', col: 2, position: 1, enities: ['hue.light.test3'] },
        { id: 'w4', col: 4, position: 1, enities: ['hue.light.test4'] }
    ];

    const [widgetsList, setWidgetsList] = useState(widgets);

    if (widgetsList.length === 0) {
        return (
            <div>
                <p>There are no widgets</p>
            </div>
        );
    }

    return (
        <Row>
            { [1, 2, 3, 4].map(col => (
                <div class="col-lg-3 col-md-6 col-sm-12">
                    {widgetsList.filter(w => w.col === col).sort((a, b) => a.position > b.position ? 1 : -1).map(w => (
                        <Widget />
                     ))}
                </div>
            ))}
        </Row>
    );
}

export default Dashboard;
