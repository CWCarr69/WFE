import React, {
  useCallback,
  useContext,
  useEffect,
  useState,
} from "react";
import { ThemeContext } from "../../context/themeContext";
import { getMyTeam } from "../../redux/actions/employees";
import { getTimeoffTypes } from "../../redux/actions/referentials";
import SpinnerComponent from "../../components/spinner/spinner";
import { useSelector } from "react-redux";
import NewTimeOff from "../timeoff/newTimeOff";
import { enumerateDaysBetweenDates } from "../../services/util";
import { displayError, displaySuccess } from "../../services/toast";
import moment from "moment";
import { addTimeoff } from "../../redux/actions/timesoffs";
import EmployeeListFilter from "./employeeListFilter";
import EmployeeListHeader from "./employeeListHeader";
import EmployeeListDatatable from "./employeeListDatatable";
import EmployeeListStatusFilter from "./employeeListStatusFilter";


const EmployeeList = () => {
  const { setTitle } = useContext(ThemeContext);
  const loading = useSelector((state) => state.auth.showLoading);

  useEffect(() => {
    setTitle("Timesheets");
  }, [setTitle]);

  const [data, setData] = useState({});
  const [items, setItems] = useState([]);

  const [filter, setFilter] = useState("");
  const [direct, setDirect] = useState(true);
  const [timesheetActiveStatusesFilter, setTimesheetActiveStatusesFilter] = useState([]);
  const [timeoffActiveStatusesFilter, setTimeoffActiveStatusesFilter] = useState([]);

  const [types, setTypes] = useState([]);
  const [openAddTimeOff, setOpenAddTimeOff] = useState(false);
  const [timeoff, setTimeoff] = useState({});
  const [selectedEmployeeId, setSelectedEmployeeId] = useState(null);

  const fetchData = useCallback(async () => {
    await getMyTeam(direct)
      .then((resp) => {
        setData(resp);
        setItems(resp.items);
      })
      .catch((err) => displayError(err, "Error while retrieving fetching team data"))
  }, [direct]);

  const fetchTypes = async () => {
    await getTimeoffTypes(true)
      .then((resp) => setTypes(resp))
      .catch((err) => displayError(err, "Error while getting Time off types"))
  };

  const createTimeOff = async () => {
    var dates = enumerateDaysBetweenDates(
      moment(timeoff.start),
      moment(timeoff.end)
    );

    var dataToSave = {
      requestStartDate: timeoff.start,
      requestEndDate: timeoff.end,
      employeeId: selectedEmployeeId,
      employeeComment: timeoff.employeeComment,
      entries: dates.map((d) => {
        return {
          employeeId: selectedEmployeeId,
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

  const onFilterUpdate = useCallback(() => {
    var FilteredByStatuses = items.filter(
      (d) => timeoffActiveStatusesFilter.includes(d.lastTimeoffStatusString) || timesheetActiveStatusesFilter.includes(d.lastTimesheetStatusString)
    );

    var itemFilter = (employees) => employees.filter((d) => d.fullName.toLowerCase().concat(d.employeeId).includes(filter.toLowerCase()));

    return timeoffActiveStatusesFilter.length + timesheetActiveStatusesFilter.length > 0 ? itemFilter(FilteredByStatuses) : itemFilter(items);
  }, [filter, items, timesheetActiveStatusesFilter, timeoffActiveStatusesFilter]);

  useEffect(() => {
    fetchData();
    fetchTypes();
  }, [fetchData]);

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
      <EmployeeListFilter onChange={(filter) => setFilter(filter)} />
      <EmployeeListHeader onClickDirectOrAllFilter={() => setDirect(!direct)} count={data.length} loadDirectEmployees={direct} />
      <EmployeeListStatusFilter 
        onTimeoffStatusesFilterChanged={(statuses) => setTimeoffActiveStatusesFilter(statuses)}
        onTimesheetStatusesFilterChanged={(statuses) => setTimesheetActiveStatusesFilter(statuses)}
        />
      <EmployeeListDatatable 
        onAddClick={(employeeId) => {
          setSelectedEmployeeId(employeeId);
          setOpenAddTimeOff(true);
        }}
        onFilterUpdate={onFilterUpdate}
        />
    </>
  );
};
export default EmployeeList;

