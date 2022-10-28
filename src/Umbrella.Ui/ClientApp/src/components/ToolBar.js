import React, { useEffect, useState } from 'react';
import { List, ListInlineItem } from 'reactstrap';
import { GoChevronDown } from "react-icons/go";
import './ToolBar.css'

const ToolBar = ({ dashboards, currentDashboard, addHandler, selectHandler, editHandler, deleteHandler }) => {

    const [isMinimal, setIsMinimal] = useState(false);
    const [timer, setTimer] = useState(null);

    const toggleIsMinimal = () => {
        setIsMinimal(!isMinimal);
    }

    useEffect(() => {
        if (isMinimal) {
            clearTimeout(timer);
            setTimer(null);
            return;
        }
        setTimer(setTimeout(() => {
            toggleIsMinimal();
        }, 10000));
    }, [isMinimal]);

    return (
        <>
            {isMinimal ? (
                <div className="position-relative navbar-minimal mt-m3 mb-3 toolbar-minimal" onClick={toggleIsMinimal}><GoChevronDown className="position-absolute start-50" /></div>
            ) : (
                <div className="d-flex justify-content-between align-items-center mt-m3 mb-3 px-3 py-1 toolbar">
                    {!dashboards || dashboards.length === 0 ? (
                        <p>No dashboards yet</p>
                    ) : (
                        <div>
                            <List type="inline" className="mb-0">
                                {dashboards.map(d => (
                                    <ListInlineItem key={d.id}>
                                        <a onClick={() => selectHandler(d.id)} className={currentDashboard && d.id === currentDashboard.id ? 'fw-bold' : ''}>{d.name}</a>
                                    </ListInlineItem>
                                ))}
                            </List>
                        </div>
                    )}
                    <div>
                        <List type="inline float-end" className="mb-0">
                            <ListInlineItem>
                                <a className="" onClick={addHandler}>Add</a>
                            </ListInlineItem>
                            {currentDashboard ? (
                                <>
                                    <ListInlineItem className="text-secondary">|</ListInlineItem>
                                    <ListInlineItem>
                                        <a className="" onClick={editHandler}>Edit</a>
                                    </ListInlineItem>
                                    <ListInlineItem className="text-secondary">|</ListInlineItem>
                                    <ListInlineItem>
                                        <a className="" onClick={deleteHandler}>Delete</a>
                                    </ListInlineItem>
                                </>
                            ) : null}
                        </List>
                    </div>
                </div>
            )}
        </>
    );
}

export default ToolBar;
