import { API } from "../../constants";
import { interactWithAPI } from "./base";


export const getReports = async () => interactWithAPI({
  method: "GET",
  url: `${API()}/Reports`,
});
