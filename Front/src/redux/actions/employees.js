import { API } from "../../constants";
import instance from "../../services/axiosInstance";

export const getMyTeam = async (direct) => {
  var config = {
    method: "GET",
    url: `${API()}/Employee/Team?directReport=${direct}`,
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

export const getEmployee = async (page) => {
  var config = {
    method: "GET",
    url: `${API()}/Employee`,
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

export const getEmployeeById = async (id) => {
  var config = {
    method: "GET",
    url: `${API()}/Employee/${id}`,
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

export const getEmployeeBenefits = async (id) => {
  var config = {
    method: "GET",
    url: `${API()}/Employee/${id}/Benefits`,
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

export const getEmployeeCalculatedBenefits = async (id) => {
  var config = {
    method: "GET",
    url: `${API()}/Employee/${id}/CalculatedBenefits`,
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

export const updateBenefits = async (data) => {
  var config = {
    method: "PUT",
    url: `${API()}/Employee/${data.employeeId}/Benefits`,
    data: data,
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

export const getEmployeeApprovers = async (id) => {
  var config = {
    method: "GET",
    url: `${API()}/Employee/${id}/Approvers`,
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
