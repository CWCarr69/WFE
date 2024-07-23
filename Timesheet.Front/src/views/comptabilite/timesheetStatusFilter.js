import React, { useState, useEffect } from "react";
import { Button } from "react-bootstrap";
import { getTimesheetStatuses, getTimesheetEntryStatuses } from "../../redux/actions/referentials";
import { displayError } from "../../services/toast";
  
const TimesheetStatusFilter = ({onTimesheetEntryStatusesFilterChanged}) => {
    const [timesheetEntryStatuses, setTimesheetEntryStatuses] = useState([]);

    const fetchTimesheetEntriesStatuses = async () => {
        await getTimesheetEntryStatuses()
        .then((resp) => { setTimesheetEntryStatuses(resp.map((e) => { return { ...e, active: false } })) })
        .catch((err) => displayError(err, "Error while fectching Timesheet entry status"));
    };

    useEffect(() => {fetchTimesheetEntriesStatuses() }, []);

    const filterButton = (statuses, setStatuses, index, status, onStatusesChanged) => {
        return <Button
            key={index}
            className="me-2 btn-xxs"
            variant={status.active ? "danger" : "outline-danger"}
            onClick={() => {
                let array = [...statuses];
                array[index].active = !array[index].active;
                onStatusesChanged(array.filter(a => a.active).map((a) => a.name.replaceAll("_", " ")));
                setStatuses(array);
            }}
        >
            {status.name.replaceAll("_", " ")}
        </Button>
    }

    return (
    <div className="col-xl-12 col-lg-12">
        <div className="d-sm-flex  d-block align-items-center">
            <div className="d-flex align-items-center">
                <div className="media-body">
                    <label className="me-3">Timesheet Status : </label>
                    {timesheetEntryStatuses.map((status, index) => filterButton(timesheetEntryStatuses, setTimesheetEntryStatuses, index, status, onTimesheetEntryStatusesFilterChanged))}
                </div>
            </div>
        </div>
    </div>
  );
}

export default TimesheetStatusFilter;
