import React, { useContext, useEffect, useRef } from "react";
import FullCalendar from "@fullcalendar/react";
import dayGridPlugin from "@fullcalendar/daygrid";
import timeGridPlugin from "@fullcalendar/timegrid";
import interactionPlugin from "@fullcalendar/interaction";
import { displayError, displaySuccess } from "../../services/toast";

import {
  getFirstDayOfNextMonth,
  getFirstDayOfPreviousMonth,
  DefaultGlobalDateFormat
} from "../../services/util";

import {
  Accordion,
  Button,
  Card,
  Col,
  Nav,
  Pagination,
  Row,
  Tab,
  Table,
} from "react-bootstrap";
import { useState } from "react";
import moment from "moment";
import { Link } from "react-router-dom";
import { ThemeContext } from "../../context/themeContext";
import {
  approveTimesheet,
  deleteTimesheet,
  getTimesheetEmployeeById,
  getTimesheetEmployeeSummaryByDate,
  getTimesheetEmployeeSummaryByPayroll,
  rejectTimesheet,
  submitTimesheet,
  updateComment,
  getCurrentEmployeeTimesheetPeriod,
} from "../../redux/actions/timesheets";
import { getEmployeeById } from "../../redux/actions/employees";
import { useCallback } from "react";
import { toast } from "react-toastify";
import { getPayrollCodes } from "../../redux/actions/referentials";
import SpinnerComponent from "../../components/spinner/spinner";
import { useSelector } from "react-redux";
import ConfirmModal from "./confirmModal";

