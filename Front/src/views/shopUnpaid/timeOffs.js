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
import NewTimeOff from "./newTimeOff";
import {
  addTimeoff,
  deleteTimeoff,
  getTimeoffsEmployeeEntriesHistory,
  getTimeoffsEmployeeHistory,
  getTimeoffsEmployeeMonthStatistics,
} from "../../redux/actions/timesoffs";
import { enumerateDaysBetweenDates } from "../../services/util";
import { toast } from "react-toastify";
import ConfirmModal from "./confirmModal";
import { getNonRegularTimeoffTypes } from "../../redux/actions/referentials";
import SpinnerComponent from "../../components/spinner/spinner";
import { useSelector } from "react-redux";
import { getEmployeeById } from "../../redux/actions/employees";

const TimeOffs = ({ history, match }) => {
  const { setTitle } = useContext(ThemeContext);
  const loading = useSelector((state) => state.auth.showLoading);

  const [employee, setEmployee] = useState({});

  useEffect(() => {
    setTitle(
      `${
        employee.fullName ? employee.fullName : ""
      } - Timeoff History Shop & Unpaid`
    );
  }, [setTitle, employee]);

  const calendarRef = useRef({});

  const [data, setData] = useState({});
  const [items, setItems] = useState([]);
  const [page, setPage] = useState(1);
  const [statistics, setStatistics] = useState([]);
  const [paggination, setPagginnation] = useState([]);
  const timeoffsData = useRef([]);

  const [isOpen, setIsOpen] = useState(false);
  const [timeOff, setTimeOff] = useState({});

  const [accordionCalendar, setAccordionCalendar] = useState(false);
  const [legendAccordion, setLegendAccordion] = useState(false);

  const [selectedTimeOff, setSelectedTimeOff] = useState({});
  const [openModal, setOpenModal] = useState(false);

  const [types, setTypes] = useState([]);

  const [filter, setFilter] = useState("");

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
      case "JURY_DUTY":
        return "#07b1a6";
      case "PERSONAL":
        return "#b1a7a6";
      case "BERV":
        return "#ffe66d";
      default:
        return "#ffe66d";
    }
  };

  const activePag = useRef(0);

  const size = 50;

  const fetchData = useCallback(async () => {
    await getTimeoffsEmployeeHistory(match.params.id, false, page)
      .then((resp) => {
        setData(resp);
        setItems(resp.items);
      })
      .catch((err) => {
        toast.error(
          err.response.data.message ? err.response.data.message : "Error"
        );
      });
  }, [match.params.id, page]);

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

  const onFilterUpdate = useCallback(() => {
    var array = items.filter(
      (d) =>
        moment(d.createdDate)
          .format("MM/DD/YYYY")
          .toLowerCase()
          .includes(filter.toLowerCase()) ||
        `${moment(d.requestStartDate).format("MMM DD, YYYY")} - ${moment(
          d.requestEndDate
        ).format("MMM DD, YYYY")}`
          .toLowerCase()
          .includes(filter.toLowerCase()) ||
        d.statusName
          .replace("_", " ")
          .toLowerCase()
          .includes(filter.toLowerCase())
    );
    return array;
  }, [items, filter]);

  const fetchCalendarData = useCallback(
    async (fetchInfo, successCallback, failureCallback) => {
      try {
        let date = moment(new Date()).format("MM/DD/YYYY");

        if (fetchInfo) {
          date = moment(fetchInfo.start).add(7, "d").format("MM/DD/YYYY");
        }

        const response = await getTimeoffsEmployeeEntriesHistory(
          match.params.id,
          false,
          date
        );

        successCallback(
          response
            ? response.map((d) => {
                console.log(d);
                return {
                  id: d.id,
                  title: `${d.hours}h.`,
                  start: moment(d.requestDate).format("YYYY-MM-DD"),
                  end: moment(d.requestDate).format("YYYY-MM-DD"),
                  url: `/timeoffs/${d.timeoffHeaderId}/employee/${
                    match.params.id
                  }/${new Date(d.requestDate).getTime()}`,
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

  const fetchStatistics = useCallback(async () => {
    await getTimeoffsEmployeeMonthStatistics(match.params.id)
      .then((resp) => {
        setStatistics(resp);
      })
      .catch((err) => {
        toast.error(
          err.response.data.message ? err.response.data.message : "Error"
        );
      });
  }, [match.params.id]);

  const fetchTypes = async () => {
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
  };

  useEffect(() => {
    fetchEmployee();
  }, [fetchEmployee]);

  useEffect(() => {
    fetchData();
    fetchStatistics();
    fetchTypes();
  }, [fetchData, fetchStatistics]);

  useEffect(() => {
    fetchCalendarData();
  }, [fetchCalendarData]);

  useEffect(() => {
    if (data.totalItems) {
      let array = onFilterUpdate();
      setPagginnation(
        Array(Math.ceil(array.length / size))
          .fill()
          .map((_, i) => i + 1)
      );

      timeoffsData.current = array.slice(
        activePag.current * size,
        (activePag.current + 1) * size
      );
    }
  }, [data, timeoffsData, items, onFilterUpdate]);

  const onClick = (i) => {
    activePag.current = i;
    timeoffsData.current = onFilterUpdate().slice(
      activePag.current * size,
      (activePag.current + 1) * size
    );
  };

  const createTimeOff = async () => {
    var dates = enumerateDaysBetweenDates(
      moment(timeOff.start),
      moment(timeOff.end)
    );

    var dataToSave = {
      requestStartDate: timeOff.start,
      requestEndDate: timeOff.end,
      employeeId: match.params.id,
      employeeComment: timeOff.employeeComment,
      entries: dates.map((d) => {
        return {
          employeeId: match.params.id,
          requestDate: new Date(d),
          type: timeOff.type.value,
          hours: timeOff.hours,
          label: timeOff.label && timeOff.label.value,
        };
      }),
    };

    await addTimeoff(dataToSave)
      .then((res) => {
        setIsOpen(false);
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
          err.response.data.message ? err.response.data.message : "Error"
        );
      });
  };

  const delTimeOff = async () => {
    setOpenModal(false);
    await deleteTimeoff(selectedTimeOff.timeoffId, match.params.id)
      .then((resp) => {
        toast.success("Successful deletion", {
          position: "top-right",
          autoClose: 5000,
          hideProgressBar: false,
          closeOnClick: true,
          pauseOnHover: true,
          draggable: true,
        });
        history.reload();
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
      <NewTimeOff
        isOpen={isOpen}
        close={() => {
          setTimeOff({});
          setIsOpen(false);
        }}
        data={timeOff}
        updateData={setTimeOff}
        save={createTimeOff}
        types={types}
      />
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
                                  {types.map((t, index) => (
                                    <dt
                                      key={index}
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
                        height={600}
                        selectable={true}
                        select={(e) => {
                          setTimeOff({
                            ...timeOff,
                            start: moment(e.startStr),
                            end: moment(e.endStr).subtract(1, "d"),
                          });
                          setIsOpen(true);
                        }}
                        rerenderDelay={10}
                        eventDurationEditable={false}
                        editable={true}
                        plugins={[
                          dayGridPlugin,
                          timeGridPlugin,
                          interactionPlugin,
                        ]}
                        events={(fetchInfo, successCallback, failureCallback) =>
                          fetchCalendarData(
                            fetchInfo,
                            successCallback,
                            failureCallback
                          )
                        }
                        eventClick={(e) => {}}
                      />
                    </Col>
                    <Col lg={12} className="mt-lg-4">
                      <Accordion
                        className="accordion accordion-primary"
                        defaultActiveKey="3"
                      >
                        <div className="accordion-item">
                          <Accordion.Toggle
                            as={Card.Text}
                            eventKey="1"
                            className={`accordion-header rounded-lg ${
                              accordionCalendar ? "" : "collapsed"
                            }`}
                            onClick={() =>
                              setAccordionCalendar(!accordionCalendar)
                            }
                          >
                            <span className="accordion-header-text">
                              Months statics
                            </span>
                            <span className="accordion-header-indicator"></span>
                          </Accordion.Toggle>
                          <Accordion.Collapse eventKey="1">
                            <div className="accordion-body-text">
                              <div className="mx-4">
                                <dl>
                                  {statistics.map((s, index) => (
                                    <Fragment key={index}>
                                      <dt>
                                        • {moment(s.month).format("MMMM YYYY")}{" "}
                                        -{" "}
                                        {s.monthStatistics.reduce((x, i) => {
                                          return x + i.hours;
                                        }, 0)}{" "}
                                        h.
                                      </dt>
                                      {s.monthStatistics.map((m, index) => (
                                        <dd className="mx-3" key={index}>
                                          - {m.statusName.replaceAll("_", " ")}{" "}
                                          - {m.hours} h.
                                        </dd>
                                      ))}
                                    </Fragment>
                                  ))}
                                </dl>
                              </div>
                            </div>
                          </Accordion.Collapse>
                        </div>
                      </Accordion>
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
                    <Col>
                      <div className="d-sx-inline-block">
                        <Button onClick={() => setIsOpen(true)}>Add</Button>
                      </div>
                    </Col>
                  </Row>
                  <Table responsive striped className="mt-3">
                    <tbody>
                      {timeoffsData.current.map((d, i) => (
                        <tr key={i}>
                          <td
                            style={{ textAlign: "center", cursor: "pointer" }}
                            onClick={() => {
                              history.push(
                                `/timeoffs/${d.timeoffId}/employee/${d.employeeId}`
                              );
                            }}
                          >
                            {d.fullName}
                          </td>
                          <td
                            style={{ textAlign: "center", cursor: "pointer" }}
                            onClick={() => {
                              history.push(
                                `/timeoffs/${d.timeoffId}/employee/${d.employeeId}`
                              );
                            }}
                          >
                            Created:{" "}
                            {moment(d.createdDate).format("MM/DD/YYYY")}
                          </td>
                          <td
                            style={{ textAlign: "center", cursor: "pointer" }}
                            onClick={() => {
                              history.push(
                                `/timeoffs/${d.timeoffId}/employee/${d.employeeId}`
                              );
                            }}
                          >{`${moment(d.requestStartDate).format(
                            "MMM DD, YYYY"
                          )} - ${moment(d.requestEndDate).format(
                            "MMM DD, YYYY"
                          )}`}</td>
                          <td
                            style={{ textAlign: "center", cursor: "pointer" }}
                            onClick={() => {
                              history.push(
                                `/timeoffs/${d.timeoffId}/employee/${d.employeeId}`
                              );
                            }}
                          >
                            {d.statusName.replaceAll("_", " ")}
                          </td>
                          <td
                            style={{ textAlign: "center", cursor: "pointer" }}
                            onClick={() => {
                              setSelectedTimeOff(d);
                              setOpenModal(true);
                            }}
                          >
                            <i className="flaticon-381-multiply-1 text-danger"></i>
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

export default TimeOffs;
