import React, {
  Fragment,
  useCallback,
  useContext,
  useEffect,
  useRef,
  useState,
} from "react";
import { Button, Pagination } from "react-bootstrap";
import { Link } from "react-router-dom";
import { ThemeContext } from "../../context/themeContext";
import { getMyTeam } from "../../redux/actions/employees";
import {
  getTimeoffStatuses,
  getTimeoffTypes,
  getTimesheetStatuses,
  getTimesheetEntryStatuses,
} from "../../redux/actions/referentials";
import SpinnerComponent from "../../components/spinner/spinner";
import { useSelector } from "react-redux";
import NewTimeOff from "../timeoff/newTimeOff";
import { enumerateDaysBetweenDates } from "../../services/util";
import { displayError, displaySuccess } from "../../services/toast";
import moment from "moment";
import { addTimeoff } from "../../redux/actions/timesoffs";


const EmployeeList = () => {
  const { setTitle } = useContext(ThemeContext);
  const loading = useSelector((state) => state.auth.showLoading);

  useEffect(() => {
    setTitle("Timesheets");
  }, [setTitle]);

  const [data, setData] = useState({});
  const [items, setItems] = useState([]);

  // const activePag = useRef(0);
  // const size = 50;

  // const [paggination, setPagginnation] = useState([]);
  const employeeData = useRef([]);
  const [filter, setFilter] = useState("");
  const [direct, setDirect] = useState(true);
  const [timesheetStatus, setTimesheetStatus] = useState([]);
  const [timeoffStatus, setTimeoffStatus] = useState([]);

  const [types, setTypes] = useState([]);
  const [openAddTimeOff, setOpenAddTimeOff] = useState(false);
  const [timeoff, setTimeoff] = useState({});
  const [setselectedEmployeeId, setSetselectedEmployeeId] = useState(null);

  const fetchStatus = async () => {
    let statuses = [];
    await getTimesheetStatuses(true)
      .then((resp) => resp.forEach((e) => statuses.push({ ...e, active: false})))
      .catch((err) => displayError(err, "Error while retrieving timesheet statuses"));

    await getTimesheetEntryStatuses()
      .then((resp) => resp.forEach((e) => statuses.push({ ...e, active: false})))
      .catch((err) => displayError(err, "Error while retrieving timesheet entry statuses"));

    setTimesheetStatus(statuses);
  };

  const fetchTimeOffStatus = async () => {
    await getTimeoffStatuses()
      .then((resp) => setTimeoffStatus(resp.map((e) => { return {...e, active: false} })))
      .catch((err) => displayError(err, "Error while retrieving timeoff statuses"));
  };

  const fetchData = useCallback(async () => {
    await getMyTeam(direct)
      .then((resp) => {
        setData(resp);
        setItems(resp.items);
      })
      .catch((err) => displayError(err, "Error while retrieving fetching team data"))
  }, [direct]);

  useEffect(() => {
    fetchData();
    fetchStatus();
    fetchTimeOffStatus();
  }, [fetchData]);

  useEffect(() => {
    fetchTypes();
  }, []);

  const onFilterUpdate = useCallback(() => {
    let timeoffStat = timeoffStatus.filter((s) => s.active).map((s) => s.name);
    let timesheetStat = timesheetStatus.filter((s) => s.active).map((s) => s.name);

    var statusFiltered = items.filter(
      (d) =>
        timeoffStat.includes(d.lastTimeoffStatusString) ||
        timesheetStat.includes(d.lastTimesheetStatusString)
    );

    return timeoffStat.length > 0 || timesheetStat.length > 0
      ? statusFiltered.filter((d) =>
          d.fullName.toLowerCase().concat(d.employeeId).includes(filter.toLowerCase())
        )
      : items.filter((d) =>
          d.fullName.toLowerCase().concat(d.employeeId).includes(filter.toLowerCase())
        );
  }, [filter, items, timesheetStatus, timeoffStatus]);

  // useEffect(() => {
    // if (data.totalItems) {
      // let array = onFilterUpdate();

      // setPagginnation(
        // Array(Math.ceil(array.length / size))
          // .fill()
          // .map((_, i) => i + 1)
      // );

      // employeeData.current = array.slice(
        // activePag.current * size,
        // (activePag.current + 1) * size
      // );
    // }
  // }, [data, employeeData, items, onFilterUpdate]);

  // const onPaginationClick = (i) => {
    // activePag.current = i;
    // employeeData.current = onFilterUpdate().slice(
      // activePag.current * size,
      // (activePag.current + 1) * size
    // );
  // };

  const fetchTypes = async () => {
    await getTimeoffTypes(true)
      .then((resp) => {
        setTypes(resp);
      })
      .catch((err) => displayError(err, "Erro while getting Time off types"))
  };

  const createTimeOff = async () => {
    var dates = enumerateDaysBetweenDates(
      moment(timeoff.start),
      moment(timeoff.end)
    );

    var dataToSave = {
      requestStartDate: timeoff.start,
      requestEndDate: timeoff.end,
      employeeId: setselectedEmployeeId,
      employeeComment: timeoff.employeeComment,
      entries: dates.map((d) => {
        return {
          employeeId: setselectedEmployeeId,
          requestDate: new Date(d),
          type: timeoff.type.value,
          hours: timeoff.hours,
          label: timeoff.label && timeoff.label.value,
        };
      }),
    };

    await addTimeoff(dataToSave)
      .then((res) => {
        setOpenAddTimeOff(false);
        displaySuccess("Successful submit");
        fetchData();
      })
      .catch((err) => displayError(err, "Error while adding a new timeoff"));
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
      <NewTimeOff
        isOpen={openAddTimeOff}
        close={() => {
          setTimeoff({});
          setOpenAddTimeOff(false);
        }}
        data={timeoff}
        updateData={setTimeoff}
        save={createTimeOff}
        types={types}
      />
      <div className="form-head mb-4 d-flex flex-wrap align-items-center">
        <div className="input-group">
          <button className="input-group-text">
            <i className="flaticon-381-search-2 text-primary"></i>
          </button>
          <input
            type="text"
            className="form-control"
            placeholder="Search Employee Name..."
            value={filter}
            onChange={(e) => {
              setFilter(e.target.value);
            }}
          />
        </div>
      </div>
      <div className="row mb-4 align-items-center">
        <div className="col-xl-12 col-lg-12">
          <div className="card m-0 ">
            <div className="card-body py-3 py-md-2">
              <div className="d-sm-flex  d-block align-items-center">
                <div className="d-flex mb-sm-0 mb-3 me-auto align-items-center">
                  <svg
                    className="me-2 user-ico mb-1"
                    width="24"
                    height="24"
                    viewBox="0 0 24 24"
                    fill="none"
                    xmlns="http://www.w3.org/2000/svg"
                  >
                    <g clipPath="url(#clip0)">
                      <path
                        d="M21 24H3C2.73478 24 2.48043 23.8946 2.29289 23.7071C2.10536 23.5196 2 23.2652 2 23V22.008C2.00287 20.4622 2.52021 18.9613 3.47044 17.742C4.42066 16.5227 5.74971 15.6544 7.248 15.274C7.46045 15.2219 7.64959 15.1008 7.78571 14.9296C7.92182 14.7583 7.9972 14.5467 8 14.328V13.322L6.883 12.206C6.6032 11.9313 6.38099 11.6036 6.22937 11.2419C6.07776 10.8803 5.99978 10.4921 6 10.1V5.96201C6.01833 4.41693 6.62821 2.93765 7.70414 1.82861C8.78007 0.719572 10.2402 0.0651427 11.784 5.16174e-06C12.5992 -0.00104609 13.4067 0.158488 14.1603 0.469498C14.9139 0.780509 15.5989 1.2369 16.1761 1.81263C16.7533 2.38835 17.2114 3.07213 17.5244 3.82491C17.8373 4.5777 17.999 5.38476 18 6.20001V10.1C17.9997 10.4949 17.9204 10.8857 17.7666 11.2495C17.6129 11.6132 17.388 11.9426 17.105 12.218L16 13.322V14.328C16.0029 14.5469 16.0784 14.7586 16.2147 14.9298C16.351 15.1011 16.5404 15.2221 16.753 15.274C18.251 15.6548 19.5797 16.5232 20.5298 17.7424C21.4798 18.9617 21.997 20.4624 22 22.008V23C22 23.2652 21.8946 23.5196 21.7071 23.7071C21.5196 23.8946 21.2652 24 21 24ZM4 22H20C19.9954 20.8996 19.6249 19.8319 18.9469 18.9651C18.2689 18.0983 17.3219 17.4816 16.255 17.212C15.6125 17.0494 15.0423 16.6779 14.6341 16.1558C14.2259 15.6337 14.0028 14.9907 14 14.328V12.908C14.0001 12.6428 14.1055 12.3885 14.293 12.201L15.703 10.792C15.7965 10.7026 15.8711 10.5952 15.9221 10.4763C15.9731 10.3574 15.9996 10.2294 16 10.1V6.20001C16.0017 5.09492 15.5671 4.03383 14.7907 3.24737C14.0144 2.46092 12.959 2.01265 11.854 2.00001C10.8264 2.04117 9.85379 2.47507 9.1367 3.21225C8.41962 3.94943 8.01275 4.93367 8 5.96201V10.1C7.99979 10.2266 8.0249 10.352 8.07384 10.4688C8.12278 10.5856 8.19458 10.6914 8.285 10.78L9.707 12.2C9.89455 12.3875 9.99994 12.6418 10 12.907V14.327C9.99724 14.9896 9.77432 15.6325 9.3663 16.1545C8.95827 16.6766 8.3883 17.0482 7.746 17.211C6.67872 17.4804 5.73137 18.0972 5.05318 18.9642C4.37498 19.8313 4.00447 20.8993 4 22Z"
                        fill="#000"
                      />
                    </g>
                    <defs>
                      <clipPath id="clip0">
                        <rect width="24" height="24" fill="white" />
                      </clipPath>
                    </defs>
                  </svg>
                  <div className="media-body">
                    <h3 className="mb-0 font-w600 fs-22">
                      {data.length} Employees 
                      <Button
                        className="me-2 btn-xxs"
                        variant={direct ? "danger" : "outline-primary"}
                        onClick={() => {
                          setDirect(!direct);
                        }}
                      >
                        {direct ? "Show all" : "Direct Report"}
                      </Button>
                    </h3>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div className="row mb-3 align-items-center">
        <div className="col-xl-12 col-lg-12">
          <div className="d-sm-flex  d-block align-items-center">
            <div className="d-flex align-items-center">
              <div className="media-body">
                <label className="me-3">Timesheet Status : </label>
                {timesheetStatus.map((s, index) => (
                  <Button
                    key={index}
                    className="me-2 btn-xxs"
                    variant={s.active ? "danger" : "outline-danger"}
                    onClick={() => {
                      let array = [...timesheetStatus];
                      array[index].active = !array[index].active;
                      setTimesheetStatus(array);
                    }}
                  >
                    {s.name.replace("_", " ")}
                  </Button>
                ))}
                <br/>
                <label className="me-3">Timeoff Status : </label>
                {timeoffStatus.map((s, index) => (
                  <Button
                    key={index}
                    className="me-2 btn-xxs"
                    variant={s.active ? "danger" : "outline-danger"}
                    onClick={() => {
                      let array = [...timeoffStatus];
                      array[index].active = !array[index].active;
                      setTimeoffStatus(array);
                    }}
                  >
                    {s.name.replace("_", " ")}
                  </Button>
                ))}
              </div>
            </div>
          </div>
        </div>
      </div>
      <div className="card">
        <div className="card-body">
          <div className="w-100">
            <div id="example_wrapper" className="dataTables_wrapper">
              <table id="example" className="display w-100 dataTable">
                <thead>
                  <tr role="row">
                    <th>Name</th>
                    <th>Timesheet</th>
                    <th>Time off</th>
                  </tr>
                </thead>
                <tbody>
                  {onFilterUpdate().map((da, i) => (
                    <tr key={i}>
                      <td>
                        <Link to={`/profile/${da.employeeId}`}>
                          <strong>{da.fullName}</strong> - <strong>{da.employeeId}</strong> :
                        </Link>

                        <span>
                          &nbsp;[ P :{" "}
                          <strong>{da.personalBalance}</strong>- V :{" "}
                          <strong>{da.vacationBalance}</strong>]
                        </span>
                      </td>
                      <td>
                        {da.timesheetId ? (
                          <Link to={`/timesheets/${da.lastTimesheetPayrollPeriod}/employee/${da.employeeId}/${new Date(da.lastTimesheetWorkDate).getTime()}`}>
                            <strong>{da.lastTimesheetStatusString.replace("_", " ")}</strong>
                          </Link>
                        ) : (
                          <div>
                            {da.lastTimesheetStatusString.replace("_", " ")}
                          </div>
                        )}
                      </td>
                      <td>
                        {da.timeoffId ? (
                          <Link to={`/${da.isLastTimeoffRequireApproval ? "timeoffs" : "shop&unpaid"}/${da.timeoffId}/employee/${da.employeeId}/${new Date(da.lastTimeoffRequestDate).getTime()}`}>
                            <strong>{da.lastTimeoffStatusString.replace("_", " ")}</strong>
                          </Link>
                        ) : (
                          <Fragment>
                            {da.lastTimeoffStatusString.replace("_", " ")}
                          </Fragment>
                        )}
                        <span
                          className="badge badge-rounded badge-primary"
                          style={{ cursor: "pointer", marginLeft: 10 }}
                          onClick={() => {
                            setSetselectedEmployeeId(da.employeeId);
                            setOpenAddTimeOff(true);
                          }}
                        >
                          Add
                        </span>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>

              {/* <div className="d-sm-flex text-center justify-content-between align-items-center mt-3">
                <div className="dataTables_info" />
                <Pagination
                  size={"sx"}
                  className={`pagination-gutter pagination- pagination-circle`}
                >
                  <li className="page-item page-indicator">
                    <Link
                      className="page-link"
                      to="my-team"
                      onClick={() =>
                        activePag.current > 0 &&
                        onPaginationClick(activePag.current - 1)
                      }
                    >
                      <i className="la la-angle-left" />
                    </Link>
                  </li>
                  {paggination.map((number, i) => (
                    <Pagination.Item
                      key={i}
                      active={activePag.current === i}
                      onClick={() => onPaginationClick(i)}
                    >
                      {number}
                    </Pagination.Item>
                  ))}
                  <li className="page-item page-indicator">
                    <Link
                      className="page-link"
                      to="my-team"
                      onClick={() =>
                        activePag.current + 1 < paggination.length &&
                        onPaginationClick(activePag.current + 1)
                      }
                    >
                      <i className="la la-angle-right" />
                    </Link>
                  </li>
                </Pagination>
              </div> */}
            </div>
          </div>
        </div>
      </div>
    </>
  );
};
export default EmployeeList;

