import { API } from "../../constants";
import { interactWithAPI } from "./base";


export const getMyTeam = async (direct) => interactWithAPI({
  method: "GET",
  url: `${API()}/Employee/Team?directReport=${direct}`,
});

export const getMyLightTeam = async (direct) => interactWithAPI({
  method: "GET",
  url: `${API()}/Employee/LightTeam?directReport=${direct}`,
});

export const getEmployee = async () => interactWithAPI({
  method: "GET",
  url: `${API()}/Employee`,
});

export const getEmployeeById = async (id) => interactWithAPI({
  method: "GET",
  url: `${API()}/Employee/${id}`,
});

export const getEmployeeBenefits = async (id) => interactWithAPI({
  method: "GET",
  url: `${API()}/Employee/${id}/Benefits`,
});

export const getEmployeeCalculatedBenefits = async (id) => interactWithAPI({
  method: "GET",
  url: `${API()}/Employee/${id}/CalculatedBenefits`,
});


export const updateBenefits = async (data) => interactWithAPI({
  method: "PUT",
  url: `${API()}/Employee/${data.employeeId}/Benefits`,
  data: data,
});

export const getEmployeeApprovers = async (id) => interactWithAPI({
  method: "GET",
  url: `${API()}/Employee/${id}/Approvers`,
});
