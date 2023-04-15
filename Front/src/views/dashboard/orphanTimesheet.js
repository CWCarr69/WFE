import React, { useEffect, useRef, useState } from "react";
import { Button, Pagination } from "react-bootstrap";
import { Link } from "react-router-dom";
import { toast } from "react-toastify";
import {
  approveTimesheet,
  getOrphanTimesheets,
  rejectTimesheet,
} from "../../redux/actions/timesheets";

const OrphanTimesheet = () => {
  const [data, setData] = useState({
    totalItems: 0,
    itemsPerPage: 50,
  });
  const [items, setItems] = useState([]);
  const [page, setPage] = useState(1);
  const [pagination, setPagination] = useState([]);
  const timessheetsData = useRef([]);
  const activePag = useRef(0);
  const [direct, setDirect] = useState(true);

  const fetchData = async () => {
    await getOrphanTimesheets(direct, page)
      .then((resp) => {
        setData(resp);
        setItems(resp.items);
      })
      .catch((err) => {
        toast.error(
          err.response.data.message ? err.response.data.message : "Error"
        );
      });
  };

  useEffect(() => {
    fetchData();
  }, [direct, page]);

  useEffect(() => {
    if (data) {
      setPagination(
        Array(Math.ceil(data.totalItems / data.itemsPerPage))
          .fill()
          .map((_, i) => i + 1)
      );

      timessheetsData.current = items;
    }
  }, [data, timessheetsData, items]);

  const onClick = (i) => {
    fetchData();
    setPage(i + 1);
    activePag.current = i;
  };

  const approve = async (timeoff) => {
    await approveTimesheet({
      employeeId: timeoff.employeeId,
      timesheetId: timeoff.timesheetId,
      comment: "",
    })
      .then((res) => {
        toast.success("Successful timesheet approve", {
          position: "top-right",
          autoClose: 5000,
          hideProgressBar: false,
          closeOnClick: true,
          pauseOnHover: true,
          draggable: true,
        });
        fetchData();
      })
      .catch((err) => {
        toast.error(
          err.response.data.message
            ? err.response.data.message
            : "Error approve",
          {
            position: "top-right",
            autoClose: 5000,
            hideProgressBar: false,
            closeOnClick: true,
            pauseOnHover: true,
            draggable: true,
          }
        );
      });
  };

  const reject = async (timeoff) => {
    await rejectTimesheet({
      employeeId: timeoff.employeeId,
      timesheetId: timeoff.timesheetId,
      comment: "",
    })
      .then((res) => {
        toast.success("Successful timesheet reject", {
          position: "top-right",
          autoClose: 5000,
          hideProgressBar: false,
          closeOnClick: true,
          pauseOnHover: true,
          draggable: true,
        });
        fetchData();
      })
      .catch((err) => {
        toast.error(
          err.response.data.message
            ? err.response.data.message
            : "Error reject",
          {
            position: "top-right",
            autoClose: 5000,
            hideProgressBar: false,
            closeOnClick: true,
            pauseOnHover: true,
            draggable: true,
          }
        );
      });
  };

  return (
    <div className="card">
      <div className="card-header border-0 pb-0 flex-wrap justify-content-center">
        <h4 className="fs-20 font-w500">Finalized Timesheets with pending entries</h4>
      </div>
      <div className="card-body pt-2">
        <div className="d-sm-flex d-block align-items-center justify-content-center">
          <div className="col-xl-12 col-xxl-12">
            <div className=" mt-1">
              <Button
                className="btn-sm"
                variant={direct ? "danger" : "outline-primary"}
                onClick={() => {
                  setDirect(!direct);
                }}
              >
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
                      <th>In Progress Hrs.</th>
                      <th>Submitted Hrs.</th>
                      <th></th>
                    </tr>
                  </thead>
                  <tbody>
                    {timessheetsData &&
                      timessheetsData.current &&
                      timessheetsData.current.map((da, i) => (
                        <tr key={i}>
                          <td>
                            <Link
                              to={`/timesheets/${da.data.timesheetId}/employee/${da.data.employeeId}`}
                            >
                              {da.data.fullName}
                            </Link>
                          </td>
                          <td>{da.data.payrollPeriod}</td>
                          <td>{da.data.totalHours}</td>
                          <td>{da.data.hoursPerStatus?.submitted}</td>
                          <td>{da.data.hoursPerStatus?.in_progress}</td>
                          <td>
                              <span
                                className="text-dark"
                                style={{ cursor: "pointer" }}
                                onClick={() => approve(da.data)}
                              >
                                &#10003;
                              </span>
                              <span
                                className="text-danger mx-2"
                                style={{ cursor: "pointer" }}
                                onClick={() => reject(da.data)}
                              >
                                &#10006;
                              </span>
                          </td>
                        </tr>
                      ))}
                  </tbody>
                </table>
                <div className="d-sm-flex text-center justify-content-between align-items-center mt-3">
                  <div className="dataTables_info" />
                  <Pagination
                    size={"sx"}
                    className={`pagination-gutter pagination- pagination-circle`}
                  >
                    <li className="page-item page-indicator">
                      <Link
                        className="page-link"
                        to="#"
                        onClick={() =>
                          activePag.current > 0 &&
                          onClick(activePag.current - 1)
                        }
                      >
                        <i className="la la-angle-left" />
                      </Link>
                    </li>
                    {pagination.map((number, i) => (
                      <Pagination.Item
                        key={number}
                        active={activePag.current === i}
                        onClick={() => onClick(i)}
                      >
                        {number}
                      </Pagination.Item>
                    ))}
                    <li className="page-item page-indicator">
                      <Link
                        className="page-link"
                        to="#"
                        onClick={() =>
                          activePag.current + 1 < pagination.length &&
                          onClick(activePag.current + 1)
                        }
                      >
                        <i className="la la-angle-right" />
                      </Link>
                    </li>
                  </Pagination>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default OrphanTimesheet;
