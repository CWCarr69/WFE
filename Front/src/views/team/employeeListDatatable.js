import React, { Fragment } from "react";
import { Link } from "react-router-dom";
import NewTimeoffButton from "../shared/newTimeoffButton";

const EmployeeListDatatable = ({onFilterUpdate, onAddClick}) => {
    return  (     
    <div className="card">
        <div className="card-body">
            <div className="w-100">
                <div id="example_wrapper" className="dataTables_wrapper">
                    <table id="example" className="display w-100 dataTable">
                        <thead>
                            <tr role="row">
                                <th>Name</th>
                                <th>Timesheet</th>
                                <th>Time off</th>
                            </tr>
                        </thead>
                        <tbody>
                        {onFilterUpdate().map((da, i) => (
                            <tr key={i}>
                            <td>
                                <Link to={`/profile/${da.employeeId}`}><strong>{da.fullName}</strong> - <strong>{da.employeeId}</strong> :</Link>
                                <span>&nbsp;[ P :{" "}<strong>{da.personalBalance}</strong>- V :{" "}<strong>{da.vacationBalance}</strong>]</span>
                            </td>
                            <td>
                                {da.timesheetId ? (
                                <Link to={`/timesheets/${da.lastTimesheetPayrollPeriod}/employee/${da.employeeId}/${new Date(da.lastTimesheetWorkDate).getTime()}`}>
                                    <strong>{da.lastTimesheetStatusString.replace("_", " ")}</strong>
                                </Link>
                                ) : (
                                <div>{da.lastTimesheetStatusString.replace("_", " ")}</div>
                                )}
                            </td>
                            <td>
                                {da.timeoffId ? (
                                // <Link to={`/timeoffs/${da.timeoffId}/employee/${da.employeeId}/${new Date(da.lastTimeoffRequestDate).getTime()}`}>
                                //     <strong>{da.lastTimeoffStatusString.replace("_", " ")}</strong>
                                // </Link>
                                <Link to={`/timeoffs/${da.employeeId}`}>History</Link>
                                ) : (
                                <Fragment>{da.lastTimeoffStatusString.replace("_", " ")}</Fragment>
                                )}
                                <NewTimeoffButton onClick={() => onAddClick(da.employeeId) } />
                            </td>
                            </tr>
                        ))}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
  </div>
  );
}

export default EmployeeListDatatable;