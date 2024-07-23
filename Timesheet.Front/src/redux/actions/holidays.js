import { API } from "../../constants";
import { interactWithAPI } from "./base";

export const getHolidays = async () => interactWithAPI({
  method: "GET",
  url: `${API()}/Holiday`,
});

export const createHoliday = async (data) => interactWithAPI({
  method: "POST",
  url: `${API()}/Holiday`,
  data: data,
});

export const updateHoliday = async (data) => interactWithAPI({
  method: "PUT",
  url: `${API()}/Holiday`,
  data: data,
});

export const deleteHoliday = async (data) => interactWithAPI({
  method: "DELETE",
  url: `${API()}/Holiday`,
  data: data,
});
