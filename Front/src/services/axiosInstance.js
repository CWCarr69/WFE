import axios from "axios";
import TokenService from "./tokenService";
import { API } from "../constants";
import { LOADING_TOGGLE_ACTION } from "../redux/actions/auth";
import { store } from "../redux/store";

const instance = axios.create({
  baseURL: API(),
  headers: {
    "Content-Type": "application/json",
    "ngrok-skip-browser-warning": "test",
  },
});

instance.interceptors.request.use(
  (config) => {
    const token = TokenService.getLocalAccessToken();
    if (token) {
      config.headers["Authorization"] = "Bearer " + token;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

instance.interceptors.response.use(
  (res) => {
    return res;
  },
  async (err) => {
    const originalConfig = err.config;

    if (originalConfig.url !== "/Authentication" && err.response) {
      // Access Token was expired
      if (err.response.status === 401 && !originalConfig._retry) {
        originalConfig._retry = true;
        localStorage.removeItem("userDetails");
        window.location.replace("/");
      }
    }

    return Promise.reject(err);
  }
);

export default instance;
