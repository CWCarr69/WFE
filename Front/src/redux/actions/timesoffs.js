import { API } from "../../constants";
import instance from "../../services/axiosInstance";
import { interactWithAPI } from "./base";

export const getSettings = async () => interactWithAPI({
  method: "GET",
  url: `${API()}/Settings`,
});

export const getTimeoffsEmployeeHistory = async (employee, page) => interactWithAPI({
  method: "GET",
  url: `${API()}/Employee/${employee}/Timeoff/History?page=${page}`,
});

export const getTimeoffsEmployeeEntriesHistory = async (employee,date) => interactWithAPI({
  method: "GET",
  url: `${API()}/Employee/${employee}/Timeoff/History/Entries?start=${date}`,
});

export const getTimeoffsEmployeeMonthStatistics = async (employee) => interactWithAPI({
  method: "GET",
  url: `${API()}/Employee/${employee}/Timeoff/MonthsStatistics`,
});

export const getTimeoffEmployeeDetails = async (employee, timeoffId) => interactWithAPI({
  method: "GET",
  url: `${API()}/Employee/${employee}/Timeoff/${timeoffId}`,
});

export const getTimeoffEmployeeSummary = async (employee, timeoffId) => interactWithAPI({
  method: "GET",
  url: `${API()}/Employee/${employee}/Timeoff/${timeoffId}/Summary`,
});

export const getPendingTimesoffs = async (direct) => interactWithAPI({
  method: "GET",
  url: `${API()}/Employee/Timeoff/Pending?directReport=${direct}`,
});

export const addTimeoff = async (data)=> interactWithAPI({
  method: "POST",
  url: `${API()}/Employee/timeoff`,
  data: data,
});

export const submitTimeoff = async (data) => interactWithAPI({
  method: "PUT",
  url: `${API()}/Employee/timeoff/Submit`,
  data: data,
});

export const deleteTimeoff = async (timeoff, employee) => interactWithAPI({
  method: "DELETE",
  url: `${API()}/Employee/timeoff/${timeoff}?employeeId=${employee}`,
});

export const rejectTimeOff = async (data) => interactWithAPI({
  method: "PUT",
  url: `${API()}/Employee/timeoff/Reject`,
  data: data,
});

export const approveTimeOff = async (data) => interactWithAPI({
  method: "PUT",
  url: `${API()}/Employee/timeoff/Approve`,
  data: data,
});

export const addEntry = async (data) => interactWithAPI({
  method: "POST",
  url: `${API()}/Employee/timeoff/Entry`,
  data: data,
});

export const updateEntry = async (data) => interactWithAPI({
  method: "PUT",
  url: `${API()}/Employee/timeoff/Entry`,
  data: data,
});

export const deleteEntry = async (timeoff, entry, employee) => interactWithAPI({
  method: "DELETE",
  url: `${API()}/Employee/timeoff/${timeoff}/Entry/${entry}?employeeId=${employee}`,
});

export const updateComment = async (data) => interactWithAPI({
  method: "PUT",
  url: `${API()}/Employee/timeoff/${data.timeoffId}/AddComment`,
  data: data,
});