import { API } from "../../constants";
import { interactWithAPI } from "./base";

export const getTimeoffTypes = async (_all=false) => interactWithAPI({
  method: "GET",
  url: _all ? `${API()}/Referential/AllTimeoffTypes` : `${API()}/Referential/TimeoffTypes`,
});

export const getNonRegularTimeoffTypes = async () => interactWithAPI({
  method: "GET",
  url: `${API()}/Referential/NonRegularTimeoffTypes`,
});

export const getDepartments = async () => interactWithAPI({
  method: "GET",
  url: `${API()}/Referential/Departments`,
});

export const getPayrollPeriods = async () => interactWithAPI({
  method: "GET",
  url: `${API()}/Referential/PayrollPeriods`,
});

export const getPayrollCodes = async () => interactWithAPI({
  method: "GET",
  url: `${API()}/Referential/PayrollCodes`,
});

export const getTimesheetStatuses = async (withoutInProgress) => interactWithAPI({
  method: "GET",
  url: `${API()}/Referential/TimesheetStatuses?withoutInProgress=${withoutInProgress}`,
});

export const getTimesheetEntryStatuses = async () => interactWithAPI({
  method: "GET",
  url: `${API()}/Referential/TimesheetEntryStatuses`,
});

export const getTimeoffStatuses = async () => interactWithAPI({
  method: "GET",
  url: `${API()}/Referential/TimeoffStatuses`,
});

export const getTimeoffLabels = async () => interactWithAPI({
  method: "GET",
  url: `${API()}/Referential/TimeoffLabels`,
});

export const getServiceOrders = async () => interactWithAPI({
  method: "GET",
  url: `${API()}/Referential/ServiceOrders`,
});

export const getJobs = async () => interactWithAPI({
  method: "GET",
  url: `${API()}/Referential/Jobs`,
});

export const getJobTasks = async () => interactWithAPI({
  method: "GET",
  url: `${API()}/Referential/JobTasks`,
});

export const getLaborCode = async () => interactWithAPI({
  method: "GET",
  url: `${API()}/Referential/LaborCodes`,
});

export const getCustomerNumbers = async () => interactWithAPI({
  method: "GET",
  url: `${API()}/Referential/CustomerNumber`,
});

export const getProfitCenters = async () => interactWithAPI({
  method: "GET",
  url: `${API()}/Referential/ProfitCenters`,
});
