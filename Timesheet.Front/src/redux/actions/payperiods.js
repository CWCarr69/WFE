import { API } from "../../constants";
import { interactWithAPI } from "./base";

export const payPeriodsCreate = async (data) => interactWithAPI({
  method: "POST",
  url: `${API()}/payperiods/create`,
  data: data,
});