import React, { useCallback, useContext, useEffect, useRef } from "react";
import FullCalendar from "@fullcalendar/react";
import dayGridPlugin from "@fullcalendar/daygrid";
import timeGridPlugin from "@fullcalendar/timegrid";
import interactionPlugin from "@fullcalendar/interaction";
import { Button, Col, Nav, Pagination, Row, Tab, Table } from "react-bootstrap";
import { useState } from "react";
import moment from "moment";
import { Link } from "react-router-dom";
import { id } from "date-fns/locale";
import { ThemeContext } from "../../context/themeContext";
import {
  createHoliday,
  deleteHoliday,
  getHolidays,
  updateHoliday,
} from "../../redux/actions/holidays";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import NewHoliday from "./newHoliday";
import ConfirmModal from "./confirmModal";
import SpinnerComponent from "../../components/spinner/spinner";
import { useSelector } from "react-redux";

const Holidays = () => {
  const { setTitle } = useContext(ThemeContext);
  const loading = useSelector((state) => state.auth.showLoading);

  useEffect(() => {
    setTitle("Holidays calendar");
  }, [setTitle]);

  const calendarRef = useRef({});

  const sort = 15;
  const [data, setData] = useState([]);
  const [paggination, setPagginnation] = useState([]);
  const holidaysData = useRef([]);
  const [filter, setFilter] = useState("");

  const [isOpen, setIsOpen] = useState(false);
  const [holiday, setHoliday] = useState({
    date: new Date(),
  });

  const [openConfirm, setOpenConfirm] = useState(false);
  const [selectedHoliday, setSelectedHoliday] = useState(null);

  const [action, setAction] = useState("New Holiday");

  const fetchData = useCallback(async () => {
    await getHolidays()
      .then((resp) => {
        setData(resp);
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

  const onFilterUpdate = useCallback(() => {
    var array = data.filter(
      (d) =>
        d.description.toLowerCase().includes(filter.toLowerCase()) ||
        d.notes.toLowerCase().includes(filter.toLowerCase()) ||
        moment(d.date)
          .format("MMM DD, YYYY")
          .toLowerCase()
          .includes(filter.toLowerCase())
    );
    return array;
  }, [data, filter]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  useEffect(() => {
    let array = onFilterUpdate();

    setPagginnation(
      Array(Math.ceil(array.length / sort))
        .fill()
        .map((_, i) => i + 1)
    );

    holidaysData.current = array.slice(
      activePag.current * sort,
      (activePag.current + 1) * sort
    );
  }, [data, onFilterUpdate, holidaysData]);

  const activePag = useRef(0);

  const onClick = (i) => {
    activePag.current = i;

    holidaysData.current = data.slice(
      activePag.current * sort,
      (activePag.current + 1) * sort
    );
  };

  const saveHoliday = async () => {
    if (selectedHoliday) {
      await updateHoliday({
        id: selectedHoliday.id,
        description: holiday.description,
        notes: holiday.notes,
      })
        .then((res) => {
          setIsOpen(false);
          toast.success("Successful updating", {
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
    } else {
      await createHoliday(holiday)
        .then((res) => {
          setIsOpen(false);
          toast.success("Successful creation", {
            position: "top-right",
            autoClose: 5000,
            hideProgressBar: false,
            closeOnClick: true,
            pauseOnHover: true,
            draggable: true,
          });
          fetchData();
        })
        .cath((err) => {});
    }
  };

  const sendDelete = async () => {
    await deleteHoliday({
      id: selectedHoliday.id,
    })
      .then((resp) => {
        setOpenConfirm(false);
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
      <NewHoliday
        close={() => setIsOpen(false)}
        data={holiday}
        updateData={setHoliday}
        isOpen={isOpen}
        save={() => saveHoliday()}
        action={action}
      />

      <ConfirmModal
        close={() => setOpenConfirm(!openConfirm)}
        isOpen={openConfirm}
        action={() => sendDelete()}
      />

      <div className="card">
        <div className="card-body">
          <div className="custom-tab-1">
            <Tab.Container animation={false} defaultActiveKey={"calendar"}>
              <Nav as="ul" className="nav-pills mb-4 justify-content-start">
                <Nav.Item as="li">
                  <Nav.Link eventKey={"calendar"}>
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
                  <FullCalendar
                    ref={calendarRef}
                    rerenderDelay={10}
                    eventDurationEditable={false}
                    editable={true}
                    plugins={[dayGridPlugin, timeGridPlugin, interactionPlugin]}
                    dateClick={(info) => {
                      setAction("New Holiday");
                      setIsOpen(true);
                      setHoliday({ ...holiday, date: new Date(info.dateStr) });
                    }}
                    events={data.map((d, i) => {
                      return {
                        id: id,
                        title: d.description,
                        start: moment(d.date).format("YYYY-MM-DD"),
                        end: moment(d.date).format("YYYY-MM-DD"),
                      };
                    })}
                  />
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
                          placeholder="Search holidays..."
                          value={filter}
                          onChange={(e) => setFilter(e.target.value)}
                        />
                      </div>
                    </Col>
                    <Col>
                      <div className="input-group d-sx-inline-block">
                        <Button
                          onClick={() => {
                            setAction("New Holiday");
                            setIsOpen(true);
                          }}
                        >
                          Add
                        </Button>
                      </div>
                    </Col>
                  </Row>
                  <Table responsive striped>
                    <thead>
                      <tr>
                        <th>Date</th>
                        <th>Description</th>
                        <th>Notes</th>
                        <th>Action</th>
                      </tr>
                    </thead>
                    <tbody>
                      {holidaysData.current.map((d, i) => (
                        <tr key={i}>
                          <td>{moment(d.date).format("MMM DD, YYYY")}</td>
                          <td>{d.description}</td>
                          <td>{d.notes}</td>
                          <td style={{ cursor: "pointer" }}>
                            <i
                              className="flaticon-381-id-card-3 text-primary me-2"
                              onClick={() => {
                                setSelectedHoliday(d);
                                setAction("Update Holiday");
                                setHoliday({
                                  id: d.id,
                                  date: new Date(d.date),
                                  description: d.description,
                                  notes: d.notes,
                                });
                                setIsOpen(true);
                              }}
                            ></i>
                            <i
                              className="flaticon-381-multiply-1 text-danger"
                              onClick={() => {
                                setSelectedHoliday(d);
                                setOpenConfirm(true);
                              }}
                            ></i>
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
                </Tab.Pane>
              </Tab.Content>
            </Tab.Container>
          </div>
        </div>
      </div>
    </>
  );
};

export default Holidays;