const Timesheet = ({ match, history }) => {
  const { setTitle } = useContext(ThemeContext);
  const loading = useSelector((state) => state.auth.showLoading);

  const user = useSelector((state) => state.auth.auth);

  const [accordionTable, setAccordionTable] = useState(false);
  const [accordionTableCalendar, setAccordionTableCalendar] = useState(false);
  const [accordionCalendar, setAccordionCalendar] = useState(false);

  const [employee, setEmployee] = useState({});

  const [currentPeriod, setCurrentPeriod] = useState({});

  const [summaryDate, setSummaryDate] = useState([]);
  const [summaryPayroll, setSummaryPayroll] = useState([]);

  useEffect(() => { setTitle(`${employee.fullName ? employee.fullName : ""} - Timesheet`) }, [setTitle, employee]);

  const calendarRef = useRef({});
  const [displayCalendar, setDisplayCalendar] = useState(false);
  const [calendarFocusDate, setCalendarFocusDate] = useState("");

  const sort = 15;
  const [data, setData] = useState({
    data: {
      entries: [],
    },
    authorizedActions: [],
  });
  const [paggination, setPagginnation] = useState([]);
  const timesheetData = useRef([]);

  const [employeeComment, setEmployeeComment] = useState("");
  const [supervisorComment, setSupervisorComment] = useState("");

  const [payrollCodes, setPayrollCodes] = useState([]);

  const [openModal, setOpenModal] = useState(false);

  const [selectedTimesheet, setSelectedTimesheet] = useState({});

  const getColor = (e) => {
    switch (e) {
      case "HOLIDAY":
        return "#dde5b6";
      case "UNPAID":
        return "#caf0f8";
      case "SHOP":
        return "#f2cc8f";
      case "VACATION":
        return "#ff9770";
      case "PERSONAL":
        return "#b1a7a6";
      case "JURY_DUTY":
        return "#07b1a6";
      case "BERV":
        return "#ffe66d";
      case "REGULAR":
        return "#00e60d";
      case "OVERTIME":
        return "#e60d00";
      default:
        return "#ode60d";
    }
  };

  const fetchTypes = useCallback(async () => {
    await getPayrollCodes()
    .then((resp) => setPayrollCodes(resp.map((r) => ({ ...r, color: getColor(r.payrollCode) }))))
    .catch((err) => displayError(err, "Error while fetching payroll codes data"))
  }, []);

  const fetchData = useCallback(async () => {
    await getTimesheetEmployeeById(match.params.id, match.params.employee)
      .then((resp) => {
        if (resp.data !== null) {
          let entries = resp?.data?.entries;
          let firstEntry = entries && entries[0];
          setData(resp);
          setEmployeeComment(resp.data.employeeComment);
          setSupervisorComment(resp.data.approverComment);
          setDisplayCalendar(true);
          setCalendarFocusDate(firstEntry.workDate);
        }
      })
      .catch((err) => displayError(err, "Error while fetching employee timesheet details"))
  }, [match.params.id, match.params.employee]);

  const fetchSummaryByDate = useCallback(async () => {
    await getTimesheetEmployeeSummaryByDate( match.params.id, match.params.employee)
    .then((resp) => setSummaryDate(resp))
    .catch((err) => displayError(err, "Error while fetching employee timesheet summary by date"))
  }, [match.params.id, match.params.employee]);

  const fetchSummaryByPayroll = useCallback(async () => {
    await getTimesheetEmployeeSummaryByPayroll( match.params.id, match.params.employee )
    .then((resp) => setSummaryPayroll(resp))
    .catch((err) => displayError(err, "Error while fetching employee timesheet summary by payroll"))
  }, [match.params.id, match.params.employee]);

  const fetchEmployee = useCallback(async () => {
    await getEmployeeById(match.params.employee)
    .then((resp) => setEmployee(resp))
    .catch((err) => displayError(err, "Error while fetching employee details"))
  }, [match.params.employee]);

  const fetchCurrentPeriod = useCallback(async () => {
    await getCurrentEmployeeTimesheetPeriod(match.params.employee)
    .then((resp) => setCurrentPeriod(resp))
    .catch((err) => displayError(err, "Error while fetching current period"))
  }, [match.params.employee]);

  useEffect(() => {
    fetchData();
    fetchEmployee();
    fetchSummaryByDate();
    fetchSummaryByPayroll();
    fetchTypes();
    fetchCurrentPeriod();
  }, [
    match,
    fetchData,
    fetchEmployee,
    fetchSummaryByDate,
    fetchSummaryByPayroll,
    fetchTypes,
    fetchCurrentPeriod
  ]);

  useEffect(() => zoomCalendarOndateIfRequired(), [displayCalendar, calendarFocusDate]);

  useEffect(() => {
    if (data.data.entries) {
      setPagginnation(Array(Math.ceil(data.data.entries.length / sort)).fill().map((_, i) => i + 1));
      timesheetData.current = data.data.entries.slice(activePag.current * sort, (activePag.current + 1) * sort);
    }
  }, [data, timesheetData]);

  const activePag = useRef(0);

  const isSupervisor = () => user.id === employee.primaryApproverId || user.id === employee.secondaryApproverId;

  const isOwner = () => user.id === employee.id;

  const zoomCalendarOndateIfRequired = () => {
    if (displayCalendar && (match.params.date || calendarFocusDate)) {
      let calendarApi = calendarRef.current.getApi();
      let zoomDate = Number(match.params.date) || calendarFocusDate;
      calendarApi.gotoDate(new Date(zoomDate));
    }
  };

  const onClick = (i) => {
    activePag.current = i;
    timesheetData.current = data.data.entries.slice(activePag.current * sort,(activePag.current + 1) * sort);
  };

  const submit = async () => {
    await submitTimesheet({
      employeeId: match.params.employee,
      timesheetId: match.params.id,
      comment: employeeComment,
    })
    .then((resp) => {
      displaySuccess("Employee Timesheet submitted successfully");
      fetchData();
    })
    .catch((err) => displayError(err, "Error while submitting employee timesheet"))
  };

  const approve = async () => {
    await approveTimesheet({
      employeeId: match.params.employee,
      timesheetId: match.params.id,
      comment: supervisorComment,
    })
    .then((resp) => {
      displaySuccess("Employee Timesheet approved successfully");
      fetchData();
    })
    .catch((err) => displayError(err, "Error while approving employee timesheet"))
  };

  const reject = async () => {
    await rejectTimesheet({
      employeeId: match.params.employee,
      timesheetId: match.params.id,
      comment: supervisorComment,
    })
    .then((resp) => {
      displaySuccess("Employee Timesheet rejected successfully");
      fetchData();
    })
    .catch((err) => displayError(err, "Error while rejecting employee timesheet"))
  };

  const delTimesheet = async () => {
    setOpenModal(false);
    await deleteTimesheet()
    .then((resp) => {
      displaySuccess("Employee Timesheet deleted successfully");
      history.reload();
    })
    .catch((err) => displayError(err, "Error while deleting employee timesheet"))
  };

  const updateTimesheetComment = async () => {
    await updateComment({
      employeeId: match.params.employee,
      timesheetId: match.params.id,
      employeeComment: employeeComment,
      approverComment: supervisorComment,
    })
    .then((resp) => {
      displaySuccess("Employee Timesheet comment updated successfully");
      fetchData();
    })
    .catch((err) => displayError(err, "Error while updating employee timesheet comment"))
  };

  const nextButton = () => {
    let calendarApi = calendarRef.current.getApi();
    let nextCalendarFirstDayOfMonth = getFirstDayOfNextMonth(calendarApi.getDate());

    var endDate = data?.data?.endDate;
    var currentTimesheetHaveEntriesOnNextMonth =endDate &&
      new Date(endDate).getMonth() >= nextCalendarFirstDayOfMonth.getMonth() &&
      new Date(endDate).getYear() >= nextCalendarFirstDayOfMonth.getYear();

    if (currentTimesheetHaveEntriesOnNextMonth) {
      calendarApi.next();
      return;
    }

    if (data.data.nextTimesheetId !== null) {
      history.push(`/timesheets/${data.data.nextTimesheetId}/employee/${match.params.employee}`);
    }
  };

  const prevButton = () => {
    let calendarApi = calendarRef.current.getApi();
    let previousCalendarFirstDayOfMonth = getFirstDayOfPreviousMonth(calendarApi.getDate());

    var startDate = data?.data?.endDate;
    var currentTimesheetHaveEntriesOnPreviousMonth = startDate &&
      new Date(startDate).getMonth() <= previousCalendarFirstDayOfMonth.getMonth() &&
      new Date(startDate).getYear() <= previousCalendarFirstDayOfMonth.getYear();

    if (currentTimesheetHaveEntriesOnPreviousMonth) {
      calendarApi.next();
      return;
    }
    if (data.data.previousTimesheetId !== null) {
      history.push(`/timesheets/${data.data.previousTimesheetId}/employee/${match.params.employee}`);
    }
  };

  return loading ? (
    <div style={{ display: "flex", alignItems: "center", alignContent: "center", justifyContent: "center", height: "100%",}}>
      <SpinnerComponent />
    </div>
  ) : (
    <>
      <ConfirmModal action={delTimesheet} close={() => setOpenModal(false)} isOpen={openModal} />
      <div className="card">
        <div className="card-body">
          <div className="custom-tab-1">
            <Row>
              <Col lg={10} />
              <Col style={{ display: "flex", alignItems: "flex-end", justifyContent: "flex-end" }}>
                {data.authorizedActions.find((a) => a.name === "SUBMIT") && (
                  <div className="d-sx-inline-block me-3">
                    <Button  variant="primary" onClick={() => submit()}> Submit </Button>
                  </div>
                )}
                {data.authorizedActions.find((a) => a.name === "APPROVE") && (
                  <div className="d-sx-inline-block me-3">
                    <Button variant="primary" onClick={() => approve()}> Approve </Button>
                  </div>
                )}
                {data.authorizedActions.find((a) => a.name === "REJECT") && (
                  <div className="d-sx-inline-block me-3">
                    <Button variant="danger" onClick={() => reject()}> Reject </Button>
                  </div>
                )}
              </Col>
            </Row>
            <Tab.Container animation={false} defaultActiveKey={"calendar"}>
              <Nav as="ul" className="nav-pills mb-4 justify-content-start">
                <Nav.Item as="li">
                  <Nav.Link onClick={() => calendarRef.current.getApi().changeView("dayGridMonth")} eventKey={"calendar"}>
                    <i className={`la la-calendar me-2`} /> Calendar View
                  </Nav.Link>
                </Nav.Item>
                <Nav.Item as="li">
                  <Nav.Link eventKey={"table"}>
                    <i className={`la la-list me-2`} /> Table View
                  </Nav.Link>
                </Nav.Item>
              </Nav>
              <Tab.Content className="pt-4">
                <Tab.Pane eventKey={"calendar"}>
                  <Row>
                    <Col lg={12}>
                      <Accordion className="accordion accordion-primary">
                        <div className="accordion-item">
                          <Accordion.Toggle
                            as={Card.Text}
                            eventKey="0"
                            className={`accordion-header rounded-lg ${accordionCalendar ? "" : "collapsed"}`}
                            onClick={() => setAccordionCalendar(!accordionCalendar)}
                          >
                            <span className="accordion-header-text">Calendar legend</span>
                            <span className="accordion-header-indicator"></span>
                          </Accordion.Toggle>
                          <Accordion.Collapse eventKey="0">
                            <div className="accordion-body-text">
                              <div className="mx-4 mt-4">
                                <dl>
                                  {payrollCodes.map((t, i) => (
                                    <dt key={i} className="mt-2" style={{ display: "flex" }}>
                                      • {t.payrollCode.replace("_", " ")}{" "}
                                      <div className="mx-2 rounded" style={{ backgroundColor: t.color, width: 50 }}/>
                                    </dt>
                                  ))}
                                </dl>
                              </div>
                            </div>
                          </Accordion.Collapse>
                        </div>
                      </Accordion>
                      {data.data !== null && (
                        <Col>
                          <span>
                            Payroll period ({data.data.payrollPeriod}) :{" "}
                            <span className="font-w600"> {moment(data.data.startDate).format(DefaultGlobalDateFormat)}</span>
                            {" "}-{" "}
                            <span className="font-w600">{moment(data.data.endDate).format(DefaultGlobalDateFormat)}</span>
                          </span>
                          &nbsp;&nbsp;&nbsp;
                          {data?.data?.isFinalized && <span className='text-success'>{data?.data?.statusName?.replace("_", " ")}</span>}
                          {!data?.data?.isFinalized && <span className='text-danger'> {data?.data?.partialStatusName?.replace("_", " ")}</span>}
                        </Col>
                      )}
                      {displayCalendar && (
                        <FullCalendar
                          headerToolbar={{
                            right: "myPrevButton myNextButton",
                          }}
                          ref={calendarRef}
                          initialDate={data.data.startDate}
                          height={600}
                          timeZone="local"
                          rerenderDelay={10}
                          eventDurationEditable={false}
                          editable={true}
                          plugins={[
                            dayGridPlugin,
                            timeGridPlugin,
                            interactionPlugin,
                          ]}
                          customButtons={{
                            myNextButton: {
                              text: "Next",
                              click: function () {
                                nextButton();
                              },
                            },
                            myPrevButton: {
                              text: "Prev",
                              click: function () {
                                prevButton();
                              },
                            },
                          }}
                          events={data.data.entries.map((d, i) => {
                            return {
                              id: i,
                              isRejected: d.isRejected,
                              isApproved: d.isApproved,
                              title: `${d.quantity} h.`,
                              start: moment(d.workDate).format("YYYY-MM-DD"),
                              end: moment(d.workDate).format("YYYY-MM-DD"),
                              backgroundColor: getColor(d.payrollCode),
                            };
                          })}
                          dayCellClassNames = {
                            ({date}) => {
                              let currentDate = new Date(date);
                              if(currentPeriod &&  currentDate >= new Date(currentPeriod.start) && currentDate <= new Date(currentPeriod.end)){
                                return ['bg-light-danger'] 
                              }
                            }
                          }
                          eventRender = {
                            (info) => {
                              const event = info.event;
                              if (event.isRejected) {
                                info.el.querySelector('.fc-event-title').style.textDecoration = 'line-through';
                              }
                              if (event.isApproved) {
                                info.el.querySelector('.fc-event-title').style.fontWeight = 'bold';
                              }
                            }
                          }
                        />
                      )}
                    </Col>
                    <Col lg={12} className="mt-lg-4">
                      <Accordion className="accordion accordion-primary" defaultActiveKey="3">
                        <div className="accordion-item">
                          <Accordion.Toggle as={Card.Text} eventKey="1"
                            className={`accordion-header rounded-lg ${accordionTableCalendar ? "" : "collapsed"}`}
                            onClick={() => setAccordionTableCalendar(!accordionTableCalendar)}
                          >
                            <span className="accordion-header-text">Total Hours : {data.data.totalHours}</span>
                            <span className="accordion-header-indicator"></span>
                          </Accordion.Toggle>
                          <Accordion.Collapse eventKey="1">
                            <div className="accordion-body-text">
                              <div className="mx-4">
                                <dl>
                                  <dt>By Payroll Code</dt>
                                  {summaryPayroll.map((s, i) => (
                                    <dd key={i} className="mx-3" style={{ display: "flex",}}>
                                      ◦{" "}
                                      <div className="mx-2 rounded" style={{backgroundColor: getColor(s.payrollCode ? s.payrollCode : "OTHER"), width: 100,textAlign: "center" }}>
                                        {s.payrollCode}
                                      </div>
                                      - total hours: {s.hours}
                                    </dd>
                                  ))}
                                  <dt>By Date</dt>
                                  {summaryDate.map((s, i) => (
                                    <dd key={i} className="mx-3"style={{display: "flex" }}>
                                      •{" "}
                                      <div className="mx-2 rounded" style={{ backgroundColor: getColor(s.payrollCode? s.payrollCode: "OTHER"), width: 100, textAlign: "center", }}>
                                        {moment(s.workDate).format("DD/MM/YYYY")}
                                      </div>
                                      - total hours: {s.hours}
                                    </dd>
                                  ))}
                                </dl>
                              </div>
                            </div>
                          </Accordion.Collapse>
                        </div>
                      </Accordion>
                    </Col>
                    <Row className="mt-lg-4">
                      <Col lg={6} className="mt-lg-2">
                        <label>Employee Comment</label>
                        <textarea
                          className="form-control"
                          rows="4"
                          id="comment"
                          disabled={!(isOwner() && !data.data.isFinalized)}
                          value={employeeComment || ""}
                          onChange={(e) => setEmployeeComment(e.target.value)}
                        />
                        {isOwner() && !data.data.isFinalized && (
                          <div className="col-xl-12 col-lg-12 mt-4" style={{display: "flex",alignItems: "flex-end",justifyContent: "flex-end",}}>
                            <Button variant="danger" size="sm" onClick={() => updateTimesheetComment()}>Update</Button>
                          </div>
                        )}
                      </Col>
                      <Col lg={6} className="mt-lg-2">
                        <label>Supervisor Comment</label>
                        <textarea
                          className="form-control"
                          rows="4"
                          id="comment"
                          value={supervisorComment || ""}
                          onChange={(e) => setSupervisorComment(e.target.value)}
                          disabled={!(isSupervisor() && !data.data.isFinalized)}
                        />
                        {isSupervisor() && !data.data.isFinalized && (
                            <div className="col-xl-12 col-lg-12 mt-4" style={{ display: "flex", alignItems: "flex-end", justifyContent: "flex-end", }}>
                              <Button variant="danger" size="sm" onClick={() => updateTimesheetComment()}>Update</Button>
                            </div>
                          )}
                      </Col>
                    </Row>
                  </Row>
                </Tab.Pane>
                <Tab.Pane eventKey={"table"}>
                  <Table responsive striped className="mt-3">
                    <thead>
                      <tr>
                        <th style={{ textAlign: "center" }}></th>
                        <th style={{ textAlign: "center" }}>Work Date</th>
                        <th style={{ textAlign: "center" }}>Payroll Code</th>
                        <th style={{ textAlign: "center" }}>Quantity</th>
                        <th style={{ textAlign: "center" }}>Job No</th>
                        <th style={{ textAlign: "center" }}>Job Task No</th>
                        <th style={{ textAlign: "center" }}>
                          Service Order No
                        </th>
                        <th style={{ textAlign: "center" }}>Profit Center</th>
                        <th style={{ textAlign: "center" }}>Description</th>
                      </tr>
                    </thead>
                    <tbody>
                      {timesheetData.current.map((t, i) => (
                        <tr key={t.id} style={{ 
                          textDecoration: t.isRejected ? "line-through" : "none",
                          fontWeight: t.isApproved ? "bold" : "normal"
                        }}>
                          <td>
                            {t.isDeletable && (
                              <i style={{ textAlign: "center", cursor: "pointer",}}
                                onClick={() => {
                                  setSelectedTimesheet({
                                    employeeId: match.params.employee,
                                    timesheetId: match.params.id,
                                    timesheetEntryId: t.id,
                                  });
                                  setOpenModal(true);
                                }}
                                className="flaticon-381-multiply-1 text-danger"
                              ></i>
                            )}
                          </td>
                          <td>{moment(t.workDate).format(DefaultGlobalDateFormat)}</td>
                          <td>{t.payrollCode}</td>
                          <td>{t.quantity}</td>
                          <td>{t.jobNumber}</td>
                          <td>{t.jobTaskNumber}</td>
                          <td>{t.serviceOrderNumber}</td>
                          <td>{t.profitCenterNumber}</td>
                          <td>{t.description}</td>
                        </tr>
                      ))}
                    </tbody>
                  </Table>
                  <div id="example_wrapper" className="dataTables_wrapper">
                    <div className="d-sm-flex text-center justify-content-between align-items-center mt-1">
                      <div className="dataTables_info" />
                      <Pagination size={"sx"} className={`pagination-gutter pagination- pagination-circle`}>
                        <li className="page-item page-indicator">
                          <Link className="page-link" to="#" onClick={() => activePag.current > 0 && onClick(activePag.current - 1)}>
                            <i className="la la-angle-left" />
                          </Link>
                        </li>
                        {paggination.map((number, i) => (
                          <Pagination.Item key={number} active={activePag.current === i}>{number}</Pagination.Item>
                        ))}
                        <li className="page-item page-indicator">
                          <Link className="page-link" to="#" onClick={() => activePag.current + 1 < paggination.length && onClick(activePag.current + 1)}>
                            <i className="la la-angle-right" />
                          </Link>
                        </li>
                      </Pagination>
                    </div>
                  </div>
                  <Accordion className="accordion accordion-primary" defaultActiveKey="3">
                    <div className="accordion-item">
                      <Accordion.Toggle
                        as={Card.Text}
                        eventKey="5"
                        className={`accordion-header rounded-lg mt-3 ${accordionTable ? "" : "collapsed"}`}
                        onClick={() => setAccordionTable(!accordionTable)}
                      >
                        <span className="accordion-header-text">
                          Total Hours : {data.data.totalHours}
                        </span>
                        <span className="accordion-header-indicator"></span>
                      </Accordion.Toggle>
                      <Accordion.Collapse eventKey="5">
                        <div className="accordion-body-text">
                          <div className="mx-4">
                            <dl>
                              <dt>By Payroll Code</dt>
                              {summaryPayroll.map((s, i) => (
                                <dd key={i} className="mx-3" style={{display: "flex",}}
                                >
                                  ◦{" "}
                                  <div className="mx-2 rounded"
                                    style={{ 
                                      backgroundColor: getColor( s.payrollCode ? s.payrollCode : "OTHER"),
                                      width: 100,
                                      textAlign: "center",
                                    }}
                                  >
                                    {s.payrollCode}
                                  </div>
                                  - total hours: {s.hours}
                                </dd>
                              ))}
                              <dt>By Date</dt>
                              {summaryDate.map((s, i) => (
                                <dd key={i} className="mx-3" style={{ display: "flex",}}>
                                  •{" "}
                                  <div
                                    className="mx-2 rounded"
                                    style={{ 
                                      backgroundColor: getColor(s.payrollCode ? s.payrollCode : "OTHER"),
                                      width: 100,
                                      textAlign: "center",
                                    }}
                                  >
                                    {s.payrollCode}
                                  </div>{" "}
                                  - total hours: {s.hours}
                                </dd>
                              ))}
                            </dl>
                          </div>
                        </div>
                      </Accordion.Collapse>
                    </div>
                  </Accordion>
                  <Row className="mt-lg-4">
                    <Col lg={6} className="mt-lg-2">
                      <label>Employee Comment</label>
                      <textarea
                        className="form-control"
                        rows="4"
                        id="comment"
                        value={employeeComment || ""}
                        disabled={!(isOwner() && !data.data.isFinalized)}
                        onChange={(e) => setEmployeeComment(e.target.value)}
                      />
                      {isOwner() && !data.data.isFinalized && (
                        <div
                          className="col-xl-12 col-lg-12 mt-4"
                          style={{display: "flex", alignItems: "flex-end", justifyContent: "flex-end",}}
                        >
                          <Button variant="danger" size="sm" onClick={() => updateTimesheetComment()}>Update</Button>
                        </div>
                      )}
                    </Col>
                    <Col lg={6} className="mt-lg-2">
                      <label>Supervisor Comment</label>
                      <textarea
                        className="form-control"
                        rows="4"
                        id="comment"
                        value={supervisorComment || ""}
                        disabled={ !(isSupervisor() && !data.data.isFinalized) }
                        onChange={(e) => setSupervisorComment(e.target.value)}
                      />
                      {isSupervisor() && !data.data.isFinalized && (
                        <div
                          className="col-xl-12 col-lg-12 mt-4"
                          style={{ display: "flex", alignItems: "flex-end", justifyContent: "flex-end", }}
                        >
                          <Button variant="danger" size="sm" onClick={() => updateTimesheetComment()}>Update</Button>
                        </div>
                      )}
                    </Col>
                  </Row>
                </Tab.Pane>
              </Tab.Content>
            </Tab.Container>
          </div>
        </div>
      </div>
    </>
  );
};

export default Timesheet;
