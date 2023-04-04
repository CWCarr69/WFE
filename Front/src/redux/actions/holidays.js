import { API } from "../../constants";
import instance from "../../services/axiosInstance";

export const getHolidays = async () => {
  var config = {
    method: "GET",
    url: `${API()}/Holiday`,
  };

  return new Promise(async (resolve, reject) => {
    await instance(config)
      .then((res) => {
        resolve(res.data);
      })
      .catch((err) => {
        reject(err);
      });
  });
};

export const createHoliday = async (data) => {
  var config = {
    method: "POST",
    url: `${API()}/Holiday`,
    data: data,
  };

  return new Promise(async (resolve, reject) => {
    await instance(config)
      .then((res) => {
        resolve(res.data);
      })
      .catch((err) => {
        console.log(err);
        reject(err);
      });
  });
};

export const updateHoliday = async (data) => {
  var config = {
    method: "PUT",
    url: `${API()}/Holiday`,
    data: data,
  };

  return new Promise(async (resolve, reject) => {
    await instance(config)
      .then((res) => {
        resolve(res.data);
      })
      .catch((err) => {
        console.log(err);
        reject(err);
      });
  });
};

export const deleteHoliday = async (data) => {
  var config = {
    method: "DELETE",
    url: `${API()}/Holiday`,
    data: data,
  };

  return new Promise(async (resolve, reject) => {
    await instance(config)
      .then((res) => {
        resolve(res.data);
      })
      .catch((err) => {
        console.log(err);
        reject(err);
      });
  });
};
