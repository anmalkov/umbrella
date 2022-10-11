import React, { useEffect, useRef, useState } from 'react';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter, Row, Alert, Spinner } from 'reactstrap';
import Extension from './Extension';

const Extensions = () => {

    const [extensionsList, setExtensionsList] = useState([]);
    const [error, setError] = useState(null);
    const [registrationError, setRegistrationError] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [selectedExtension, setSelectedExtension] = useState({});
    const [isRegistrationDialogOpen, setIsRegistrationDialogOpen] = useState(false);

    const registrationParamsRef = useRef(null);

    const toggleRegistrationDialog = () => {
        setIsRegistrationDialogOpen(!isRegistrationDialogOpen)
        if (isRegistrationDialogOpen) {
            setRegistrationError(null);
        }
    };

    const selectExtension = (extension) => {
        setSelectedExtension(extension);
        if (extension.registered) {
            if (window.confirm(`Do you want to unregister extension '${extension.displayName}' ?`)) {
                unregisterExtension(extension.id)
            }
            return;
        }
        setIsRegistrationDialogOpen(true);
    }

    const unregisterExtension = (id) => {
        const request = {
            method: 'DELETE'
        };
        fetch(`api/extensions/${id}`, request)
            .then(
                response => {
                    if (response.status !== 200) {
                        response.json()
                            .then(
                                result => {
                                    setError(result.detail);
                                },
                                error => {
                                    setError(new Error(`[${response.status} ${response.statusText}] ${error}`));
                                }
                            );
                        return;
                    }
                    getExtensions();
                },
                error => {
                    setError(error);
                }
            );

    }

    const registerExtension = () => {
        setRegistrationError(null);
        const inputs = Array.from(registrationParamsRef.current.getElementsByTagName('input'));
        const data = [];
        inputs.forEach(e => data.push({ key: e.id ? e.id : e.name, value: e.value }));
        const request = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ parameters: data })
        };
        fetch(`api/extensions/${selectedExtension.id}`, request)
            .then(
                response => {
                    if (response.status !== 200) {
                        response.json()
                            .then(
                                result => {
                                    setRegistrationError(result.detail);
                                },
                                error => {
                                    setRegistrationError(error);
                                }
                        );
                        return;
                    }
                    setIsRegistrationDialogOpen(false);
                    getExtensions();
                },
                error => {
                    setRegistrationError(error);
                }
            );
    }

    const getExtensions = () => {
        fetch('api/extensions')
            .then(response => response.json())
            .then(
                result => {
                    setIsLoading(false);
                    setExtensionsList(result);
                },
                error => {
                    setIsLoading(false);
                    setError(error)
                }
            )
    }

    useEffect(() => {
        getExtensions();
    }, []);

    if (isLoading) {
        return (
            <div className="text-center">
                <Spinner color="light">
                    Loading...
                </Spinner>
            </div>
        );
    }

    return (
        <div>
            {error &&
                <Alert color="danger">
                    {error.message}
                </Alert>
            }
            <h1 className="swimlane">Registered</h1>
            <Row>
                {extensionsList.filter(e => e.registered).map(e =>
                    <Extension key={e.id} id={e.id} displayName={e.displayName} entitiesCount={e.entitiesCount} registered={e.registered} onSelect={selectExtension} />
                )}
            </Row>
            <h1 className="swimlane">Not registered</h1>
            <Row>
                {extensionsList.filter(e => !e.registered).map((e) =>
                    <Extension key={e.id} id={e.id} displayName={e.displayName} entitiesCount={e.entitiesCount} registered={e.registered} htmlForRegistration={e.htmlForRegistration} onSelect={selectExtension} />
                )}
            </Row>
            <Modal isOpen={isRegistrationDialogOpen} centered={true} toggle={toggleRegistrationDialog}>
                <ModalHeader toggle={toggleRegistrationDialog}>Register {selectedExtension.displayName}</ModalHeader>
                <div ref={registrationParamsRef}>
                    { registrationError &&
                        <Alert color="danger" className="m-3 mb-0">
                            {registrationError}
                        </Alert>
                    }
                    <ModalBody dangerouslySetInnerHTML={{ __html: selectedExtension.htmlForRegistration }} />
                </div>
                <ModalFooter>
                    <Button color="secondary" onClick={toggleRegistrationDialog}>Close</Button>
                    <Button color="primary" onClick={registerExtension}>Register</Button>
                </ModalFooter>
            </Modal>
      </div>
    );
}

export default Extensions;
