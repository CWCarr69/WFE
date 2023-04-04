import React from "react";
import { Button, Col, Modal, Row } from "react-bootstrap";
import Select from "react-select";
import DateRangePicker from "react-bootstrap-daterangepicker";
import "bootstrap-daterangepicker/daterangepicker.css";

const NewTimeOff = ({ isOpen, close, data, updateData, save, types }) => {
  const canSave = () => {
    return data.hours === undefined ||
      data.hours === undefined ||
      data.type === undefined
      ? false
      : true;
  };

  // const getTypes = () => {
  //   return types.filter(
  //     (t) =>
  //       !t.payrollCode.toLowerCase().includes("shop") &&
  //       !t.payrollCode.toLowerCase().includes("unpaid")
  //   );
  // };

  return (
    <Modal animation={false} show={isOpen} backdrop="static" size="lg">
      <Modal.Header>
        <Modal.Title>Timeoff edit</Modal.Title>
        <Button
          variant=""
          className="btn-close"
          onClick={() => close()}
        ></Button>
      </Modal.Header>
      <Modal.Body>
        <Row>
          <Col lg={12}>
            <Row>
              <Col lg={12}>
                <div
                  className="row mt-2"
                  style={{ textAlign: "right", alignItems: "center" }}
                >
                  <div className="col-sm-4 align-content-md-between">
                    <label>Period</label>
                  </div>
                  <div className="col-sm-8 mt-2 mt-sm-0">
                    <DateRangePicker
                      initialSettings={{
                        startDate: data.start,
                        endDate: data.end,
                      }}
                      onApply={(e) => {
                        let dts = e.target.value.split(" - ");
                        let dt = { ...data };
                        dt.start = new Date(dts[0]);
                        dt.end = new Date(dts[1]);
                        updateData(dt);
                      }}
                    >
                      <input
                        type="text"
                        className="form-control input-daterange-timepicker"
                      />
                    </DateRangePicker>
                  </div>
                </div>
              </Col>
              <Col lg={12}>
                <div
                  className="row mt-4"
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
                      options={types.map((t) => {
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
              <Col lg={12}>
                <div
                  className="row mt-4 form-group"
                  style={{ textAlign: "right", alignItems: "center" }}
                >
                  <div className="col-sm-4 align-content-md-between">
                    <label>Comment</label>
                  </div>
                  <div className="col-sm-8 mt-2 mt-sm-0">
                    <textarea
                      className="form-control"
                      rows="4"
                      id="comment"
                      value={data.employeeComment}
                      onChange={(e) =>
                        updateData({ ...data, employeeComment: e.target.value })
                      }
                    ></textarea>
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
        {canSave() && (
          <Button variant="primary" onClick={() => save()}>
            Save changes
          </Button>
        )}
      </Modal.Footer>
    </Modal>
  );
};

export default NewTimeOff;
