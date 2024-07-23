import React, { useEffect, useState } from "react";
import { Button } from "react-bootstrap";
import { Link } from "react-router-dom";
import { approveTimesheet, getPendingTimesheets, rejectTimesheet } from "../../redux/actions/timesheets";
import { displayError, displaySuccess } from "../../services/toast";


const Timesheet = () => {
  const [items, setItems] = useState([]);
  const [direct, setDirect] = useState(true);

  const fetchData = async () => {
    await getPendingTimesheets(direct)
    .then((resp) => setItems(resp.items))
    .catch((err) => displayError(err, "Error while retrieving pending timesheets"));
  }

  useEffect(() => fetchData(), [direct]);

  const approve = async (timeoff) => {
    await approveTimesheet({employeeId: timeoff.employeeId, timesheetId: timeoff.timesheetId, comment: ""})
    .then((res) => {
      displaySuccess("Timesheet approved");
      fetchData();
    })
    .catch((err) => displayError(err, "Error while approving timesheet"));
  }

  const reject = async (timeoff) => {
    await rejectTimesheet({employeeId: timeoff.employeeId, timesheetId: timeoff.timesheetId, comment: ""})
    .then((res) => {
      displaySuccess("Timesheet rejected");
      fetchData();
    })
    .catch((err) => displayError(err, "Error while rejecting timesheet"));
  }

  return (
    <div className="card">
      <div className="card-header border-0 pb-0 flex-wrap justify-content-center">
        <h4 className="fs-20 font-w500">Timesheet</h4>
      </div>
      <div className="card-body pt-2">
        <div className="d-sm-flex d-block align-items-center justify-content-center">
          <div className="col-xl-12 col-xxl-12">
            <div className=" mt-1">
              <Button className="btn-sm" variant={direct ? "danger" : "outline-primary"} onClick={() => { setDirect(!direct) }}>
                {direct ? "Show all" : "Direct Report"}
              </Button>
            </div>
            <div className="w-100">
              <div id="example_wrapper" className="dataTables_wrapper">
                <table id="example" className="display w-100 dataTable">
                  <thead>
                    <tr role="row">
                      <th>Name</th>
                      <th>Payroll Period</th>
                      <th>Total Hrs.</th>
                      <th></th>
                    </tr>
                  </thead>
                  <tbody>
                    {items?.map((da, i) => (
                        <tr key={i}>
                          <td>
                            <Link to={`/timesheets/${da.data.timesheetId}/employee/${da.data.employeeId}`}>
                              {da.data.fullName}
                            </Link>
                          </td>
                          <td>{da.data.payrollPeriod}</td>
                          <td>{da.data.totalHours}</td>
                          <td>
                            {da.authorizedActions.find((a) => a.name === "APPROVE") && (
                              <span className="text-dark" style={{ cursor: "pointer" }} onClick={() => approve(da.data)}>&#10003;</span>
                            )}
                            {da.authorizedActions.find((a) => a.name === "REJECT") && (
                              <span className="text-danger mx-2" style={{ cursor: "pointer" }} onClick={() => reject(da.data)}>&#10006;</span>
                            )}
                          </td>
                        </tr>
                      ))}
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Timesheet;
