import React from "react";
import "react-datepicker/dist/react-datepicker.css";
import { Button, Col, Modal, Row } from "react-bootstrap";
import DatePicker from "react-datepicker";

const NewHoliday = ({ isOpen, close, data, updateData, save, action }) => {
  return (
    <Modal animation={false} show={isOpen} backdrop="static" size="lg">
      <Modal.Header>
        <Modal.Title>{action}</Modal.Title>
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
                  className="row mt-2"
                  style={{ textAlign: "right", alignItems: "baseline" }}
                >
                  <div className="col-sm-4 align-content-md-between">
                    <label>Date</label>
                  </div>
                  <div className="col-sm-3 mt-2 mt-sm-0">
                    <DatePicker
                      selected={data.date}
                      className="form-control"
                      value={data.date}
                      dateFormat="MM/dd/yyyy"
                      onChange={(e) =>
                        updateData({
                          ...data,
                          date: e,
                        })
                      }
                    />
                  </div>
                </div>
              </Col>
              <Col lg={12}>
                <div
                  className="row mt-4"
                  style={{ textAlign: "right", alignItems: "baseline" }}
                >
                  <div className="col-sm-4 align-content-md-between">
                    <label>Description</label>
                  </div>
                  <div className="col-sm-8 mt-2 mt-sm-0">
                    <input
                      type="text"
                      className="form-control"
                      placeholder="Description"
                      value={data.description}
                      onChange={(e) =>
                        updateData({ ...data, description: e.target.value })
                      }
                    />
                  </div>
                </div>
              </Col>
              <Col lg={12}>
                <div
                  className="row mt-4"
                  style={{ textAlign: "right", alignItems: "baseline" }}
                >
                  <div className="col-sm-4 align-content-md-between">
                    <label>Notes</label>
                  </div>
                  <div className="col-sm-8 mt-2 mt-sm-0">
                    <input
                      type="text"
                      className="form-control"
                      placeholder="Notes"
                      value={data.notes}
                      onChange={(e) =>
                        updateData({ ...data, notes: e.target.value })
                      }
                    />
                  </div>
                </div>
              </Col>
              <Col lg={12}>
                <div
                  className="row mt-4"
                  style={{ textAlign: "right", alignItems: "baseline" }}
                >
                  <div className="col-sm-4 align-content-md-between">
                    <label>Is-it reccurent ?</label>
                  </div>
                  <div className="col-sm-1 mt-2 mt-sm-0">
                    <input
                      type="checkbox"
                      className="custom-control-input"
                      id="checkbox2"
                      value={data.isRecurrent}
                      onChange={(e) =>
                        updateData({ ...data, isRecurrent: !data.isRecurrent })
                      }
                    />
                  </div>
                </div>
              </Col>
            </Row>
          </Col>
        </Row>
      </Modal.Body>
      <Modal.Footer>
        <Button onClick={() => close()} variant="danger light">
          Close
        </Button>
        <Button variant="primary" onClick={() => save()}>
          Save changes
        </Button>
      </Modal.Footer>
    </Modal>
  );
};

export default NewHoliday;
