import React, { useCallback, useContext, useEffect, useState } from "react";
import { ThemeContext } from "../../context/themeContext";
import SpinnerComponent from "../../components/spinner/spinner";
import { useSelector } from "react-redux";
import EmployeeListFilter from "../team/employeeListFilter";
import NewTimeoff from "../shared/newTimeoff";
import { getMyTeam } from "../../redux/actions/employees";
import { displayError } from "../../services/toast";
import EmployeeListHeader from "../team/employeeListHeader";
import AdjustmentsDatatable from "./adjustmentsDatatable";
import EmployeeListStatusFilter from "../team/employeeListStatusFilter";

const Adjustments = () => {
  const { setTitle } = useContext(ThemeContext);
  const loading = useSelector((state) => state.auth.showLoading);

  useEffect(() => {
    setTitle("Adjustments");
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
    <div className="admin-adjustments">
      <NewTimeoff isOpen={isAddTimeoffOpen} selectedEmployeeId={selectedEmployeeId} onClose={() => setIsAddTimeoffOpen(false)} />
        <EmployeeListFilter onChange={(filter) => setFilter(filter)} />
        <EmployeeListHeader onClickDirectOrAllFilter={() => setDirect(!direct)} count={employees.length} loadDirectEmployees={direct} />
        <EmployeeListStatusFilter
          onTimeoffStatusesFilterChanged={(statuses) => setTimeoffActiveStatusesFilter(statuses)}
          onTimesheetStatusesFilterChanged={(statuses) => setTimesheetActiveStatusesFilter(statuses)}
        />
        <AdjustmentsDatatable
          onAddClick={(employeeId) => {
            console.log(JSON.stringify(employeeId));
            setSelectedEmployeeId(employeeId);
            setIsAddTimeoffOpen(true);
          }}
          onFilterUpdate={onFilterUpdate}
        />
    </div>
  );
};

export default Adjustments;