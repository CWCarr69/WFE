import React, {
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";
import { ThemeContext } from "../../context/themeContext";
import { displayError, displaySuccess } from "../../services/toast";
import {
  addTimesheetException,
  review,
} from "../../redux/actions/timesheets";
import TimesheetStatusFilter from "./timesheetStatusFilter";
import TimesheetExportPanel from "./timesheetExportPanel";
import TimesheetFilter from "./timesheetFilter";
import TimesheetFinalizeAction from "./timesheetFinalizeAction";
import SpinnerComponent from "../../components/spinner/spinner";
import { useSelector } from "react-redux";
import _ from "lodash";
import moment from "moment";
import { DefaultGlobalDateFormat } from "../../services/util";
import TimesheetDatatable from "./timesheetDatatable";
import OrphanTimesheet from "./orphanTimesheet";


const TimesSheet = () => {
  const { setTitle } = useContext(ThemeContext);
  const loading = useSelector((state) => state.auth.showLoading);

  const user = useSelector((state) => state.auth.auth);
  
  const [selectedPeriod, setSelectedPeriod] = useState(null);
  const [selectedEmployee, setSelectedEmployee] = useState("");
  const [selectedDepartment, setSelectedDepartment] = useState("");

  const [timesheetEntryActiveStatusesFilter, setTimesheetEntryActiveStatusesFilter] = useState([]);

  const [reveiwData, setReveiwData] = useState({
    authorizedActions: [],
    data: {items: [] },
    otherData: { TotalQuantity: 0 },
  });

  const adaptFectchedEmployeeEntries = (item, array) => {
    item.data.entries.forEach((entry) => {
      array.push({
        id: item.data.employeeId,
        entryId: entry.id,
        employee: item.data.fullname,
        department: entry.department,
        work: moment(entry.workDate).format(DefaultGlobalDateFormat),
        payrollCod: entry.payrollCode,
        quantity: entry.quantity,
        description: entry.description,
        service: `${entry.serviceOrderNumber ?? ""} ${ entry.serviceOrderDescription ? " - " + entry.serviceOrderDescription : ""}`,
        job: `${entry.jobNumber ?? ""} ${ entry.jobDescription ? " - " + entry.jobDescription : "" }`,
        task: `${entry.jobTaskNumber ?? ""} ${ entry.jobTaskDescription ? " - " + entry.jobTaskDescription : "" }`,
        total: item.data.total,
        labor: entry.laborCode,
        center: entry.profitCenterNumber ?? item.data.defaultProfitCenter,
        area: entry.workArea,
        timesheetStatus: item.data.statusName == "FINALIZED" ? item.data.statusName : item.data.partialStatusName.replaceAll("_", " "),
        status: entry.statusName.replaceAll("_", " "),
        overtime: item.data.overtime,
        authorizedActions: item.authorizedActions,
        timesheetId: item.data.timesheetId,
        delete: entry.payrollCode != "REGULAR" && entry.payrollCode != "OVERTIME",
        deleteAction: () => createTimesheetException(entry),
        isOrphan: entry.isOrphan,
        isRejected: entry.isRejected,
        isApproved: entry.isApproved,
        urlToTimesheet: `/timesheets/${selectedPeriod}/employee/${item.data.employeeId}/${new Date(entry.workDate).getTime()}`,
        urlToEmployee: `/profile/${item.data.employeeId}`,
        subRows: undefined,
      });
    });
  }

  const fetchReviewData = useCallback(async () => {
    if (selectedPeriod) {
      let dt = await review(selectedPeriod);
      var array = [];
      dt.data.items.forEach((item) => adaptFectchedEmployeeEntries(item, array));
      setReveiwData({
        data: { items: array },
        otherData: dt.data.otherData,
        authorizedActions: dt.authorizedActions,
      });
    }
  }, [selectedPeriod]);

  useEffect(() => fetchReviewData(), [fetchReviewData]);

  const onFilterUpdate = useCallback(() => {
    let arrayData = reveiwData.data.items;

    let empFilter = arrayData.filter((d) => d.id === selectedEmployee);

    let depFilter = arrayData.filter((d) => d.department === selectedDepartment);

    let array =
      selectedEmployee !== "" && selectedDepartment !== ""
        ? _.intersection(empFilter, depFilter)
        : selectedEmployee !== "" && selectedDepartment === ""
        ? empFilter
        : selectedEmployee === "" && selectedDepartment !== ""
        ? depFilter
        : arrayData;

    var resp = timesheetEntryActiveStatusesFilter.length > 0
      ? array.filter((d) => timesheetEntryActiveStatusesFilter.includes(d.status))
      : array;

	  return resp;
	}, [timesheetEntryActiveStatusesFilter, reveiwData, selectedEmployee, selectedDepartment]);

  const data = useMemo(() => onFilterUpdate(), [onFilterUpdate]);

  useEffect(() => {
    setTitle("Review Timesheet");
  }, [setTitle]);

  const createTimesheetException = async (entry) => {
    await addTimesheetException({
        employeeId: entry.employeeId,
        timesheetEntryId: entry.id,
        timesheetId: entry.timesheetId,
        isHoliday: entry.isGlobalHoliday
      })
      .then((res) => {
        displaySuccess("Successful add of timesheet Exception");
        fetchReviewData();
      })
      .catch((err) => displayError(err, "Error adding timesheet exception"));
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
          <div className="w-100 table-responsive">
            {data.length > 0 && 
            <TimesheetFinalizeAction 
              canFinalize = {reveiwData.authorizedActions.find((a) => a.name === "FINALIZE") }
              currentTimesheetId = {data[0].timesheetId}
              onDone = {fetchReviewData}
            />}
            <TimesheetFilter 
              onPayrollPeriodChanged = {(e) => setSelectedPeriod(e)}
              onEmployeeChanged = {(e) => setSelectedEmployee(e.value)}
              onDepartmentChanged = {(e) => setSelectedDepartment(e.value)}
              totalQuantity= {reveiwData.otherData && reveiwData.otherData.TotalQuantity}
            />
            <div className="row mb-3 mt-2 align-items-center">
              <TimesheetStatusFilter onTimesheetEntryStatusesFilterChanged={(statuses) => setTimesheetEntryActiveStatusesFilter(statuses)}/>
              <TimesheetExportPanel 
                showFinalizeButton = { user.isAdministrator && data.find((d) => d.timesheetStatus.toLowerCase() === "finalized") }
                employee={selectedEmployee}
                department={selectedDepartment}
                period={selectedPeriod}
              />
              <TimesheetDatatable 
                onDecisionTaken = {fetchReviewData}
                data = {data}
              />
            </div>
          </div>
        </div>
      </div>
      <div className="row">
        <div id="orphan-timesheets" className="col-12">
          <OrphanTimesheet />
        </div>
      </div>
    </>
  );
};

export default TimesSheet;
