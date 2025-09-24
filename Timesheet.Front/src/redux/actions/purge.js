import { API } from "../../constants";
import { interactWithAPI } from "./base";

export const purgeTables = async () => interactWithAPI({
  method: "DELETE",
  url: `${API()}/purge/purge-old`,
});
