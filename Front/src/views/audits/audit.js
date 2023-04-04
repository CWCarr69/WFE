import React, {
  createRef,
  useCallback,
  useContext,
  useEffect,
  useRef,
  useState,
} from "react";
import { Button, Dropdown } from "react-bootstrap";
import moment from "moment";
import { ThemeContext } from "../../context/themeContext";
import { exportAudit, getAudit } from "../../redux/actions/audits";
import { toast } from "react-toastify";
import DateRangePicker from "react-bootstrap-daterangepicker";
import { useSelector } from "react-redux";
import SpinnerComponent from "../../components/spinner/spinner";

const Audit = () => {
  const { setTitle } = useContext(ThemeContext);
  const [timesession, setTimesession] = useState({
    startDate: new Date(),
    endDate: new Date(),
  });
  const [showRange, setShowRange] = useState(false);

  const loading = useSelector((state) => state.auth.showLoading);

  const [types, setTypes] = useState([]);
  const [selectedType, setSelectedType] = useState("");

  const fetchAudit = async () => {
    await getAudit()
      .then((resp) => {
        setLogs(resp);
        setTypes([...new Set(resp.map((r) => r.type))]);
        let arrayDates = [
          ...new Set(resp.map((r) => moment(r.date).format("MM/DD/YYYY"))),
        ];
        if (arrayDates.length > 0) {
          setTimesession({
            startDate: new Date(
              arrayDates[arrayDates.length >= 7 ? 6 : arrayDates.length - 1]
            ),
            endDate: new Date(arrayDates[0]),
          });
        }
      })
      .catch((err) => {
        toast.error(
          err?.response.data.message ? err.response.data.message : "Error while fetching audit"
        );
      });
  };

  const [logs, setLogs] = useState([]);
  const [filtre, setFiltre] = useState("");

  useEffect(() => {
    setTitle("Audit Visualisation");
  }, [setTitle]);

  useEffect(() => {
    fetchAudit();
  }, []);

  const onFilterUpdate = useCallback(() => {
    let array = logs.filter(
      (e) =>
        moment(e.date).isBetween(
          moment(timesession.startDate),
          moment(timesession.endDate).add(1, "d")
        ) &&
        e.type.toLowerCase().includes(selectedType.toLowerCase()) &&
        (e.authorId.toLowerCase().includes(filtre.toLowerCase()) ||
          e.action.toLowerCase().includes(filtre.toLowerCase()))
    );
    return array;
  }, [filtre, logs, selectedType, timesession]);

  const exportation = async () => {
    await exportAudit()
      .then((res) => {})
      .catch((err) => {
        toast.error(
          err.response.data.message
            ? err.response.data.message
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
      <div className="form-head mb-4 d-flex flex-wrap align-items-center">
        <div className="input-group search-area2 d-xl-inline-flex mb-2 me-4">
          <button className="input-group-text">
            <i className="flaticon-381-search-2 text-primary"></i>
          </button>
          <input
            type="text"
            className="form-control"
            placeholder="Search here..."
            onChange={(e) => setFiltre(e.target.value)}
          />
        </div>
        <div
          className="input-group search-area2 d-xl-inline-flex mb-2 
        me-4 btn btn-sm xl d-flex align-items-center i-false c-pointer"
          onClick={() => {
            setShowRange(!showRange);
          }}
        >
          <svg
            className="primary-icon"
            width="28"
            height="28"
            viewBox="0 0 28 28"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M22.167 5.83362H21.0003V3.50028C21.0003 3.19087 20.8774 2.89412 20.6586 2.67533C20.4398 2.45653 20.143 2.33362 19.8336 2.33362C19.5242 2.33362 19.2275 2.45653 19.0087 2.67533C18.7899 2.89412 18.667 3.19087 18.667 3.50028V5.83362H9.33362V3.50028C9.33362 3.19087 9.2107 2.89412 8.99191 2.67533C8.77312 2.45653 8.47637 2.33362 8.16695 2.33362C7.85753 2.33362 7.56079 2.45653 7.34199 2.67533C7.1232 2.89412 7.00028 3.19087 7.00028 3.50028V5.83362H5.83362C4.90536 5.83362 4.01512 6.20237 3.35874 6.85874C2.70237 7.51512 2.33362 8.40536 2.33362 9.33362V10.5003H25.667V9.33362C25.667 8.40536 25.2982 7.51512 24.6418 6.85874C23.9854 6.20237 23.0952 5.83362 22.167 5.83362Z"
              fill="#0E8A74"
            />
            <path
              d="M2.33362 22.1669C2.33362 23.0952 2.70237 23.9854 3.35874 24.6418C4.01512 25.2982 4.90536 25.6669 5.83362 25.6669H22.167C23.0952 25.6669 23.9854 25.2982 24.6418 24.6418C25.2982 23.9854 25.667 23.0952 25.667 22.1669V12.8336H2.33362V22.1669Z"
              fill="#0E8A74"
            />
          </svg>
          <div className="text-left ms-3">
            <span className="d-block text-black text-start">
              Change Periode
            </span>
            <small className="d-block text-light">{`${moment(
              timesession.startDate
            ).format("MMMM  DD YYYY")} - ${moment(timesession.endDate).format(
              "MMMM  DD YYYY"
            )}`}</small>
          </div>
          <i className="fa fa-caret-down text-light scale5 ms-3"></i>
        </div>
        <div
          className="input-group search-area2 d-xl-inline-flex mb-2 
        me-4 btn btn-sm xl d-flex align-items-center i-false c-pointer"
        >
          <DateRangePicker
            initialSettings={{
              startDate: timesession.startDate,
              endDate: timesession.endDate,
            }}
            style={{ display: "none" }}
            onApply={(e) => {
              let dts = e.target.value.split(" - ");
              let dt = { ...timesession };
              dt.startDate = new Date(dts[0]);
              dt.endDate = new Date(dts[1]);
              setTimesession(dt);
              setShowRange(false);
            }}
          >
            <input
              type="text"
              style={{ display: showRange ? "block" : "none" }}
              className="form-control input-daterange-timepicker"
            />
          </DateRangePicker>
        </div>
      </div>
      <div className="row mb-3 align-items-center">
        <div className="col-xl-10 col-lg-10">
          <div className="d-sm-flex  d-block align-items-center">
            <div className="d-flex align-items-center">
              <div className="media-body">
                {types.map((t, index) => (
                  <Button
                    key={index}
                    className="me-2 btn-xxs"
                    variant={t === selectedType ? "danger" : "outline-danger"}
                    onClick={() => {
                      setSelectedType(t === selectedType ? "" : t);
                    }}
                  >
                    {t.replace("_", " ")}
                  </Button>
                ))}
              </div>
            </div>
          </div>
        </div>
        <div className="col-xl-2 col-lg-2">
          <Button
            className="me-2 btn-xxs"
            variant="outline-primary"
            onClick={() => exportation()}
          >
            <i className="fas fa-download me-3"></i>
            Export to CSV
          </Button>
        </div>
      </div>
      <div className="row">
        <div className="col-lg-12">
          <div className="table-responsive dataTables_wrapper rounded">
            <table className="table customer-table display mb-4 fs-14 dataTable card-table no-footer">
              <tbody>
                {onFilterUpdate().map((log, i) => (
                  <tr key={i}>
                    <td>{moment(log.date).format("MMM DD YYYY HH:mm:ss")}</td>
                    <td>{log.authorId}</td>
                    <td>{log.action}</td>
                    <td>{log.entity}</td>
                    <td>{log.entityId}</td>
                    <td>{log.data}</td>
                    <td>{log.type}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </>
  );
};

export default Audit;
