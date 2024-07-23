import React, { useCallback, useContext, useEffect, useRef,} from "react";
import {
  Accordion,
  Button,
  Card,
  Col,
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
import NewEntry from "./newEntry";
import ConfirmModal from "./confirmModal";
import SpinnerComponent from "../../components/spinner/spinner";
import { useSelector } from "react-redux";
import { displayError, displaySuccess } from "../../services/toast";


const Timeoff = ({ match, history }) => {
  const { setTitle } = useContext(ThemeContext);
  const loading = useSelector((state) => state.auth.showLoading);

  const [accordionTable, setAccordionTable] = useState(false);
  const [summary, setSummary] = useState([]);
  const [employee, setEmployee] = useState({});
  const [employeeComment, setEmployeeComment] = useState("");
  const [supervisorComment, setSupervisorComment] = useState("");

  const user = useSelector((state) => state.auth.auth);

  const sort = 15;
  const [data, setData] = useState({
    data: {
      entries: [],
    },
    authorizedActions: [],
  });
  const [paggination, setPaginnation] = useState([]);
  const timeoffData = useRef([]);

  const [entry, setEntry] = useState({
    date: new Date(),
  });
  const [isOpenEntry, setIsOpenEntry] = useState(false);

  const [openModal, setOpenModal] = useState(false);
  const [modalAction, setModalAction] = useState();

  useEffect(() => {
    setTitle(`${employee.fullName ? employee.fullName : ""}`);
  }, [setTitle, employee]);

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

  const fetchEmployee = useCallback(async () => {
    await getEmployeeById(match.params.employee)
      .then((resp) => setEmployee(resp))
      .catch((err) => displayError(err, "Error while fetching employee Details"));
  }, [match.params.employee]);

  const fetchData = useCallback(async () => {
    await getTimeoffEmployeeDetails(match.params.employee, match.params.id)
    .then((resp) => {
      setData(resp);
      setEmployeeComment(resp.data.employeeComment);
      setSupervisorComment(resp.data.approverComment);
    })
    .catch((err) => displayError(err, "Error while fetching time off details"));

    await getTimeoffEmployeeSummary(match.params.employee, match.params.id)
    .then((resp) => setSummary(resp))
    .catch((err) => displayError(err, "Error while fetching time off summary"));
  }, [match.params.id, match.params.employee]);

  useEffect(() => {
    fetchData();
    fetchEmployee();
    fetchEmployee();
  }, [fetchData, fetchEmployee, fetchEmployee]);

  useEffect(() => {
    if (data.data && data.data.entries) {
      let entries = data.data.entries;
      setPaginnation(Array(Math.ceil(entries.length / sort)).fill().map((_, i) => i + 1));
      timeoffData.current = entries.slice(activePag.current * sort, (activePag.current + 1) * sort);
    }
  }, [data, timeoffData]);

  const activePag = useRef(0);

  const onClick = (i) => {
    activePag.current = i;
    timeoffData.current = data.slice( i * sort, (i + 1) * sort);
  };

  const submit = async () => {
    await submitTimeoff({
      employeeId: match.params.employee,
      timeoffId: match.params.id,
      comment: employeeComment,
    })
    .then((res) => {
      displaySuccess("Time off submitted")
      fetchData();
    })
    .catch((err) => displayError(err, "Error while submitting time off summary"));
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
        displaySuccess("Time off entry updated")
        fetchData();
      })
      .catch((err) => displayError(err, "Error while updating time off entry"));
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
        displaySuccess("Time off entry added")
        fetchData();
      })
      .catch((err) => displayError(err, "Error while adding time off entry"));
    }
  };

  const delEntry = async (id) => {
    setOpenModal(false);
    await deleteEntry(match.params.id, id, match.params.employee)
    .then((resp) => {
      displaySuccess("Time off entry deleted")
      fetchData();
      resp.timeoffDeleted && history.push("/dashboard");
    })
    .catch((err) => displayError(err, "Error while deleting time off entry"));
  };

  const delTimeOff = async () => {
    setOpenModal(false);
    await deleteTimeoff(match.params.id, match.params.employee)
    .then((resp) => {
      displaySuccess("Time off deleted")
      history.push("/dashboard");
    })
    .catch((err) => displayError(err, "Error while deleting time off"));
  };

  const approve = async () => {
    await approveTimeOff({
      employeeId: match.params.employee,
      timeoffId: match.params.id,
      comment: supervisorComment,
    })
    .then((res) => {
      displaySuccess("Time off approved")
      fetchData();
    })
    .catch((err) => displayError(err, "Error while approving time off"));
  };

  const reject = async () => {
    await rejectTimeOff({
      employeeId: match.params.employee,
      timeoffId: match.params.id,
      comment: supervisorComment,
    })
    .then((res) => {
      displaySuccess("Time off rejected")
      fetchData();
    })
    .catch((err) => displayError(err, "Error while rejecting time off"));
  };

  const updateTimeoffComment = async () => {
    await updateComment({
      employeeId: match.params.employee,
      timeoffId: match.params.id,
      employeeComment: employeeComment,
      approverComment: supervisorComment,
    })
    .then((res) => {
      displaySuccess("Time off comment updated")
      fetchData();
    })
    .catch((err) => displayError(err, "Error while updating time off comment"));
  };

  const isSupervisor = () => user.id === employee.primaryApproverId || user.id === employee.secondaryApproverId;

  const isOwner = () => user.id === employee.id;;

  return loading ? (
    <div
      style={{ display: "flex", alignItems: "center", alignContent: "center", justifyContent: "center", height: "100%" }}
    >
      <SpinnerComponent />
    </div>
  ) : (
    <>
      <ConfirmModal action={modalAction} close={() => setOpenModal(false)} isOpen={openModal} />
      <NewEntry
        data={entry}
        updateData={setEntry}
        isOpen={isOpenEntry}
        close={() => {
          setEntry({ date: new Date() });
          setIsOpenEntry(false);
        }}
        save={() => saveEntry()}
      />
      <div className="card">
        <div className="card-body">
          <div className="custom-tab-1">
            <Row>
              <Col lg={10} />
              <Col style={{ display: "flex", alignItems: "flex-end", justifyContent: "flex-end",}}>
                <div className="d-inline-block me-3">
                  <Link to={`/timeoffs/${employee.id}`}>All Requests</Link>
                </div>
                {data.authorizedActions.find((a) => a.name === "ADD_ENTRY") && (
                  <div className="d-inline-block me-3">
                    <Button variant="primary" onClick={() => setIsOpenEntry(true)}>Add Entry</Button>
                  </div>
                )}
                {data.authorizedActions.find((a) => a.name === "SUBMIT") && (
                  <div className="d-inline-block me-3">
                    <Button variant="primary"  onClick={() => submit()}> Submit </Button>
                  </div>
                )}
                {data.authorizedActions.find((a) => a.name === "APPROVE") && (
                  <div className="d-inline-block me-3">
                    <Button variant="primary" onClick={() => approve()}>Approve</Button>
                  </div>
                )}
                {data.authorizedActions.find((a) => a.name === "REJECT") && (
                  <div className="d-inline-block me-3">
                    <Button variant="danger" onClick={() => reject()}> Reject </Button>
                  </div>
                )}
                {data.authorizedActions.find((a) => a.name === "DELETE") && (
                  <div className="d-inline-block">
                    <Button 
                      variant="danger" 
                      onClick={() => {
                        setModalAction(() => delTimeOff)
                        setOpenModal(true)
                      }}>
                      Delete
                    </Button>
                  </div>
                )}
              </Col>
            </Row>
            <Tab.Container animation={false} defaultActiveKey={"table"}>
              <Tab.Content className="pt-4">
                <Tab.Pane eventKey={"table"}>
                  <Table responsive striped className="mt-3">
                    <thead>
                      <tr>
                        <th style={{ textAlign: "center" }}>Action</th>
                        <th style={{ textAlign: "center" }}>Request Date</th>
                        <th style={{ textAlign: "center" }}>Hours</th>
                        <th style={{ textAlign: "center" }}>Payroll Code</th>
                      </tr>
                    </thead>
                    <tbody>
                      {timeoffData.current.map((d, i) => (
                        <tr key={i} style={{ 
                            textDecoration: d.isRejected ? "line-through" : "none",
                            fontWeight: d.isApproved ? "bold" : "normal"
                          }}>
                          {data.authorizedActions.length > 0 ? (
                            <td style={{ textAlign: "center", display: "flex", alignItems: "center", justifyContent: "center", }}>
                              {(isOwner() || user.isAdministrator) && data.authorizedActions.find((a) => a.name === "DELETE_ENTRY") && (
                                <>
                                  <div style={{ cursor: "pointer" }}>
                                    <i className="flaticon-381-multiply-1 text-danger"
                                      onClick={() => {
                                        setModalAction(() => () => delEntry(d.id))
                                        setOpenModal(true)
                                      }}
                                    />
                                  </div>

                                  <div className="mx-2" style={{ cursor: "pointer" }}>
                                    <i class="fas fa-pen"
                                      onClick={() => {
                                        setEntry({ timeoffEntryId: d.id, type: d.type, hours: d.hours,});
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
                          <td style={{ textAlign: "center" }}>{moment(d.requestDate).format("MM/DD/YYYY")}</td>
                          <td style={{ textAlign: "center" }}>{d.hours}</td>
                          <td style={{ textAlign: "center" }}>{d.payrollCode}</td>
                        </tr>
                      ))}
                    </tbody>
                  </Table>
                  <div id="example_wrapper" className="dataTables_wrapper">
                    <div className="d-sm-flex text-center justify-content-between align-items-center mt-1">
                      <div className="dataTables_info" />
                      <Pagination size={"sx"} className={`pagination-gutter pagination- pagination-circle`}>
                        <li className="page-item page-indicator">
                          <Link className="page-link" to="#"
                            onClick={() =>
                              activePag.current > 0 &&
                              onClick(activePag.current - 1)
                            }
                          >
                            <i className="la la-angle-left" />
                          </Link>
                        </li>
                        {paggination.map((number, i) => (
                          <Pagination.Item key={number} active={activePag.current === i} onClick={() => onClick(i)}>
                            {number}
                          </Pagination.Item>
                        ))}
                        <li className="page-item page-indicator">
                          <Link className="page-link" to="#"
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
                  <Accordion className="accordion accordion-primary" defaultActiveKey="12">
                    <div className="accordion-item">
                      <Accordion.Toggle
                        as={Card.Text}
                        eventKey="12"
                        className={`accordion-header rounded-lg mt-3 ${
                          !accordionTable ? "" : "collapsed"
                        }`}
                        onClick={() => setAccordionTable(!accordionTable)}
                      >
                        <span className="accordion-header-text">Total Hours : {data.data.totalHours}</span>
                        <span className="accordion-header-indicator"></span>
                      </Accordion.Toggle>
                      <Accordion.Collapse eventKey="12">
                        <div className="accordion-body-text">
                          <div className="mx-4">
                            <dl>
                              {summary.map((s) => (
                                <>
                                  <dt>•{" "} {moment(s.requestDate).format("MMM DD, YYYY")}{" "} - total hours : {s.hours}</dt>
                                  <dd className="mx-3" style={{ display: "flex",}}>
                                    <div
                                      className="mx-2 rounded"
                                      style={{
                                        backgroundColor: getColor(s.payrollCode ? s.payrollCode : "OTHER"),
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

export default Timeoff;
