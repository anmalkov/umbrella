import React, { useRef, useState, useEffect } from 'react';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter, Row, UncontrolledAlert, Spinner } from 'reactstrap';
import { useQuery, useQueryClient, useMutation } from 'react-query';
import { fetchExtensions, registerExtension, unregisterExtension } from '../fetchers/extensions';
import Extension from './Extension';

const Extensions = () => {
    const [selectedExtension, setSelectedExtension] = useState({});
    const [isRegistrationDialogOpen, setIsRegistrationDialogOpen] = useState(false);

    const { isError, isLoading, data, error } = useQuery(['extensions'], fetchExtensions, { staleTime: 60000 });
    const extensionsList = data

    const registrationParamsRef = useRef(null);

    const toggleRegistrationDialog = () => {
        setIsRegistrationDialogOpen(!isRegistrationDialogOpen)
    };

    const selectExtension = (extension) => {
        setSelectedExtension(extension);
        if (extension.registered) {
            if (window.confirm(`Do you want to unregister extension '${extension.displayName}' ?`)) {
                unregister(extension.id)
            }
            return;
        }
        registerMutation.reset();
        setIsRegistrationDialogOpen(true);
    }

    const queryClient = useQueryClient();

    const registerMutation = useMutation(({ id, parameters }) => {
        return registerExtension(id, parameters);
    });

    const unregisterMutation = useMutation(id => {
        return unregisterExtension(id);
    });

    const register = async () => {
        try {
            const inputs = Array.from(registrationParamsRef.current.getElementsByTagName('input'));
            const selects = Array.from(registrationParamsRef.current.getElementsByTagName('select'));
            const data = [];
            inputs.forEach(e => data.push({ key: e.id ? e.id : e.name, value: e.type === 'checkbox' ? String(e.checked) : e.value }));
            selects.forEach(e => data.push({ key: e.id ? e.id : e.name, value: e.value }));
            console.log(data);
            await registerMutation.mutateAsync({ id: selectedExtension.id, parameters: data });
            queryClient.invalidateQueries(['extensions']);
            queryClient.invalidateQueries(['entities']);
            queryClient.refetchQueries('extensions', { force: true });
            setIsRegistrationDialogOpen(false);
        }
        catch { }
    }

    const unregister = async (id) => {
        try {
            await unregisterMutation.mutateAsync(id);
            queryClient.invalidateQueries(['extensions']);
            queryClient.invalidateQueries(['entities']);
            queryClient.refetchQueries('extensions', { force: true });
        }
        catch { }
    }
    
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
            {(isError || unregisterMutation.isError) &&
                <UncontrolledAlert color="danger">
                    {isError ? error.message : unregisterMutation.error.message}
                </UncontrolledAlert>
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
                {registerMutation.isError &&
                    <UncontrolledAlert color="danger" className="m-3 mb-0">
                        {registerMutation.error.message}
                    </UncontrolledAlert>
                }
                <div ref={registrationParamsRef}>
                    <ModalBody dangerouslySetInnerHTML={{ __html: selectedExtension.htmlForRegistration }} />
                </div>
                <ModalFooter>
                    <Button color="secondary" onClick={toggleRegistrationDialog}>Close</Button>
                    <Button color="primary" onClick={register} disabled={registerMutation.isLoading}>Register</Button>
                    {registerMutation.isLoading &&
                        <Spinner color="light" size="sm">
                            Loading...
                        </Spinner>
                    }
                </ModalFooter>
            </Modal>
      </div>
    );
}

export default Extensions;
