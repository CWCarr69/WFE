import React, {
  useCallback,
  useContext,
  useEffect,
  useRef,
} from "react";
import {
  Button,
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
} from "../../redux/actions/timesoffs";
import {
  getTimeoffsEmployeeHistory,
} from "../../redux/actions/timesoffs";
import { enumerateDaysBetweenDates } from "../../services/util";
import { toast } from "react-toastify";
import ConfirmModal from "./confirmModal";
import { getTimeoffTypes } from "../../redux/actions/referentials";
import SpinnerComponent from "../../components/spinner/spinner";
import { useSelector } from "react-redux";
import { getEmployeeById } from "../../redux/actions/employees";
import { DefaultGlobalDateFormat } from "../../services/util";

const TimeOffs = ({ history, match }) => {
  const { setTitle } = useContext(ThemeContext);
  const loading = useSelector((state) => state.auth.showLoading);

  const [employee, setEmployee] = useState({});

  useEffect(() => {
    setTitle(`${employee.fullName ? employee.fullName : ""} - Non-Billable History`);
  }, [setTitle, employee]);

  const [data, setData] = useState({});
  const [items, setItems] = useState([]);
  const [page, setPage] = useState(1);
  const [paggination, setPagginnation] = useState([]);
  const timeoffsData = useRef([]);

  const [isOpen, setIsOpen] = useState(false);
  const [timeOff, setTimeOff] = useState({});

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
    await getTimeoffsEmployeeHistory(match.params.id, true, page)
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

  const fetchTypes = async () => {
    await getTimeoffTypes(true)
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
    fetchTypes();
  }, [fetchData]);

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
            <Tab.Container animation={false} defaultActiveKey={"table"}>
              <Nav as="ul" className="nav-pills mb-4 justify-content-start">
                <Nav.Item as="li">
                  <Nav.Link eventKey={"table"}>
                    <i className={`la la-list me-2`} />
                    Table View
                  </Nav.Link>
                </Nav.Item>
              </Nav>
              <Tab.Content className="pt-4">
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
                            {moment(d.createdDate).format(DefaultGlobalDateFormat)}
                          </td>
                          <td
                            style={{ textAlign: "center", cursor: "pointer" }}
                            onClick={() => {
                              history.push(
                                `/timeoffs/${d.timeoffId}/employee/${d.employeeId}`
                              );
                            }}
                          >{`${moment(d.requestStartDate).format(
                            "MMM DD, YY"
                          )} - ${moment(d.requestEndDate).format(
                            "MMM DD, YY"
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
