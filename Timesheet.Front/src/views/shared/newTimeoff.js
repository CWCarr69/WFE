import React, { useEffect, useState, useCallback } from "react";
import { Button, Col, Modal, Row } from "react-bootstrap";
import Select from "react-select";
import DateRangePicker from "react-bootstrap-daterangepicker";
import "bootstrap-daterangepicker/daterangepicker.css";
import { getMyLightTeam } from "../../redux/actions/employees";
import { getTimeoffTypes } from "../../redux/actions/referentials";
import { addTimeoff } from "../../redux/actions/timesoffs";
import { enumerateDaysBetweenDates } from "../../services/util";
import { displayError, displaySuccess } from "../../services/toast";
import { useSelector } from "react-redux";
import moment from "moment";

const NewTimeoff = ({ isOpen, onClose, selectedEmployeeId }) => {

  const [types, setTypes] = useState([]);
  const [employees, setEmployees] = useState([]);
  const [timeoff, setTimeoff] = useState({});
  const [hoursError, setHoursError] = useState("");
  const [availableHoursError, setAvailableHoursError] = useState("");

  const currentUser = useSelector((state) => state.auth.auth);
  const isAdmin = currentUser?.isAdministrator || false;

  const createTimeOff = async () => {
    var dates = enumerateDaysBetweenDates(moment(timeoff.start), moment(timeoff.end));

    var newTimeoff = {
      requestStartDate: timeoff.start,
      requestEndDate: timeoff.end,
      employeeId: timeoff.employeeId,
      employeeComment: timeoff.employeeComment,
      entries: dates.map((d) => {
        return {
          employeeId: timeoff.employeeId,
          requestDate: new Date(d),
          type: timeoff.type.value,
          hours: timeoff.hours,
          label: timeoff.label && timeoff.label.value,
        };
      }),
    };

    await addTimeoff(newTimeoff)
      .then((res) => {
        close();
        displaySuccess("Successful submit");
      })
      .catch((err) => displayError(err, "Error while adding a new timeoff"));
  };

  const validateAvailableHours = (type, selectedEmployee) => {
    // Skip validation if user is admin
    if (isAdmin) {
      setAvailableHoursError("");
      return true;
    }

    if (!type || !selectedEmployee) {
      setAvailableHoursError("");
      return true;
    }

    const typeLabel = type.label?.toLowerCase() || "";
    const employee = employees.find(e => e.employeeId === selectedEmployee);

    if (!employee) {
      setAvailableHoursError("");
      return true;
    }

    // Check available hours based on type
    if (typeLabel.includes("vacation")) {
      if (employee.vacationHours <= 0) {
        setAvailableHoursError("No vacation hours available.");
        return false;
      }
    } else if (typeLabel.includes("personal")) {
      if (employee.personalHours <= 0) {
        setAvailableHoursError("No personal hours available.");
        return false;
      }
    }

    setAvailableHoursError("");
    return true;
  };

  const validateHours = (hours, type) => {
    // Skip validation if user is admin
    if (isAdmin) {
      setHoursError("");
      return true;
    }

    if (!hours || !type) {
      setHoursError("");
      return true;
    }

    const hoursNum = parseFloat(hours);
    const typeLabel = type.label?.toLowerCase() || "";

    // Check if type is "personal" and hours must be 2, 4, or 8
    if (typeLabel.includes("personal")) {
      if (![2, 4, 8].includes(hoursNum)) {
        setHoursError("Personal time off must be 2, 4, or 8 hours.");
        return false;
      }
    }

    // Check if type is "vacation" and hours must be 4 or 8
    if (typeLabel.includes("vacation")) {
      if (![4, 8].includes(hoursNum)) {
        setHoursError("Vacation time off must be 4 or 8 hours.");
        return false;
      }
    }

    setHoursError("");
    return true;
  };

  const handleHoursChange = (value) => {
    const updatedTimeoff = { ...timeoff, hours: value };
    setTimeoff(updatedTimeoff);
    validateHours(value, updatedTimeoff.type);
  };

  const handleTypeChange = (selectedType) => {
    const updatedTimeoff = { ...timeoff, type: selectedType };
    setTimeoff(updatedTimeoff);
    validateHours(updatedTimeoff.hours, selectedType);
    validateAvailableHours(selectedType, updatedTimeoff.employeeId);
  };

  const handleEmployeeChange = (selectedEmployee) => {
    const updatedTimeoff = { ...timeoff, employeeId: selectedEmployee };
    setTimeoff(updatedTimeoff);
    validateAvailableHours(updatedTimeoff.type, selectedEmployee);
  };

  const canSave = () => {
    return (
      timeoff.hours &&
      timeoff.type &&
      timeoff.employeeId &&
      timeoff.start &&
      timeoff.end &&
      !hoursError &&
      !availableHoursError
    );
  };

  const fetchEmployees = useCallback(async () => {
    await getMyLightTeam(false)
      .then((resp) => setEmployees(resp))
      .catch((err) => displayError(err, "Error while fetching team data"))
  }, []);

  const fetchTypes = async () => {
    await getTimeoffTypes(true)
      .then((resp) => setTypes(resp))
      .catch((err) => displayError(err, "Error while getting Time off types"))
  };

  useEffect(() => {
    const updatedTimeoff = { ...timeoff, employeeId: selectedEmployeeId };
    setTimeoff(updatedTimeoff);
    validateAvailableHours(updatedTimeoff.type, selectedEmployeeId);
  }, [selectedEmployeeId])

  useEffect(() => {
    fetchTypes();
    fetchEmployees();
  }, [fetchEmployees])

  const close = () => {
    setTimeoff({ employeeId: selectedEmployeeId });
    setHoursError("");
    setAvailableHoursError("");
    onClose();
  }

  return (
    <Modal animation={false} show={isOpen} backdrop="static" size="lg">
      <Modal.Header>
        <Modal.Title>Timeoff edit</Modal.Title>
        <Button variant="" className="btn-close" onClick={close}></Button>
      </Modal.Header>
      <Modal.Body>
        <Row>
          <Col lg={12}>
            <Row>
              <Col lg={12}>
                <div className="row mt-2" style={{ textAlign: "right", alignItems: "center" }}>
                  <div className="col-sm-4 align-content-md-between">
                    <label>Period</label>
                  </div>
                  <div className="col-sm-8 mt-2 mt-sm-0">
                    <DateRangePicker
                      initialSettings={{ startDate: timeoff.start, endDate: timeoff.end }}
                      onApply={(e) => {
                        let dts = e.target.value.split(" - ");
                        let dt = { ...timeoff };
                        dt.start = new Date(dts[0]);
                        dt.end = new Date(dts[1]);
                        setTimeoff(dt);
                      }}
                    >
                      <input type="text" className="form-control input-daterange-timepicker" />
                    </DateRangePicker>
                  </div>
                </div>
              </Col>
              <Col lg={12}>
                <div className="row mt-4" style={{ textAlign: "right", alignItems: "center" }}>
                  <div className="col-sm-4 align-content-md-between">
                    <label>Hours</label>
                  </div>
                  <div className="col-sm-8 mt-2 mt-sm-0">
                    <input
                      type="text"
                      className={`form-control ${hoursError ? "is-invalid" : ""}`}
                      placeholder="Hours per day"
                      value={timeoff.hours || ""}
                      onChange={(e) => handleHoursChange(e.target.value)}
                    />
                    {hoursError && <div className="invalid-feedback d-block">{hoursError}</div>}
                  </div>
                </div>
              </Col>
              {!selectedEmployeeId &&
                <Col lg={12}>
                  <div className="row mt-4 form-group" style={{ textAlign: "right", alignItems: "center" }}>
                    <div className="col-sm-4 align-content-md-between">
                      <label>Employee</label>
                    </div>
                    <div className="col-sm-8 mt-2 mt-sm-0">
                      <Select
                        className="basic-single"
                        classNamePrefix="select"
                        name="classes"
                        options={employees.map((e) => { return { value: e.employeeId, label: `${e.fullName} - ${e.employeeId}` } })}
                        onChange={(e) => handleEmployeeChange(e.value)}
                      />
                    </div>
                  </div>
                </Col>
              }
              <Col lg={12}>
                <div className="row mt-4 form-group" style={{ textAlign: "right", alignItems: "center" }}>
                  <div className="col-sm-4 align-content-md-between">
                    <label>Vacation type</label>
                  </div>
                  <div className="col-sm-8 mt-2 mt-sm-0">
                    <Select
                      className={`basic-single ${availableHoursError ? "is-invalid" : ""}`}
                      classNamePrefix="select"
                      name="classes"
                      value={timeoff.type}
                      options={types.map((t) => { return { value: t.numId, label: t.payrollCode.replace("_", " ") } })}
                      onChange={handleTypeChange}
                    />
                    {availableHoursError && <div className="invalid-feedback d-block">{availableHoursError}</div>}
                  </div>
                </div>
              </Col>
              <Col lg={12}>
                <div className="row mt-4 form-group" style={{ textAlign: "right", alignItems: "center" }}>
                  <div className="col-sm-4 align-content-md-between">
                    <label>Comment</label>
                  </div>
                  <div className="col-sm-8 mt-2 mt-sm-0">
                    <textarea
                      className="form-control"
                      rows="4"
                      id="comment"
                      value={timeoff.employeeComment || ""}
                      onChange={(e) => setTimeoff({ ...timeoff, employeeComment: e.target.value })}
                    ></textarea>
                  </div>
                </div>
              </Col>
            </Row>
          </Col>
        </Row>
      </Modal.Body>
      <Modal.Footer>
        <Button onClick={close} variant="danger light">Close</Button>
        {canSave() && <Button variant="primary" onClick={createTimeOff}> Save changes</Button>}
      </Modal.Footer>
    </Modal>
  );
};

export default NewTimeoff;
