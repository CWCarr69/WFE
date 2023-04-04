import React, {
  Fragment,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";
import { Button, Col, Pagination, Row } from "react-bootstrap";
import {
  useTable,
  useGlobalFilter,
  useFilters,
  usePagination,
  useGroupBy,
  useExpanded,
} from "react-table";
import { COLUMNS } from "./data";
import { ThemeContext } from "../../context/themeContext";
import Select from "react-select";
import { Link } from "react-router-dom";
import { getEmployee } from "../../redux/actions/employees";
import { toast } from "react-toastify";
import {
  getDepartments,
  getPayrollPeriods,
  getTimesheetEntryStatuses,
  getTimesheetStatuses,
} from "../../redux/actions/referentials";
import {
  approveTimesheet,
  exportPeriod,
  exportPeriodAfterFinalize,
  finalizeTimesheet,
  rejectTimesheet,
  addTimesheetException,
  review,
} from "../../redux/actions/timesheets";
import { DefaultGlobalDateFormat } from "../../services/util";
import SpinnerComponent from "../../components/spinner/spinner";
import { useSelector } from "react-redux";
import _ from "lodash";
import moment from "moment";

const TimesSheet = () => {
  const { setTitle } = useContext(ThemeContext);
  const loading = useSelector((state) => state.auth.showLoading);
  const columns = useMemo(() => COLUMNS, []);
  const [employees, setEmployees] = useState([]);

  const user = useSelector((state) => state.auth.auth);

  const [departments, setDepartments] = useState([]);

  const [periods, setPeriods] = useState([]);
  const [selectedPeriod, setSelectedPeriod] = useState(null);
  const [selectedPeriodStart, setSelectedPeriodStart] = useState(null);
  const [selectedPeriodEnd, setSelectedPeriodEnd] = useState(null);

  const [status, setStatus] = useState([]);
  const [entryStatus, setEntryStatus] = useState([]);
  
  const [selectedEmployee, setSelectedEmployee] = useState("");
  const [selectedDepartment, setselectedDepartment] = useState("");


  const [reveiwData, setReveiwData] = useState({
    authorizedActions: [],
    data: {
      items: [],
    },
    otherData: { TotalQuantity: 0 },
  });

  const fetchEmployees = async () => {
    await getEmployee()
      .then((resp) => {
        let dts = [
          {
            value: "",
            label: "",
          },
        ];
        resp.forEach((e) => {
          dts.push({
            value: e.id,
            label: `${e.fullName} - ${e.id}`,
          });
        });
        setEmployees(dts);
      })
      .catch((err) => {
        toast.error(
          err.response?.data.message ? err.response?.data.message : "Error"
        );
      });
  };

  const fetchPayrollPeriods = async () => {
    await getPayrollPeriods()
      .then((resp) => {
        setPeriods(
          resp.map((e) => {
            return {
              value: e.code,
              label: `${e.code} [${moment(e.startDate).format(DefaultGlobalDateFormat)}-${moment(e.endDate).format(DefaultGlobalDateFormat)}]`,
              start: e.startDate?.substring(0, 10),
              end: e.endDate?.substring(0, 10)
            };
          })
        );
        if (resp?.length > 0 && selectedPeriod === null) {
          setSelectedPeriod(resp[0].code);
          setSelectedPeriodStart(resp[0].startDate?.substring(0, 10));
          setSelectedPeriodEnd(resp[0].endDate?.substring(0, 10));
        }
      })
      .catch((err) => {
        toast.error(
          err.response?.data.message ? err.response?.data.message : "Error"
        );
      });
  };

  const fetchDepartments = async () => {
    await getDepartments()
      .then((resp) => {
        resp
		.filter((r) => r.departmentId !== null)
		.map((e) => {
      let dts = [
        {
          value: "",
          label: "",
        },
      ];
      resp.forEach((e) => {
        dts.push({
          value: e.departmentId,
			    label: e.departmentName,
        });
        setDepartments(dts);
      });
		})
      })
      .catch((err) => {
        toast.error(
          err.response?.data.message ? err.response?.data.message : "Error"
        );
      });
  };

  const fetchStatus = async () => {
    await getTimesheetStatuses()
      .then((resp) => {
        setStatus(
          resp.map((e) => {
            return {
              ...e,
              active: false,
            };
          })
        );
      })
      .catch((err) => {
        toast.error(
          err.response?.data.message ? err.response?.data.message : "Error"
        );
      });
  };

  const fetchEntriesStatus = async () => {
    await getTimesheetEntryStatuses()
      .then((resp) => {
        setEntryStatus(
          resp.map((e) => {
            return {
              ...e,
              active: false,
            };
          })
        );
      })
      .catch((err) => {
        toast.error(
          err.response?.data.message ? err.response?.data.message : "Error"
        );
      });
  };

  const fetchReviewData = useCallback(async () => {
    if (selectedPeriod) {
      let dt = await review(selectedPeriod);
      var array = [];
      dt.data.items.forEach((i) => {
        i.data.entries.forEach((d) => {
          array.push({
            id: i.data.employeeId,
            entryId: d.id,
            employee: i.data.fullname,
            department: d.department,
            work: moment(d.workDate).format(DefaultGlobalDateFormat),
            payrollCod: d.payrollCode,
            quantity: d.quantity,
            description: d.description,
            service: `${d.serviceOrderNumber ?? ""} ${
              d.serviceOrderDescription ? " - " + d.serviceOrderDescription : ""
            }`,
            job: `${d.jobNumber ?? ""} ${
              d.jobDescription ? " - " + d.jobDescription : ""
            }`,
            task: `${d.jobTaskNumber ?? ""} ${
              d.jobTaskDescription ? " - " + d.jobTaskDescription : ""
            }`,
            total: i.data.total,
            labor: d.laborCode,
            center: d.profitCenterNumber ?? i.data.defaultProfitCenter,
            area: d.workArea,
            timesheetStatus: i.data.statusName.replaceAll("_", " "),
            status: d.statusName.replaceAll("_", " "),
            overtime: i.data.overtime,
            authorizedActions: i.authorizedActions,
            timesheetId: i.data.timesheetId,
            delete: d.payrollCode != "REGULAR" && d.payrollCode != "OVERTIME",
            deleteAction: () => createTimesheetException(d),
            urlToTimesheet: `/timesheets/${selectedPeriod}/employee/${i.data.employeeId}/${new Date(d.workDate).getTime()}`,
            urlToEmployee: `/profile/${i.data.employeeId}`,
            subRows: undefined,
          });
        });
      });
      setReveiwData({
        data: {
          items: array,
        },
        otherData: dt.data.otherData,
        authorizedActions: dt.authorizedActions,
      });
    }
  }, [selectedPeriod]);

  useEffect(() => {
    fetchEmployees();
    fetchPayrollPeriods();
    fetchDepartments();
    fetchStatus();
    fetchEntriesStatus();
    fetchReviewData();
  }, [fetchReviewData]);

  const onFilterUpdate = useCallback(() => {
    let arrayData = reveiwData.data.items;

    let empFilter = arrayData.filter((d) => d.id === selectedEmployee);

    let depFilter = arrayData.filter((d) => d.department === selectedDepartment);

    let array =
      selectedEmployee !== "" && selectedDepartment !== ""
        ? _.intersection(empFilter, depFilter)
        : selectedEmployee !== "" && selectedDepartment === ""
        ? empFilter
        : selectedEmployee === "" && selectedDepartment !== ""
        ? depFilter
        : arrayData;

    let selectedStatus = status.find((s) => s.active);
    let entriesStatus = entryStatus
      .filter((s) => s.active)
      .map((s) => s.name.replaceAll("_", " "));

    var resp = selectedStatus !== undefined && entriesStatus.length > 0
      ? array.filter(
          (d) =>
            d.timesheetStatus.toLowerCase() ===
              selectedStatus.name.replaceAll("_", " ").toLowerCase() &&
            entriesStatus.includes(d.status)
        )
      : selectedStatus !== undefined && entriesStatus <= 0
      ? array.filter(
          (d) =>
            d.timesheetStatus.toLowerCase() ===
            selectedStatus.name.replaceAll("_", " ").toLowerCase()
        )
      : selectedStatus === undefined && entriesStatus.length > 0
      ? array.filter((d) => entriesStatus.includes(d.status))
      : array;
	  return resp;
	}, [status, entryStatus, reveiwData, selectedEmployee, selectedDepartment]);

  const data = useMemo(() => onFilterUpdate(), [onFilterUpdate]);
  const initGroupBy = useMemo(() => ["employee"], []);

  const tableInstance = useTable(
    {
      columns,
      data,
      initialState: {
        pageIndex: 0,
        groupBy: initGroupBy,
        isAllRowsExpanded: true,
      },
    },
    useFilters,
    useGlobalFilter,
    useGroupBy,
    useExpanded,
    usePagination
  );

  const {
    getTableProps,
    getTableBodyProps,
    headerGroups,
    prepareRow,
    state,
    page,
    gotoPage,
    pageCount,
    nextPage,
    previousPage,
    canNextPage,
    canPreviousPage,
  } = tableInstance;

  const { pageIndex } = state;

  useEffect(() => {
    setTitle("Review Timesheet");
  }, [setTitle]);

  useEffect(() => tableInstance.toggleAllRowsExpanded(true), [tableInstance]);

  const onSelectPeriod = (e) => {
    setSelectedPeriod(e.value);
	  setSelectedPeriodStart(e.start);
	  setSelectedPeriodEnd(e.end);
  };

  const finalize = async () => {
    await finalizeTimesheet({ timesheetId: data[0].timesheetId })
      .then((res) => {
        toast.success("Successful finalize", {
          position: "top-right",
          autoClose: 5000,
          hideProgressBar: false,
          closeOnClick: true,
          pauseOnHover: true,
          draggable: true,
        });
        fetchReviewData();
      })
      .catch((err) => {
        toast.error(
          err.response?.data.message
            ? err.response?.data.message
            : "Error while finalizing timesheet. Please retry",
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

  const createTimesheetException = async (entry) => {
    await addTimesheetException({
        employeeId: entry.id,
        timesheetEntryId: entry.entryId,
        timesheetId: entry.timesheetId,
        isHoliday: entry.payrollCod == "HOLIDAY"
      })
      .then((res) => {
        toast.success("Successful add of timesheet Exception", {
          position: "top-right",
          autoClose: 5000,
          hideProgressBar: false,
          closeOnClick: true,
          pauseOnHover: true,
          draggable: true,
        });
        fetchReviewData();
      })
      .catch((err) => {
        toast.error(
          err.response?.data.message
            ? err.response?.data.message
            : "Error adding timesheet exception",
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

  const approve = async (timesheet) => {
    await approveTimesheet({
      employeeId: timesheet.id,
      timesheetId: timesheet.timesheetId,
    })
      .then((res) => {
        toast.success("Successful approve", {
          position: "top-right",
          autoClose: 5000,
          hideProgressBar: false,
          closeOnClick: true,
          pauseOnHover: true,
          draggable: true,
        });
        fetchReviewData();
      })
      .catch((err) => {
        toast.error(
          err.response?.data.message
            ? err.response?.data.message
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

  const reject = async (timesheet) => {
    await rejectTimesheet({
      employeeId: timesheet.id,
      timesheetId: timesheet.timesheetId,
    })
      .then((res) => {
        toast.success("Successful reject", {
          position: "top-right",
          autoClose: 5000,
          hideProgressBar: false,
          closeOnClick: true,
          pauseOnHover: true,
          draggable: true,
        });
        fetchReviewData();
      })
      .catch((err) => {
        toast.error(
          err.response?.data.message
            ? err.response?.data.message
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

  const exportRaw = async () => {
    console.log(selectedDepartment, selectedEmployee);
    await exportPeriod(selectedPeriod, selectedDepartment, selectedEmployee)
      .then((res) => {})
      .catch((err) => {

        toast.error(
          err.response?.data.message
            ? err.response?.data.message
            : `Error export`,
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

  const exportFinalized = async () => {
    await exportPeriodAfterFinalize(selectedPeriod)
      .then((res) => {
        toast.success("Successful exported finalized data", {
        position: "top-right",
        autoClose: 5000,
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true,
        });
      })
      .catch((err) => {
        toast.error(
          err.response?.data.message
            ? err.response?.data.message
            : "Error export",
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

  return loading ? (
    <div
      style={{
        display: "flex",
        alignItems: "center",
        alignContent: "center",
        justifyContent: "center",
        height: "100%",
      }}
    >
      <SpinnerComponent />
    </div>
  ) : (
    <>
      <div className="card">
        <div className="card-body">
          <div className="w-100 table-responsive">
            <div>
              <Row>
                <Col
                  style={{
                    display: "flex",
                    alignItems: "flex-end",
                    justifyContent: "flex-end",
                  }}
                >
                  {reveiwData.authorizedActions.find(
                    (a) => a.name === "FINALIZE"
                  ) && (
                    <div className="d-inline-block">
                      <Button variant="primary" onClick={() => finalize()}>
                        Finalize
                      </Button>
                    </div>
                  )}
                </Col>
              </Row>
            </div>
            <div>
              <div className="basic-form">
                <div className="row mt-4">
                  <div className="form-group mb-3 col-md-3">
                    <label>Payroll Period</label>
                    <Select
                      className="basic-single"
                      classNamePrefix="select"
                      name="classes"
                      options={periods}
                      value={
                        periods.filter((p) => p.value === selectedPeriod)[0]
                      }
                      onChange={(e) => onSelectPeriod(e)}
                    />
                  </div>
                  <div className="form-group mb-3 col-md-3">
                    <label>Employee</label>
                    <Select
                      className="basic-single"
                      classNamePrefix="select"
                      name="classes"
                      options={employees}
                      onChange={(e) => {
                        setSelectedEmployee(e.value);
                      }}
                    />
                  </div>
                  <div className="form-group mb-3 col-md-3">
                    <label>Department</label>
                    <Select
                      className="basic-single"
                      classNamePrefix="select"
                      name="classes"
                      options={departments}
                      onChange={(e) => {
                        setselectedDepartment(e.value);
                      }}
                    />
                  </div>
                  <div className="form-group mb-3 col-md-1">
                    <label>Total Quantity</label>
                    <label>
                      <strong>
                        {reveiwData.otherData &&
                        reveiwData.otherData.TotalQuantity}
                      </strong>
                    </label>
                  </div>
                </div>
                <div className="row">
                  <div className="mb-1 col-md-12">
                    From <strong>{moment(selectedPeriodStart).format(DefaultGlobalDateFormat)}</strong> to <strong>{moment(selectedPeriodEnd).format(DefaultGlobalDateFormat)}</strong>
                  </div>
                </div>
              </div>
            </div>
            <div className="row mb-3 mt-5 align-items-center">
              <div className="col-xl-12 col-lg-12">
                <div className="d-sm-flex  d-block align-items-center">
                  <div className="d-flex align-items-center">
                    <div className="media-body">
                      <label className="me-3">Timesheet Status : </label>
                      {status.map((s, index) => (
                        <Button
                          key={index}
                          className="me-2 btn-xxs"
                          variant={s.active ? "danger" : "outline-danger"}
                          onClick={() => {
                            let array = [...status];
                            array.forEach((x, i) => {
                              if (i !== index) {
                                x.active = false;
                              }
                            });
                            array[index].active = !array[index].active;
                            setStatus(array);
                          }}
                        >
                          {s.name.replaceAll("_", " ")}
                        </Button>
                      ))}<br/>
                      <label className="me-3">Timesheet entry Status : </label>
                      {entryStatus.map((s, index) => (
                        <Button
                          key={index}
                          className="me-2 btn-xxs"
                          variant={s.active ? "danger" : "outline-danger"}
                          onClick={() => {
                            let array = [...entryStatus];
                            array[index].active = !array[index].active;
                            setEntryStatus(array);
                          }}
                        >
                          {s.name.replaceAll("_", " ")}
                        </Button>
                      ))}
                    </div>
                  </div>
                </div>
              </div>
              <div className="col-xl-12 col-lg-12">
                {user.isAdministrator && data.find((d) => d.timesheetStatus.toLowerCase() === "finalized") && (
                  <Fragment>
                    <Button
                      className="me-2 btn-xxs"
                      variant="outline-primary"
                      onClick={() => exportFinalized()}
                    >
                      <i className="fas fa-download me-3"></i>
                      Finalized Timesheet
                    </Button>
                  </Fragment>
                )}

                <Button
                  className="me-2 btn-xxs"
                  variant="outline-primary"
                  onClick={() => exportRaw()}
                >
                  <i className="fas fa-download me-3"></i>
                  To Csv
                </Button>
              </div>
              <div className="table-responsive mt-4">
                <table
                  {...getTableProps()}
                  className="table dataTable display table-striped table-bordered"
                >
                  <thead>
                    {headerGroups.map((headerGroup, i) => (
                      <tr {...headerGroup.getHeaderGroupProps()} key={i}>
                        {headerGroup.headers.map((column, id) => (
                          <Fragment key={id}>
                            {column.Header !== "Id" ? (
                              <th {...column.getHeaderProps()}>
                                {column.render("Header")}
                                {column.canFilter
                                  ? column.render("Filter")
                                  : null}
                              </th>
                            ) : (
                              <th style={{ width: 10 }}></th>
                            )}
                          </Fragment>
                        ))}
                      </tr>
                    ))}
                  </thead>
                  <tbody {...getTableBodyProps()} className="">
                    {page.map((row, i) => {
                      prepareRow(row);
                      return (
                        <Fragment key={i}>
                          {row.isGrouped ? (
                            <tr>
                              <td className="py-2">
                                <div style={{ display: "flex" }}>
                                  {
                                    <span {...row.getToggleRowExpandedProps()}>
                                      {row.isExpanded ? (
                                        <i
                                          className="fas fa-chevron-circle-up"
                                          style={{ cursor: "pointer" }}
                                        ></i>
                                      ) : (
                                        <i
                                          className="fas fa-chevron-circle-down"
                                          style={{ cursor: "pointer" }}
                                        ></i>
                                      )}
                                    </span>
                                  }
                                </div>
                              </td>
                              <td className="py-2" colSpan={2} style={{ fontWeight: "bold" }}>
                                {row &&
                                  row.leafRows.length > 0 &&
                                  `${row.leafRows[0].original.employee} - ${row.leafRows[0].original.id}`}
                              </td>
                              <td className="py-2" colSpan={2} style={{ fontWeight: "bold" }}>
                                Status:{" "}
                                {row &&
                                  row.leafRows.length > 0 &&
                                  row.leafRows[0].original.timesheetStatus}
                              </td>
                              <td className="py-2" style={{ fontWeight: "bold" }}>
                                Total: {row && row.leafRows[0].original.total}
                              </td>
                              <td className="py-2" style={{ fontWeight: "bold" }}>
                                Overtime:{" "}
                                {row && row.leafRows[0].original.overtime}
                              </td>
                              <td className="py-2" colSpan={6} style={{ fontWeight: "bold" }}>
                                {row.leafRows[0].original.authorizedActions.find(
                                  (a) => a.name === "APPROVE"
                                ) && (
                                  <span
                                    className="badge badge-rounded badge-primary"
                                    style={{ cursor: "pointer" }}
                                    onClick={() =>
                                      approve(row.leafRows[0].original)
                                    }
                                  >
                                    Approve
                                  </span>
                                )}
                                {row.leafRows[0].original.authorizedActions.find(
                                  (a) => a.name === "REJECT"
                                ) && (
                                  <span
                                    className="badge badge-rounded badge-danger mx-2"
                                    style={{ cursor: "pointer" }}
                                    onClick={() =>
                                      reject(row.leafRows[0].original)
                                    }
                                  >
                                    Reject
                                  </span>
                                )}
                              </td>
                            </tr>
                          ) : (
                            <tr {...row.getRowProps()}>
                              {row.cells.map((cell, id) => {
                                return (
                                  <td className="py-2"
                                    {...cell.getCellProps()}
                                    key={id}
                                  >
                                    {cell.render("Cell", {original : row.original})}
                                  </td>
                                );
                              })}
                            </tr>
                          )}
                        </Fragment>
                      );
                    })}
                  </tbody>
                </table>
                <div id="example_wrapper" className="dataTables_wrapper">
                  <div className="d-sm-flex text-center justify-content-between align-items-center mt-1">
                    <div className="dataTables_info" />
                    <Pagination
                      size={"sx"}
                      className={`pagination-gutter pagination- pagination-circle`}
                    >
                      <li className="page-item page-indicator">
                        <Link
                          className="page-link"
                          to="#"
                          onClick={() => previousPage()}
                          disabled={!canPreviousPage}
                        >
                          <i className="la la-angle-left" />
                        </Link>
                      </li>
                      {Array.from({ length: pageCount }).map((x, i) => (
                        <Pagination.Item
                          key={i + 1}
                          active={pageIndex === i}
                          onClick={() => gotoPage(i)}
                        >
                          {i + 1}
                        </Pagination.Item>
                      ))}
                      <li className="page-item page-indicator">
                        <Link
                          className="page-link"
                          to="#"
                          onClick={() => nextPage()}
                          disabled={!canNextPage}
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
    </>
  );
};

export default TimesSheet;
