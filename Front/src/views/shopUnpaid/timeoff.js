import React, {
  createRef,
  Fragment,
  useCallback,
  useContext,
  useEffect,
  useRef,
} from "react";
import FullCalendar from "@fullcalendar/react";
import dayGridPlugin from "@fullcalendar/daygrid";
import timeGridPlugin from "@fullcalendar/timegrid";
import interactionPlugin, { Draggable } from "@fullcalendar/interaction";
import {
  Accordion,
  Badge,
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
import _ from "lodash";
import { ThemeContext } from "../../context/themeContext";
import {
  addEntry,
  approveTimeOff,
  deleteEntry,
  deleteTimeoff,
  getTimeoffEmployeeDetails,
  getTimeoffEmployeeSummary,
  rejectTimeOff,
  submitTimeoff,
  updateComment,
  updateEntry,
} from "../../redux/actions/timesoffs";
import { getEmployeeById } from "../../redux/actions/employees";
import { toast } from "react-toastify";
import NewEntry from "./newEntry";
import ConfirmModal from "./confirmModal";
import {
  getTimeoffLabels,
  getNonRegularTimeoffTypes,
} from "../../redux/actions/referentials";
import SpinnerComponent from "../../components/spinner/spinner";
import { useSelector } from "react-redux";

const TimeOff = ({ match, history }) => {
  const { setTitle } = useContext(ThemeContext);
  const loading = useSelector((state) => state.auth.showLoading);

  const [accordionTable, setAccordionTable] = useState(false);
  const [accordionTableCalendar, setAccordionTableCalendar] = useState(false);
  const [accordionCalendar, setAccordionCalendar] = useState(false);
  const [summary, setSummary] = useState([]);
  const [employee, setEmployee] = useState({});
  const [employeeComment, setEmployeeComment] = useState("");
  const [supervisorComment, setSupervisorComment] = useState("");

  const user = useSelector((state) => state.auth.auth);

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
  const timeoffData = useRef([]);

  const [entry, setEntry] = useState({
    date: new Date(),
  });
  const [isOpenEntry, setIsOpenEntry] = useState(false);

  const [openModal, setOpenModal] = useState(false);

  const [types, setTypes] = useState([]);
  const [labels, setLabels] = useState([]);

  useEffect(() => {
    setTitle(`${employee.fullName ? employee.fullName : ""} - Timeoff`);
  }, [setTitle, employee]);

  useEffect(() => {
    zoomCalendarOndateIfRequired();
  }, [displayCalendar, calendarFocusDate]);

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
      default:
        return "#ffe66d";
    }
  };

  const fetchTypes = useCallback(async () => {
    await getNonRegularTimeoffTypes()
      .then((resp) => {
        setTypes(
          resp.map((r) => {
            return {
              ...r,
              color: getColor(r.payrollCode),
            };
          })
        );
      })
      .catch((err) => {
        toast.error(
          err.response.data.message
            ? err.response.data.message
            : "API connexion denied",
          {
            position: "top-right",
            autoClose: 5000,
            hideProgressBar: false,
            closeOnClick: true,
            pauseOnHover: true,
            draggable: true,
            progress: undefined,
          }
        );
      });
  }, []);

  const fetchLabels = async () => {
    await getTimeoffLabels()
      .then((resp) => {
        setLabels(
          resp.map((d) => {
            return {
              value: d,
              label: d,
            };
          })
        );
      })
      .catch((err) => {
        toast.error(
          err.response.data.message
            ? err.response.data.message
            : "API connexion denied",
          {
            position: "top-right",
            autoClose: 5000,
            hideProgressBar: false,
            closeOnClick: true,
            pauseOnHover: true,
            draggable: true,
            progress: undefined,
          }
        );
      });
  };

  const fetchData = useCallback(async () => {
    await getTimeoffEmployeeDetails(match.params.employee, match.params.id)
      .then((resp) => {
        let entries = resp?.data?.entries;
        let firstEntry = entries && entries[0];
        setData(resp);
        setEmployeeComment(resp.data.employeeComment);
        setSupervisorComment(resp.data.approverComment);
        setDisplayCalendar(true);
        setCalendarFocusDate(firstEntry.requestDate);
      })
      .catch((err) => {
        console.log(err);
        toast.error(
          err?.response?.data?.message
            ? err.response.data.message
            : "Something wrong happened! We are investigating"
        );
      });
    await getTimeoffEmployeeSummary(match.params.employee, match.params.id)
      .then((resp) => {
        setSummary(resp);
      })
      .catch((err) => {
        toast.error(
          err?.response?.data?.message
            ? err.response.data.message
            : "Something wrong happened! We are investigating"
        );
      });
  }, [match.params.id, match.params.employee]);

  const fetchEmployee = useCallback(async () => {
    await getEmployeeById(match.params.employee)
      .then((resp) => {
        setEmployee(resp);
      })
      .catch((err) => {
        toast.error(
          err?.response?.data?.message
            ? err.response.data.message
            : "Something wrong happened! We are investigating"
        );
      });
  }, [match.params.employee]);

  useEffect(() => {
    fetchData();
    fetchEmployee();
    fetchTypes();
    fetchLabels();
  }, [fetchData, fetchEmployee, fetchTypes]);

  useEffect(() => {
    if (data.data && data.data.entries) {
      setPagginnation(
        Array(Math.ceil(data.data.entries.length / sort))
          .fill()
          .map((_, i) => i + 1)
      );
      timeoffData.current = data.data.entries.slice(
        activePag.current * sort,
        (activePag.current + 1) * sort
      );
    }
  }, [data, timeoffData]);

  const activePag = useRef(0);

  const zoomCalendarOndateIfRequired = () => {
    if (displayCalendar && (match.params.date || calendarFocusDate)) {
      let calendarApi = calendarRef.current.getApi();
      let zoomDate = Number(match.params.date) || calendarFocusDate;
      calendarApi.gotoDate(new Date(zoomDate));
    }
  };

  const onClick = (i) => {
    activePag.current = i;

    timeoffData.current = data.slice(
      activePag.current * sort,
      (activePag.current + 1) * sort
    );
  };

  const submit = async () => {
    await submitTimeoff({
      employeeId: match.params.employee,
      timeoffId: match.params.id,
      comment: employeeComment,
    })
      .then((res) => {
        toast.success("Successful submit", {
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
            : "Error submit",
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

  const saveEntry = async () => {
    if (entry.timeoffEntryId) {
      await updateEntry({
        employeeId: match.params.employee,
        timeoffId: match.params.id,
        type: entry.type.value,
        hours: entry.hours,
        timeoffEntryId: entry.timeoffEntryId,
      })
        .then((res) => {
          setIsOpenEntry(false);
          toast.success("Successful updating", {
            position: "top-right",
            autoClose: 5000,
            hideProgressBar: false,
            closeOnClick: true,
            pauseOnHover: true,
            draggable: true,
          });
          fetchData();
          fetchLabels();
        })
        .catch((err) => {
          toast.error(
            err.response.data.message
              ? err.response.data.message
              : "Error updating",
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
    } else {
      await addEntry({
        employeeId: match.params.employee,
        timeoffId: match.params.id,
        requestDate: entry.date,
        type: entry.type.value,
        hours: entry.hours,
      })
        .then((res) => {
          setIsOpenEntry(false);
          toast.success("Successful add", {
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
            err.response.data.message ? err.response.data.message : "Error add",
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
    }
  };

  const delEntry = async (id) => {
    await deleteEntry(match.params.id, id, match.params.employee)
      .then((resp) => {
        toast.success("Successful deletion", {
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
            : "Deletion error",
          {
            position: "top-right",
            autoClose: 5000,
            hideProgressBar: false,
            closeOnClick: true,
            pauseOnHover: true,
            draggable: true,
            progress: undefined,
          }
        );
      });
  };

  const delTimeOff = async () => {
    setOpenModal(false);
    await deleteTimeoff(match.params.id, match.params.employee)
      .then((resp) => {
        toast.success("Successful deletion", {
          position: "top-right",
          autoClose: 5000,
          hideProgressBar: false,
          closeOnClick: true,
          pauseOnHover: true,
          draggable: true,
        });
        history.push("/timesheets");
      })
      .catch((err) => {
        toast.error(
          err.response.data.message
            ? err.response.data.message
            : "Deletion error",
          {
            position: "top-right",
            autoClose: 5000,
            hideProgressBar: false,
            closeOnClick: true,
            pauseOnHover: true,
            draggable: true,
            progress: undefined,
          }
        );
      });
  };

  const approve = async () => {
    await approveTimeOff({
      employeeId: match.params.employee,
      timeoffId: match.params.id,
      comment: supervisorComment,
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

  const reject = async () => {
    await rejectTimeOff({
      employeeId: match.params.employee,
      timeoffId: match.params.id,
      comment: supervisorComment,
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

  const updateTimeoffComment = async () => {
    await updateComment({
      employeeId: match.params.employee,
      timeoffId: match.params.id,
      employeeComment: employeeComment,
      approverComment: supervisorComment,
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

  const isSupervisor = () => {
    return (
      user.id === employee.primaryApproverId ||
      user.id === employee.secondaryApproverId
    );
  };

  const isOwner = () => {
    return user.id === employee.id;
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
      <ConfirmModal
        action={delTimeOff}
        close={() => setOpenModal(false)}
        isOpen={openModal}
      />
      <NewEntry
        data={entry}
        updateData={setEntry}
        isOpen={isOpenEntry}
        close={() => {
          setEntry({ date: new Date() });
          setIsOpenEntry(false);
        }}
        save={() => saveEntry()}
        types={types}
        labels={labels}
      />
      <div className="card">
        <div className="card-body">
          <div className="custom-tab-1">
            <Row>
              <Col lg={10} />
              <Col
                style={{
                  display: "flex",
                  alignItems: "flex-end",
                  justifyContent: "flex-end",
                }}
              >
                {data.authorizedActions.find((a) => a.name === "ADD_ENTRY") && (
                  <div className="d-sx-inline-block me-3">
                    <Button
                      variant="primary"
                      onClick={() => setIsOpenEntry(true)}
                    >
                      Add Entry
                    </Button>
                  </div>
                )}
                {data.authorizedActions.find((a) => a.name === "SUBMIT") && (
                  <div className="d-sx-inline-block me-3">
                    <Button variant="warning" onClick={() => submit()}>
                      Submit
                    </Button>
                  </div>
                )}
                {data.authorizedActions.find((a) => a.name === "APPROVE") && (
                  <div className="d-sx-inline-block me-3">
                    <Button variant="primary" onClick={() => approve()}>
                      Approve
                    </Button>
                  </div>
                )}
                {data.authorizedActions.find((a) => a.name === "REJECT") && (
                  <div className="d-sx-inline-block me-3">
                    <Button variant="danger" onClick={() => reject()}>
                      Reject
                    </Button>
                  </div>
                )}
                {data.authorizedActions.find((a) => a.name === "DELETE") && (
                  <div className="d-sx-inline-block">
                    <Button variant="danger" onClick={() => setOpenModal(true)}>
                      Delete
                    </Button>
                  </div>
                )}
              </Col>
            </Row>
            <Tab.Container animation={false} defaultActiveKey={"calendar"}>
              <Nav as="ul" className="nav-pills mb-4 justify-content-start">
                <Nav.Item as="li">
                  <Nav.Link
                    onClick={() => {
                      calendarRef.current.getApi().changeView("dayGridMonth");
                    }}
                    eventKey={"calendar"}
                  >
                    <i className={`la la-calendar me-2`} />
                    Calendar View
                  </Nav.Link>
                </Nav.Item>
                <Nav.Item as="li">
                  <Nav.Link eventKey={"table"}>
                    <i className={`la la-list me-2`} />
                    Table View
                  </Nav.Link>
                </Nav.Item>
              </Nav>
              <Tab.Content className="pt-4">
                <Tab.Pane eventKey={"calendar"}>
                  <Row>
                    <Col lg={12}>
                      <Accordion
                        className="accordion accordion-primary"
                        defaultActiveKey="3"
                      >
                        <div className="accordion-item">
                          <Accordion.Toggle
                            as={Card.Text}
                            eventKey="0"
                            className={`accordion-header rounded-lg ${
                              accordionCalendar ? "" : "collapsed"
                            }`}
                            onClick={() =>
                              setAccordionCalendar(!accordionCalendar)
                            }
                          >
                            <span className="accordion-header-text">
                              Calendar legend
                            </span>
                            <span className="accordion-header-indicator"></span>
                          </Accordion.Toggle>
                          <Accordion.Collapse eventKey="0">
                            <div className="accordion-body-text">
                              <div className="mx-4 mt-4">
                                <dl>
                                  {types.map((t) => (
                                    <dt
                                      className="mt-2"
                                      style={{
                                        display: "flex",
                                      }}
                                    >
                                      • {t.payrollCode.replace("_", " ")}{" "}
                                      <div
                                        className="mx-2 rounded"
                                        style={{
                                          backgroundColor: t.color,
                                          width: 50,
                                        }}
                                      />
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
                            Timeoff period :{" "}
                            <span class="font-w600">
                              {moment(data.data.requestStartDate).format(
                                "MM/DD/YYYY"
                              )}
                            </span>{" "}
                            -{" "}
                            <span class="font-w600">
                              {moment(data.data.requestEndDate).format(
                                "MM/DD/YYYY"
                              )}
                            </span>
                          </span>
                          &nbsp;&nbsp;&nbsp;
                          <button
                            className={`btn btn-xs  ${
                              data.data.statusName !== "APPROVED"
                                ? "btn-outline-danger"
                                : "btn-outline-success"
                            }`}
                          >
                            {data.data.statusName}
                          </button>
                        </Col>
                      )}
                      {displayCalendar && (
                        <FullCalendar
                          ref={calendarRef}
                          defaultView="dayGridMonth"
                          header={{
                            left: "prev,next today",
                            center: "title",
                            right:
                              "dayGridMonth,timeGridWeek,timeGridDay,listWeek",
                          }}
                          height={600}
                          rerenderDelay={10}
                          eventDurationEditable={false}
                          editable={true}
                          plugins={[
                            dayGridPlugin,
                            timeGridPlugin,
                            interactionPlugin,
                          ]}
                          events={timeoffData.current.map((d, i) => {
                            return {
                              id: i,
                              title: d.hours,
                              start: moment(d.requestDate).format("YYYY-MM-DD"),
                              end: moment(d.requestDate).format("YYYY-MM-DD"),
                              backgroundColor: getColor(d.payrollCode),
                            };
                          })}
                        />
                      )}
                    </Col>
                    <Col lg={12} className="mt-lg-4">
                      <Accordion
                        className="accordion accordion-primary"
                        defaultActiveKey="16"
                      >
                        <div className="accordion-item">
                          <Accordion.Toggle
                            as={Card.Text}
                            eventKey="13"
                            className={`accordion-header rounded-lg ${
                              accordionTableCalendar ? "" : "collapsed"
                            }`}
                            onClick={() =>
                              setAccordionTableCalendar(!accordionTableCalendar)
                            }
                          >
                            <span className="accordion-header-text">
                              Total Hours : {data.data.totalHours}
                            </span>
                            <span className="accordion-header-indicator"></span>
                          </Accordion.Toggle>
                          <Accordion.Collapse eventKey="13">
                            <div className="accordion-body-text">
                              <div className="mx-4">
                                <dl>
                                  {summary.map((s) => (
                                    <>
                                      <dt>
                                        •{" "}
                                        {moment(s.requestDate).format(
                                          "MMM DD, YYYY"
                                        )}{" "}
                                        - total hours : {s.hours}
                                      </dt>
                                      <dd
                                        className="mx-3"
                                        style={{
                                          display: "flex",
                                        }}
                                      >
                                        <div
                                          className="mx-2 rounded"
                                          style={{
                                            backgroundColor: getColor(
                                              s.payrollCode
                                                ? s.payrollCode
                                                : "OTHER"
                                            ),
                                            width: 50,
                                          }}
                                        />
                                        {s.payrollCode} : {s.hours}
                                      </dd>
                                    </>
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
                          value={employeeComment || ""}
                          disabled={
                            !isOwner() && data.data.statusName !== "APPROVED"
                          }
                          onChange={(e) => setEmployeeComment(e.target.value)}
                        ></textarea>
                        {isOwner() && data.data.statusName !== "APPROVED" && (
                          <div
                            className="col-xl-12 col-lg-12 mt-4"
                            style={{
                              display: "flex",
                              alignItems: "flex-end",
                              justifyContent: "flex-end",
                            }}
                          >
                            <Button
                              variant="danger"
                              size="sm"
                              onClick={() => updateTimeoffComment()}
                            >
                              Update
                            </Button>
                          </div>
                        )}
                      </Col>
                      <Col lg={6} className="mt-lg-2">
                        <label>Supervisor Comment</label>
                        <textarea
                          className="form-control"
                          rows="4"
                          id="comment"
                          disabled={
                            !isSupervisor() &&
                            data.data.statusName !== "APPROVED"
                          }
                          value={supervisorComment || ""}
                          onChange={(e) => setSupervisorComment(e.target.value)}
                        ></textarea>
                        {isSupervisor() &&
                          data.data.statusName !== "APPROVED" && (
                            <div
                              className="col-xl-12 col-lg-12 mt-4"
                              style={{
                                display: "flex",
                                alignItems: "flex-end",
                                justifyContent: "flex-end",
                              }}
                            >
                              <Button
                                variant="danger"
                                size="sm"
                                onClick={() => updateTimeoffComment()}
                              >
                                Update
                              </Button>
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
                        <th style={{ textAlign: "center" }}>Action</th>
                        <th style={{ textAlign: "center" }}>Request Date</th>
                        <th style={{ textAlign: "center" }}>Hours</th>
                        <th style={{ textAlign: "center" }}>Label</th>
                        <th style={{ textAlign: "center" }}>Payroll Code</th>
                      </tr>
                    </thead>
                    <tbody>
                      {timeoffData.current.map((d, i) => (
                        <tr key={i}>
                          {data.authorizedActions.length > 0 ? (
                            <td
                              style={{
                                textAlign: "center",
                                display: "flex",
                                alignItems: "center",
                                justifyContent: "center",
                              }}
                            >
                              {(isOwner() || user.isAdministrator) && (
                                <>
                                  <div style={{ cursor: "pointer" }}>
                                    <i
                                      className="flaticon-381-multiply-1 text-danger"
                                      onClick={() => delEntry(d.id)}
                                    />
                                  </div>

                                  <div
                                    className="mx-2"
                                    style={{ cursor: "pointer" }}
                                  >
                                    <i
                                      class="fas fa-pen"
                                      onClick={() => {
                                        setEntry({
                                          timeoffEntryId: d.id,
                                          type: d.type,
                                          hours: d.hours,
                                        });
                                        setIsOpenEntry(true);
                                      }}
                                    />
                                  </div>
                                </>
                              )}
                            </td>
                          ) : (
                            <td></td>
                          )}
                          <td style={{ textAlign: "center" }}>
                            {moment(d.requestDate).format("MM/DD/YYYY")}
                          </td>
                          <td style={{ textAlign: "center" }}>{d.hours}</td>
                          <td style={{ textAlign: "center" }}>{d.label}</td>
                          <td style={{ textAlign: "center" }}>
                            {d.payrollCode}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </Table>
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
                            onClick={() =>
                              activePag.current > 0 &&
                              onClick(activePag.current - 1)
                            }
                          >
                            <i className="la la-angle-left" />
                          </Link>
                        </li>
                        {paggination.map((number, i) => (
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
                              activePag.current + 1 < paggination.length &&
                              onClick(activePag.current + 1)
                            }
                          >
                            <i className="la la-angle-right" />
                          </Link>
                        </li>
                      </Pagination>
                    </div>
                  </div>
                  <Accordion
                    className="accordion accordion-primary"
                    defaultActiveKey="12"
                  >
                    <div className="accordion-item">
                      <Accordion.Toggle
                        as={Card.Text}
                        eventKey="12"
                        className={`accordion-header rounded-lg mt-3 ${
                          !accordionTable ? "" : "collapsed"
                        }`}
                        onClick={() => setAccordionTable(!accordionTable)}
                      >
                        <span className="accordion-header-text">
                          Total Hours : {data.data.totalHours}
                        </span>
                        <span className="accordion-header-indicator"></span>
                      </Accordion.Toggle>
                      <Accordion.Collapse eventKey="12">
                        <div className="accordion-body-text">
                          <div className="mx-4">
                            <dl>
                              {summary.map((s) => (
                                <>
                                  <dt>
                                    •{" "}
                                    {moment(s.requestDate).format(
                                      "MMM DD, YYYY"
                                    )}{" "}
                                    - total hours : {s.hours}
                                  </dt>
                                  <dd
                                    className="mx-3"
                                    style={{
                                      display: "flex",
                                    }}
                                  >
                                    <div
                                      className="mx-2 rounded"
                                      style={{
                                        backgroundColor: getColor(
                                          s.payrollCode
                                            ? s.payrollCode
                                            : "OTHER"
                                        ),
                                        width: 50,
                                      }}
                                    />
                                    {s.payrollCode} : {s.hours}
                                  </dd>
                                </>
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
                        disabled={
                          !isOwner() && data.data.statusName !== "APPROVED"
                        }
                        value={employeeComment || ""}
                        onChange={(e) => setEmployeeComment(e.target.value)}
                      ></textarea>
                      {isOwner() && data.data.statusName !== "APPROVED" && (
                        <div
                          className="col-xl-12 col-lg-12 mt-4"
                          style={{
                            display: "flex",
                            alignItems: "flex-end",
                            justifyContent: "flex-end",
                          }}
                        >
                          <Button
                            variant="danger"
                            size="sm"
                            onClick={() => updateTimeoffComment()}
                          >
                            Update
                          </Button>
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
                        disabled={
                          !isSupervisor() && data.data.statusName !== "APPROVED"
                        }
                        onChange={(e) => setSupervisorComment(e.target.value)}
                      ></textarea>
                      {isSupervisor() &&
                        data.data.statusName !== "APPROVED" && (
                          <div
                            className="col-xl-12 col-lg-12 mt-4"
                            style={{
                              display: "flex",
                              alignItems: "flex-end",
                              justifyContent: "flex-end",
                            }}
                          >
                            <Button
                              variant="danger"
                              size="sm"
                              onClick={() => updateTimeoffComment()}
                            >
                              Update
                            </Button>
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

export default TimeOff;
