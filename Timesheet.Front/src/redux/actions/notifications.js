import { API } from "../../constants";
import { interactWithAPI } from "./base";

export const sendTestNotification = async () => interactWithAPI({
  method: "GET",
  url: `${API()}/EmailTest`,
});


