import React, { useState } from 'react';
import { Button, Modal } from 'react-bootstrap';


const _Modal = ({
    isShow = false,
    title,
    descriptions,
    confirmLabel,
    confirmCallbackFn,
    abortLabel = "Close",
    abortCallbackFn,
    ...otherProps
}) => {
    const [show, setShow] = useState(isShow);

    const handleClose = () => setShow(false);
    
    const onClickConfirm = (event) => {
        event.preventDefault();

        typeof confirmCallbackFn === "function" && confirmCallbackFn();
    };
    const onClickAbort = (event) => {
        if (abortCallbackFn == undefined)
        {
            handleClose()
            return;
        }
        event.preventDefault();
        typeof abortCallbackFn === "function" && abortCallbackFn();
    };
    const confirmBtn = confirmLabel == undefined ? "" : (<Button variant="primary" onClick={onClickConfirm}>{confirmLabel}</Button>)
    
    return (
        <Modal show={show} onHide={onClickAbort}>
            <Modal.Header closeButton>
                <Modal.Title>{title}</Modal.Title>
            </Modal.Header>
            <Modal.Body>{ descriptions}</Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={onClickAbort}>
                    {abortLabel}
                </Button>
                { confirmBtn}
            </Modal.Footer>
        </Modal>
    );
};

export default _Modal;