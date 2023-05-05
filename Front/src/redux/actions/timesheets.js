import { API } from "../../constants";
import { interactWithAPI } from "./base";
import instance from "../../services/axiosInstance";

export const getTimesheetEmployeeById = async (id, employee) => interactWithAPI({
  method: "GET",
  url: `${API()}/Timesheet/${id}/Employee/${employee}`,
});

export const getTimesheetEmployeeSummaryByDate = async (id, employee) => interactWithAPI({
    method: "GET",
    url: `${API()}/Timesheet/${id}/SummaryByDate/Employee/${employee}`,
});

export const getTimesheetEmployeeSummaryByPayroll = async (id, employee) => interactWithAPI({
  method: "GET",
  url: `${API()}/Timesheet/${id}/SummaryByPayroll/Employee/${employee}`,
});

export const getTimesheetEmployeeHistory = async (employee) => interactWithAPI({
  method: "GET",
  url: `${API()}/Timesheet/History/Employee/${employee}`,
});

export const getTimesheetEmployeeEntriesHistory = async (employee, date) => interactWithAPI({
  method: "GET",
  url: `${API()}/Timesheet/History/Entries/Employee/${employee}?start=${date}`,
});

export const getCurrentEmployeeTimesheetPeriod = async (employee) => interactWithAPI({
  method: "GET",
  url: `${API()}/Timesheet/CurrentTimesheetPeriod/Employee/${employee}`,
});

export const getPendingTimesheets = async (direct, page) => interactWithAPI({
  method: "GET",
  url: `${API()}/Employee/Timesheet/Pending?directReport=${direct}&page=${page}`,
});

export const getOrphanTimesheets = async (direct, page) => interactWithAPI({
  method: "GET",
  url: `${API()}/Employee/Timesheet/Orphan?directReport=${direct}&page=${page}`,
});

export const addTimesheetEntry = async (data) => interactWithAPI({
  method: "POST",
  url: `${API()}/Timesheet/Entries`,
  data: data,
});

export const submitTimesheet = async (data) => interactWithAPI({
  method: "PUT",
  url: `${API()}/Timesheet/Submit`,
  data: data,
});

export const approveTimesheet = async (data) => interactWithAPI({
  method: "PUT",
  url: `${API()}/Timesheet/Approve`,
  data: data,
});

export const rejectTimesheet = async (data) => interactWithAPI({
  method: "PUT",
  url: `${API()}/Timesheet/Reject`,
  data: data,
});

export const finalizeTimesheet = async (data) => interactWithAPI({
  method: "PUT",
  url: `${API()}/Timesheet/Finalize`,
  data: data,
});

export const addTimesheetException = async (data) => interactWithAPI({
  method: "POST",
  url: `${API()}/Timesheet/Exceptions`,
  data: data,
});

export const exportPeriod = async (period, department, employeeId) => {
  let criteriaParams = department ? `department=${department}` : "";
  criteriaParams = criteriaParams ? `${criteriaParams}&` : criteriaParams;
  criteriaParams = employeeId ? `${criteriaParams}employeeId=${employeeId}` : criteriaParams;
  criteriaParams = criteriaParams ? `?${criteriaParams}` : criteriaParams;

  var config = {
    method: "GET",
    url: `${API()}/Timesheet/${period}/Export${criteriaParams}`,
  };

    // Creating an anchor(a) tag of HTML
    const a = document.createElement("a");

    // Passing the blob downloading url
    a.setAttribute("href", config.url);

    // Performing a download with click
    a.click();

    return new Promise(async (resolve, reject) => {
      await instance(config)
        .then((res) => {
          resolve(res.data);
        })
        .catch((err) => {
          reject(err);
        });
    });
};

export const exportPeriodAfterFinalize = async (period) => {
  var config = {
    method: "GET",
    url: `${API()}/Timesheet/${period}/Export/AfterFinalize`,
  };

  // Creating an anchor(a) tag of HTML
  const a = document.createElement("a");

  // Passing the blob downloading url
  a.setAttribute("href", config.url);

  // Performing a download with click
  a.click();

  return new Promise(async (resolve, reject) => {
    await instance(config)
      .then((res) => {
        resolve(res.data);
      })
      .catch((err) => {
        reject(err);
      });
  });
};

export const review = async (period) => interactWithAPI({
  method: "GET",
  url: `${API()}/Timesheet/Review?payrollPeriod=${period}&itemsPerpage=100000`,
});

export const deleteTimesheet = async (data) => interactWithAPI({
  method: "DELETE",
  url: `${API()}/Timesheet/Entries`,
  data: data,
});

export const updateComment = async (data) => interactWithAPI({
  method: "PUT",
  url: `${API()}/Timesheet/AddComment`,
  data: data,
});