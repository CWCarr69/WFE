import React, { useCallback, useContext, useEffect, useState } from "react";
import { ThemeContext } from "../../context/themeContext";
import { getMyTeam } from "../../redux/actions/employees";
import SpinnerComponent from "../../components/spinner/spinner";
import { useSelector } from "react-redux";
import NewTimeoff from "../shared/newTimeoff";
import { displayError } from "../../services/toast";
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

  const [employees, setEmployees] = useState([]);

  const [filter, setFilter] = useState("");
  const [direct, setDirect] = useState(true);
  const [timesheetActiveStatusesFilter, setTimesheetActiveStatusesFilter] = useState([]);
  const [timeoffActiveStatusesFilter, setTimeoffActiveStatusesFilter] = useState([]);
  const [isAddTimeoffOpen, setIsAddTimeoffOpen] = useState(false);

  const [selectedEmployeeId, setSelectedEmployeeId] = useState(null);

  const fetchTeamEmployees = useCallback(async () => {
    await getMyTeam(direct)
      .then((resp) => setEmployees(resp.items))
      .catch((err) => displayError(err, "Error while retrieving fetching team data"))
  }, [direct]);

  const onFilterUpdate = useCallback(() => {
    var FilteredByStatuses = employees.filter(
      (d) => timeoffActiveStatusesFilter.includes(d.lastTimeoffStatusString) || timesheetActiveStatusesFilter.includes(d.lastTimesheetStatusString)
    );

    var itemFilter = (employees) => employees.filter((d) => d.fullName.toLowerCase().concat(d.employeeId).includes(filter.toLowerCase()));

    return timeoffActiveStatusesFilter.length + timesheetActiveStatusesFilter.length > 0 ? itemFilter(FilteredByStatuses) : itemFilter(employees);
  }, [filter, employees, timesheetActiveStatusesFilter, timeoffActiveStatusesFilter]);

  useEffect(() => fetchTeamEmployees(), [fetchTeamEmployees, isAddTimeoffOpen]);

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
      <NewTimeoff isOpen={isAddTimeoffOpen} selectedEmployeeId={selectedEmployeeId} onClose={() => setIsAddTimeoffOpen(false)}/>
      <EmployeeListFilter onChange={(filter) => setFilter(filter)} />
      <EmployeeListHeader onClickDirectOrAllFilter={() => setDirect(!direct)} count={employees.length} loadDirectEmployees={direct} />
      <EmployeeListStatusFilter 
        onTimeoffStatusesFilterChanged={(statuses) => setTimeoffActiveStatusesFilter(statuses)}
        onTimesheetStatusesFilterChanged={(statuses) => setTimesheetActiveStatusesFilter(statuses)}
        />
      <EmployeeListDatatable 
        onAddClick={(employeeId) => {
          console.log(JSON.stringify(employeeId));
          setSelectedEmployeeId(employeeId);
          setIsAddTimeoffOpen(true);
        }}
        onFilterUpdate={onFilterUpdate}
      />
    </>
  );
};
export default EmployeeList;

