import React, {
  Fragment,
  useCallback,
  useContext,
  useEffect,
  useRef,
} from "react";
import FullCalendar from "@fullcalendar/react";
import dayGridPlugin from "@fullcalendar/daygrid";
import timeGridPlugin from "@fullcalendar/timegrid";
import interactionPlugin from "@fullcalendar/interaction";
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
import _ from "lodash";
import { ThemeContext } from "../../context/themeContext";
import {
  getTimesheetEmployeeEntriesHistory,
  getTimesheetEmployeeHistory,
  getCurrentEmployeeTimesheetPeriod,
} from "../../redux/actions/timesheets";
import { toast } from "react-toastify";
import SpinnerComponent from "../../components/spinner/spinner";
import { useSelector } from "react-redux";
import { getEmployeeById } from "../../redux/actions/employees";
import { getPayrollCodes } from "../../redux/actions/referentials";
import { DefaultGlobalDateFormat } from "../../services/util";

const Timesheet = ({ history, match }) => {
  const { setTitle } = useContext(ThemeContext);
  const loading = useSelector((state) => state.auth.showLoading);

  const user = useSelector((state) => state.auth.auth);

  const [employee, setEmployee] = useState({});
  const [currentPeriod, setCurrentPeriod] = useState({});

  const calendarRef = useRef({});

  const [data, setData] = useState({});
  const [items, setItems] = useState([]);
  const [page, setPage] = useState(1);
  const [paggination, setPagginnation] = useState([]);
  const timesheetData = useRef([]);
  const [filter, setFilter] = useState("");
  const [initialDate, setInitialDate] = useState(new Date());

  const [payrollCodes, setPayrollCodes] = useState([]);

  const [legendAccordion, setLegendAccordion] = useState(false);

  const activePag = useRef(0);

  const size = 50;

  useEffect(() => {
    setTitle(
      `${employee.fullName ? employee.fullName : ""} - Timesheet History`
    );
  }, [setTitle, employee]);

  const fetchEmployee = useCallback(async () => {
    await getEmployeeById(match.params.id)
      .then((resp) => {
        setEmployee(resp);
      })
      .catch((err) => {
        toast.error(
          err.response.data.message ? err.response.data.message : "Error"
        );
      });
  }, [match.params.id]);

  const fetchCurrentPeriod = useCallback(async () => {
    await getCurrentEmployeeTimesheetPeriod(match.params.id)
      .then((resp) => {
        setCurrentPeriod(resp);
      })
      .catch((err) => {
        toast.error(
          err.response.data.message ? err.response.data.message : "Error"
        );
      });
  }, [match.params.id]);

  useEffect(() => {
    fetchEmployee();
    fetchCurrentPeriod();
  }, [fetchEmployee]);  

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

  const fetchPayrollCodes = useCallback(async () => {
    await getPayrollCodes()
      .then((resp) => {
        setPayrollCodes(
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
          err.response.data.message ? err.response.data.message : "Error"
        );
      });
  }, []);

  const fetchData = useCallback(async () => {
    await getTimesheetEmployeeHistory(match.params.id, page)
      .then((resp) => {
        setData(resp);
        setItems(resp.items);
      })
      .catch((err) => {
        toast.error(
          err.response.data.message ? err.response.data.message : "Error"
        );
      });
  }, [match.params.id, page, setItems]);

  const fetchCalendarData = useCallback(
    async (fetchInfo, successCallback, failureCallback) => {
      try {
        let date = moment(new Date()).format("MM/DD/YYYY");

        if (fetchInfo) {
          date = moment(fetchInfo.start).add(7, "d").format("MM/DD/YYYY");
        }

        const response = await getTimesheetEmployeeEntriesHistory(match.params.id, date);

        if (response && response.length > 0) {
          setInitialDate(response[0].workDate);
        }

        successCallback(
          response
            ? response.map((d, index) => {
                return {
                  id: d.id,
                  title: `${d.quantity}h.`,
                  start: moment(d.workDate).format("YYYY-MM-DD"),
                  end: moment(d.workDate).format("YYYY-MM-DD"),
                  url: `/timesheets/${d.timesheetHeaderId}/employee/${
                    match.params.id
                  }/${new Date(d.workDate).getTime()}`,
                  backgroundColor: getColor(d.payrollCode),
                };
              })
            : []
        );
      } catch (err) {
        toast.error(err);
      }
    },
    [match.params.id]
  );

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  useEffect(() => {
    fetchCalendarData();
  }, [fetchCalendarData]);

  useEffect(() => {
    fetchPayrollCodes();
  }, [fetchPayrollCodes]);

  const onFilterUpdate = useCallback(() => {
    return items.filter(
      (d) =>
        d.payrollPeriod
          .split("-")
          .pop()
          .toLowerCase()
          .includes(filter.toLowerCase()) ||
        `${moment(d.startDate).format("MM/DD/YYYY")} - ${moment(
          d.endDate
        ).format("MM/DD/YYYY")}`
          .toLowerCase()
          .includes(filter.toLowerCase()) ||
        d.statusName
          .replace("_", " ")
          .toLowerCase()
          .includes(filter.toLowerCase())
    );
  }, [filter, items]);

  useEffect(() => {
    if (data.totalItems) {
      let array = onFilterUpdate();

      setPagginnation(
        Array(Math.ceil(array.length / size))
          .fill()
          .map((_, i) => i + 1)
      );

      timesheetData.current = array.slice(
        activePag.current * size,
        (activePag.current + 1) * size
      );
    }
  }, [data, items, timesheetData, onFilterUpdate]);

  const onClick = (i) => {
    activePag.current = i;
    timesheetData.current = onFilterUpdate().slice(
      activePag.current * size,
      (activePag.current + 1) * size
    );
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
          <div className="custom-tab-1">
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
                        defaultActiveKey="30"
                      >
                        <div className="accordion-item">
                          <Accordion.Toggle
                            as={Card.Text}
                            eventKey="0"
                            className={`accordion-header rounded-lg ${
                              legendAccordion ? "" : "collapsed"
                            }`}
                            onClick={() => setLegendAccordion(!legendAccordion)}
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
                                  {payrollCodes.map((t, i) => (
                                    <dt
                                      key={i}
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
                      <FullCalendar
                        ref={calendarRef}
                        initialView="dayGridMonth"
                        height={600}
                        rerenderDelay={10}
                        eventDurationEditable={false}
                        editable={true}
                        plugins={[
                          dayGridPlugin,
                          timeGridPlugin,
                          interactionPlugin,
                        ]}
                        initialDate={initialDate}
                        events={(fetchInfo, successCallback, failureCallback) =>
                          fetchCalendarData(
                            fetchInfo,
                            successCallback,
                            failureCallback
                          )
                        }
                        eventClick={(e) => {}}
                        dayCellClassNames = {
                          ({date}) => {
                            let currentDate = new Date(date);
                            if(currentPeriod &&  currentDate >= new Date(currentPeriod.start) && currentDate <= new Date(currentPeriod.end)){
                              return ['bg-light-danger'] 
                            }
                          }
                        }
                      />
                    </Col>
                  </Row>
                </Tab.Pane>
                <Tab.Pane eventKey={"table"}>
                  <Row>
                    <Col lg={10}>
                      <div className="input-group search-area2 d-xl-inline-flex">
                        <button className="input-group-text">
                          <i className="flaticon-381-search-2 text-primary"></i>
                        </button>
                        <input
                          type="text"
                          className="form-control"
                          placeholder="Search timesheets..."
                          value={filter}
                          onChange={(e) => setFilter(e.target.value)}
                        />
                      </div>
                    </Col>
                    {user.isAdministrator && (
                      <Col>
                        <div className="d-sx-inline-block">
                          <Link to={`/timesheets/${match.params.id}/entry`}>
                            <Button>Add</Button>
                          </Link>
                        </div>
                      </Col>
                    )}
                  </Row>
                  <Table responsive striped className="mt-3">
                    <tbody>
                      {timesheetData.current.map((d, i) => (
                        <tr key={i} style={{ cursor: "pointer" }}>
                          <td
                            onClick={() =>
                              history.push(
                                `/timesheets/${d.timesheetId}/employee/${d.employeeId}`
                              )
                            }
                          >
                            {d.fullName}
                          </td>
                          <td
                            onClick={() =>
                              history.push(
                                `/timesheets/${d.timesheetId}/employee/${d.employeeId}`
                              )
                            }
                          >
                            Period: {d.payrollPeriod.split("-").pop()} - (
                            {moment(d.startDate).format(DefaultGlobalDateFormat)} -{" "}
                            {moment(d.endDate).format(DefaultGlobalDateFormat)})
                          </td>
                          <td
                            onClick={() =>
                              history.push(
                                `/timesheets/${d.timesheetId}/employee/${d.employeeId}`
                              )
                            }
                          >
                            Hours: {d.totalHours}H
                          </td>
                          <td
                            onClick={() =>
                              history.push(
                                `/timesheets/${d.timesheetId}/employee/${d.employeeId}`
                              )
                            }
                          >
                            { d.isFinalized ? d.statusName.replace("_", " ") : d.partialStatusName.replace("_", " ") }
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
