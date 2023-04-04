import { API } from "../../constants";
import { interactWithAPI } from "./base";

export const getSettings = async () => interactWithAPI({
  method: "GET",
  url: `${API()}/Settings`,
});

export const updateSetting = async (data) => interactWithAPI({
  method: "PUT",
  url: `${API()}/Settings`,
  data: data,
});

export const getNotificationsSettings = async () => interactWithAPI({
  method: "GET",
  url: `${API()}/Notification`,
});

export const updateNotificationsSetting = async (data) => interactWithAPI({
  method: "PUT",
  data: data,
  url: `${API()}/Notification`,
});


