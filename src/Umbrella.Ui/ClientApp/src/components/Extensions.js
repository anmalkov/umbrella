import React, { useRef, useState } from 'react';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter, Row, Alert } from 'reactstrap';
import Extension from './Extension';

const Extensions = () => {

    const extensions = [
        { id: "1", displayName: "Test 1", entitiesCount: 10, registered: true },
        { id: "2", displayName: "Test 2", entitiesCount: 5, registered: true },
        {
            id: "3", displayName: "Test 3", registered: false, htmlForRegistration: `
                <div class=\"mb-3\">
                  <label for=\"bridgeIp\" class= \"form-label\">Hue bridge IP address</label >
                  <input type=\"text\" class= \"form-control\" id=\"bridgeIp\" aria-describedby=\"hubIpAddressHelp\" >
                  <div id=\"hubIpAddressHelp\" class=\"form-text\">In the Hue mobile app go to: Settings -> My Hue System -> Philips Hue</div>
                </div >
                <p><b>NOTE:</b>Please make sure that you press the button on your Philips Hue Hub before you press 'Register' button below</p>` },
        { id: "4", displayName: "Test 4", registered: false, htmlForRegistration: "Test text 2" },

    ];

    const [selectedExtension, setSelectedExtension] = useState({});
    const [isRegistrationDialogOpen, setIsRegistrationDialogOpen] = useState(false);

    const registrationParamsRef = useRef(null);

    const toggleRegistrationDialog = () => setIsRegistrationDialogOpen(!isRegistrationDialogOpen);

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
        //send unregister request
    }

    const registerExtension = (extension) => {
        const inputs = Array.from(registrationParamsRef.current.getElementsByTagName("input"));
        const data = {}
        inputs.reduce((result, e) => {
            result[e.id] = e.value;
        }, data);
        // send register request with data 
        setIsRegistrationDialogOpen(false);
    }


    return (
      <div>
            <h1 className="swimlane">Registered</h1>
            <Row>
                {extensions.filter(e => e.registered).map(e =>
                    <Extension key={e.id} id={e.id} displayName={e.displayName} entitiesCount={e.entitiesCount} registered={e.registered} onSelect={selectExtension} />
                )}
            </Row>
            <h1 className="swimlane">Not registered</h1>
            <Row>
                {extensions.filter(e => !e.registered).map((e) =>
                    <Extension key={e.id} id={e.id} displayName={e.displayName} entitiesCount={e.entitiesCount} registered={e.registered} htmlForRegistration={e.htmlForRegistration} onSelect={selectExtension} />
                )}
            </Row>
            <Modal isOpen={isRegistrationDialogOpen} centered={true} toggle={toggleRegistrationDialog}>
                <ModalHeader toggle={toggleRegistrationDialog}>Register {selectedExtension.displayName}</ModalHeader>
                <div ref={registrationParamsRef}>
                    <Alert color="danger" className="m-3 mb-0">
                        Error: Button on the hub is not pressed
                    </Alert>
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
