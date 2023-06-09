import React, { useCallback, useContext, useEffect, useRef} from "react";
import { Col, Pagination, Row, Tab, Table } from "react-bootstrap";
import { useState } from "react";
import moment from "moment";
import { Link } from "react-router-dom";
import _ from "lodash";
import { ThemeContext } from "../../context/themeContext";
import NewTimeoff from "../shared/newTimeoff";
import NewTimeoffButton from "../shared/newTimeoffButton";
import { deleteTimeoff } from "../../redux/actions/timesoffs";
import { getTimeoffsEmployeeHistory, } from "../../redux/actions/timesoffs";
import ConfirmModal from "./confirmModal";
import SpinnerComponent from "../../components/spinner/spinner";
import { useSelector } from "react-redux";
import { getEmployeeById } from "../../redux/actions/employees";
import { DefaultGlobalDateFormat } from "../../services/util";
import { displayError, displaySuccess } from "../../services/toast";

const Timeoffs = ({ history, match }) => {
  const { setTitle } = useContext(ThemeContext);
  const loading = useSelector((state) => state.auth.showLoading);

  const [employee, setEmployee] = useState({});

  useEffect(() => setTitle(`${employee.fullName ? employee.fullName : ""} - History`), [setTitle, employee]);

  const [data, setData] = useState({});
  const [items, setItems] = useState([]);
  const [page, setPage] = useState(1);
  const [paggination, setPagginnation] = useState([]);
  const timeoffsData = useRef([]);

  const [isOpen, setIsOpen] = useState(false);

  const [selectedTimeOff, setSelectedTimeOff] = useState({});
  const [openModal, setOpenModal] = useState(false);

  const [filter, setFilter] = useState("");

  const activePag = useRef(0);

  const size = 50;

  const fetchData = useCallback(async () => {
    await getTimeoffsEmployeeHistory(match.params.id, page)
    .then((resp) => {
      setData(resp);
      setItems(resp.items);
    })
    .catch((err) => displayError(err, "Error while fetching employee time off history"));
  }, [match.params.id, page]);

  const fetchEmployee = useCallback(async () => {
    await getEmployeeById(match.params.id)
    .then((resp) => setEmployee(resp))
    .catch((err) => displayError(err, "Error while fetching employee data"));
  }, [match.params.id]);

  const onFilterUpdate = useCallback(() => {
    var array = items.filter(
      (d) =>
        moment(d.createdDate).format("MM/DD/YYYY").toLowerCase().includes(filter.toLowerCase()) ||
        `${moment(d.requestStartDate).format("MMM DD, YYYY")} - ${moment(d.requestEndDate).format("MMM DD, YYYY")}`.toLowerCase().includes(filter.toLowerCase()) ||
        d.statusName.replace("_", " ").toLowerCase().includes(filter.toLowerCase())
    );
    return array;
  }, [items, filter]);

  useEffect(() => fetchEmployee(), [fetchEmployee]);

  useEffect(() => fetchData(), [fetchData]);

  useEffect(() => {
    if (data.totalItems) {
      let array = onFilterUpdate();
      setPagginnation(Array(Math.ceil(array.length / size)).fill().map((_, i) => i + 1));
      timeoffsData.current = array.slice(activePag.current * size, (activePag.current + 1) * size);
    }
  }, [data, timeoffsData, items, onFilterUpdate]);

  const onPageIndicatorClick = (i) => {
    activePag.current = i;
    timeoffsData.current = onFilterUpdate().slice(
      activePag.current * size,
      (activePag.current + 1) * size
    );
  };

  const delTimeOff = async () => {
    setOpenModal(false);
    await deleteTimeoff(selectedTimeOff.timeoffId, match.params.id)
    .then((resp) => {
      displaySuccess("Time off deleted");
      history.reload();
    })
    .catch((err) => displayError(err, "Error while deleting time off"));
  };

  const onClickDetailLink = (timeoff) => history.push(`/timeoffs/${timeoff.timeoffId}/employee/${timeoff.employeeId}`);

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
      <ConfirmModal action={delTimeOff} close={() => setOpenModal(false)} isOpen={openModal} />
      <NewTimeoff isOpen={isOpen} onClose={() => setIsOpen(false)} selectedEmployeeId={match.params.id} />
      <div className="card">
        <div className="card-body">
          <div className="custom-tab-1">
            <Tab.Container animation={false} defaultActiveKey={"table"}>
              <Tab.Content className="pt-4">
                <Tab.Pane eventKey={"table"}>
                  <Row>
                    <Col lg={10}>
                      <div className="input-group search-area2 d-xl-inline-flex">
                        <button className="input-group-text"> <i className="flaticon-381-search-2 text-primary"></i></button>
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
                      <NewTimeoffButton onClick={() => setIsOpen(true)} />
                    </Col>
                  </Row>
                  <Table responsive striped className="mt-3">
                    <tbody>
                      {timeoffsData.current.map((d, i) => (
                        <tr key={i}>
                          <td style={{ textAlign: "center", cursor: "pointer" }} onClick={() => onClickDetailLink(d)}>{d.fullName}</td>
                          <td>Created:{" "}{moment(d.createdDate).format(DefaultGlobalDateFormat)}</td>
                          <td style={{ textAlign: "center", cursor: "pointer" }} onClick={() => onClickDetailLink(d)}>{`${moment(d.requestStartDate).format("MMM DD, YY")} - ${moment(d.requestEndDate).format("MMM DD, YY")}`}</td>
                          <td style={{ textAlign: "center", cursor: "pointer" }} onClick={() => onClickDetailLink(d)}>{d.totalHours}</td>
                          <td style={{ textAlign: "center", cursor: "pointer" }} onClick={() => onClickDetailLink(d)}>{d.statusName.replaceAll("_", " ")}</td>
                          <td>{d.statusName != "APPROVED" &&(<i onClick={() => setSelectedTimeOff(d)} className="flaticon-381-multiply-1 text-danger"></i>)}</td>
                        </tr>
                      ))}
                    </tbody>
                  </Table>
                  <div id="example_wrapper" className="dataTables_wrapper">
                    <div className="d-sm-flex text-center justify-content-between align-items-center mt-1">
                      <div className="dataTables_info" />
                      <Pagination size={"sx"} className={`pagination-gutter pagination- pagination-circle`}>
                        <li className="page-item page-indicator">
                          <Link className="page-link" to="#" onClick={() => activePag.current > 0 && onPageIndicatorClick(activePag.current - 1)}>
                            <i className="la la-angle-left" />
                          </Link>
                        </li>
                        {paggination.map((number, i) => (
                          <Pagination.Item key={number} active={activePag.current === i}>{number}</Pagination.Item>
                        ))}
                        <li className="page-item page-indicator">
                          <Link className="page-link" to="#" onClick={() =>activePag.current + 1 < paggination.length && onPageIndicatorClick(activePag.current + 1)}>
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

export default Timeoffs;
