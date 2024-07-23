import React, { useEffect, useState, } from "react";
import { useSelector } from "react-redux";
import { Button } from "react-bootstrap";
import { Link } from "react-router-dom";
import {approveTimeOff, getPendingTimesoffs, rejectTimeOff,} from "../../redux/actions/timesoffs";
import { DefaultGlobalDateFormat } from "../../services/util";
import { displayError, displaySuccess } from "../../services/toast";
import moment from "moment";
import NewTimeoff from "../shared/newTimeoff";
import NewTimeoffButton from "../shared/newTimeoffButton";

const Timeoff = () => {
  const [items, setItems] = useState([]);
  const [direct, setDirect] = useState(true);
  const [isAddTimeoffOpen, setIsAddTimeoffOpen] = useState(false);

  const user = useSelector((state) => state.auth.auth);

  const fetchData = async () => {
    await getPendingTimesoffs(direct)
      .then((resp) => { setItems(resp.items);})
      .catch((err) => displayError(err, "Error while retrieving pending time offs"));
  }

  useEffect(() => fetchData(), [direct, isAddTimeoffOpen]);

  const approve = async (timeoff) => {
    await approveTimeOff({
      employeeId: timeoff.employeeId,
      timeoffId: timeoff.timeoffId,
      comment: "",
    })
    .then((res) => { 
      displaySuccess("Time off approved");
      fetchData();
    })
    .catch((err) => displayError(err, "Error while trying to approve time off. Try again or contact administrator"));
  };

  const reject = async (timeoff) => {
    await rejectTimeOff({
      employeeId: timeoff.employeeId,
      timeoffId: timeoff.timeoffId,
      comment: "",
    })
    .then((res) => { 
      displaySuccess("Time off rejected");
      fetchData();
    })
    .catch((err) => displayError(err, "Error while trying to reject time off. Try again or contact administrator"));
  };

  return (
    <div className="card">
      <div className="card-header border-0 pb-0 flex-wrap justify-content-center">
        <h4 className="fs-20 font-w500">Time off</h4>
      </div>
      <div className="card-body pt-2">
        <div className="d-sm-flex d-block align-items-center justify-content-center">
          <div className="col-xl-12 col-xxl-12">
            <div className=" mt-1">
              <Button
                className="btn-sm" variant={direct ? "danger" : "outline-primary"}
                onClick={() => setDirect(!direct) }
              >
                {direct ? "Show all" : "Direct Report"}
              </Button>
              {user.isAdministrator &&
                <>
                  <NewTimeoffButton onClick={() => setIsAddTimeoffOpen(true)}/>
                  <NewTimeoff isOpen={isAddTimeoffOpen} onClose={() => setIsAddTimeoffOpen(false)} />
                </>
              }
            </div>
            <div className="w-100">
              <div id="example_wrapper" className="dataTables_wrapper">
                <table id="example" className="display w-100 dataTable">
                  <thead>
                    <tr role="row">
                      <th>Name</th>
                      <th>Hrs.</th>
                      <th>Start</th>
                      <th>End</th>
                      <th></th>
                    </tr>
                  </thead>
                  <tbody>
                    {items.map((da, i) => (
                        <tr key={i}>
                          <td>
                            <Link to={`/timeoffs/${da.data.timeoffId}/employee/${da.data.employeeId}`}>
                              {da.data.fullName}
                              <span> &nbsp;[ P :{" "} <strong>{da.data.personalBalance}</strong>- V :{" "}<strong>{da.data.vacationBalance}</strong>]</span>
                            </Link>
                          </td>
                          <td>{da.data.totalHours} <strong>{da.data.payrollCode.substring(0,3)}</strong></td>
                          <td>{moment(da.data.requestStartDate).format(DefaultGlobalDateFormat)}</td>
                          <td>{moment(da.data.requestEndDate).format(DefaultGlobalDateFormat)}</td>
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

export default Timeoff;
