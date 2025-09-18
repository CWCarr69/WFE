import React, {
  useState,
  useEffect,
} from "react";
import { Button } from "react-bootstrap";
import {
  getTimeoffStatuses,
  getTimesheetStatuses,
  getTimesheetEntryStatuses,
} from "../../redux/actions/referentials";
import { displayError } from "../../services/toast";

const EmployeeListStatusFilter = ({onTimeoffStatusesFilterChanged, onTimesheetStatusesFilterChanged}) => {
  const [timesheetStatuses, setTimesheetStatuses] = useState([]);
  const [timeoffStatuses, setTimeoffStatuses] = useState([]);

  const fetchStatus = async () => {
      let statuses = [];  
      await getTimesheetEntryStatuses()
      .then((resp) => resp.forEach((e) => statuses.push({ ...e, active: false})))
      .catch((err) => displayError(err, "Error while retrieving timesheet entry statuses"));

      await getTimesheetStatuses(true)
      .then((resp) => resp.forEach((e) => statuses.push({ ...e, active: false})))
      .catch((err) => displayError(err, "Error while retrieving timesheet statuses"));
  
      setTimesheetStatuses(statuses);
  };

  
  const fetchTimeOffStatus = async () => {
      await getTimeoffStatuses()
      .then((resp) => setTimeoffStatuses(resp.map((e) => { return {...e, active: false} })))
      .catch((err) => displayError(err, "Error while retrieving timeoff statuses"));
  };

  useEffect(() => {
    fetchStatus();
    fetchTimeOffStatus();
  }, []);

  const filterButton = (statuses, setStatuses, index, onStatusesChanged, status) => {
    return <Button
      key={index} className="me-2 btn-xxs" variant={status.active ? "danger" : "outline-danger"}
      onClick={() => {
        let array = [...statuses];
        array[index].active = !array[index].active;
        onStatusesChanged(array.filter(a => a.active).map((a) => a.name));
        setStatuses(array);
      }}
    >
      {status.name.replace("_", " ")}
    </Button>
  }

  return  (
  <div className="row mb-3 align-items-center">
    <div className="col-xl-12 col-lg-12 hidden">
      <div className="d-sm-flex  d-block align-items-center">
        <div className="d-flex align-items-center">
          <div className="media-body">
            <label className="me-3">Timesheet Status : </label>
            {timesheetStatuses.map((status, index) => filterButton(timesheetStatuses, setTimesheetStatuses, index, onTimesheetStatusesFilterChanged, status))}
            <br/>
            <label className="me-3">Timeoff Status : </label>
            {timeoffStatuses.map((status, index) => filterButton(timeoffStatuses, setTimeoffStatuses, index, onTimeoffStatusesFilterChanged, status))}
          </div>
        </div>
      </div>
    </div>
  </div>
  );
}

export default EmployeeListStatusFilter;
