import React from "react";
import { Button, Col, Modal, Row } from "react-bootstrap";

const ConfirmModal = ({ isOpen, close, action }) => {
  return (
    <Modal animation={false} show={isOpen} backdrop="static" size="lg">
      <Modal.Header>
        <Modal.Title>Delete Timesheet Entry</Modal.Title>
        <Button
          variant=""
          className="btn-close"
          onClick={() => close()}
        ></Button>
      </Modal.Header>
      <Modal.Body>
        <Row>
          <Col lg={12} style={{ textAlign: "right" }}>
            <Row>
              <Col lg={12}>
                <div
                  className="row mt-4"
                  style={{ textAlign: "right", alignItems: "baseline" }}
                >
                  <div className="col-sm-4 align-content-md-between">
                    <label>Are you sure to delete ?</label>
                  </div>
                </div>
              </Col>
            </Row>
          </Col>
        </Row>
      </Modal.Body>
      <Modal.Footer>
        <Button onClick={() => close()} variant="danger light">
          Cancel
        </Button>
        <Button variant="primary" onClick={() => action()}>
          Yes
        </Button>
      </Modal.Footer>
    </Modal>
  );
};

export default ConfirmModal;
