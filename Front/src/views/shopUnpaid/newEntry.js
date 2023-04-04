import React from "react";
import { Button, Col, Modal, Row } from "react-bootstrap";
import Select from "react-select";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

const NewEntry = ({ isOpen, close, data, updateData, save, types, labels }) => {
  const getTypes = () => {
    return types.filter(
      (t) =>
        t.payrollCode.toLowerCase().includes("shop") ||
        t.payrollCode.toLowerCase().includes("unpaid")
    );
  };

  return (
    <Modal animation={false} show={isOpen} backdrop="static" size="lg">
      <Modal.Header>
        <Modal.Title>
          {data.timeoffEntryId ? "Updating" : "New"} Entry
        </Modal.Title>
        <Button
          variant=""
          className="btn-close"
          onClick={() => close()}
        ></Button>
      </Modal.Header>
      <Modal.Body>
        <Row>
          {!data.timeoffEntryId && (
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
          )}
          <Col lg={12}>
            <Row>
              <Col lg={12}>
                <div
                  className="row mt-2"
                  style={{ textAlign: "right", alignItems: "center" }}
                >
                  <div className="col-sm-4 align-content-md-between">
                    <label>Hours</label>
                  </div>
                  <div className="col-sm-8 mt-2 mt-sm-0">
                    <input
                      type="text"
                      className="form-control"
                      placeholder="Hours"
                      value={data.hours}
                      onChange={(e) =>
                        updateData({ ...data, hours: e.target.value })
                      }
                    />
                  </div>
                </div>
              </Col>
              {/* <Col lg={12}>
                <div
                  className="row mt-4 form-group"
                  style={{ textAlign: "right", alignItems: "center" }}
                >
                  <div className="col-sm-4 align-content-md-between">
                    <label>Label</label>
                  </div>
                  <div className="col-sm-8 mt-2 mt-sm-0">
                    <CreatableSelect
                      isClearable
                      options={labels}
                      onChange={(e) => {
                        updateData({ ...data, label: e });
                      }}
                    />
                  </div>
                </div>
              </Col> */}
              <Col lg={12}>
                <div
                  className="row mt-4 form-group"
                  style={{ textAlign: "right", alignItems: "center" }}
                >
                  <div className="col-sm-4 align-content-md-between">
                    <label>Vacation type</label>
                  </div>
                  <div className="col-sm-8 mt-2 mt-sm-0">
                    <Select
                      className="basic-single"
                      classNamePrefix="select"
                      name="classes"
                      value={types.filter((s) => s.numId === data.type)[0]}
                      options={getTypes().map((t) => {
                        return {
                          value: t.numId,
                          label: t.payrollCode.replace("_", " "),
                        };
                      })}
                      onChange={(e) => {
                        updateData({ ...data, type: e });
                      }}
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

export default NewEntry;
