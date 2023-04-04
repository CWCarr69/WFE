import { API } from "../../constants";
import instance from "../../services/axiosInstance";

export const getTimeoffTypes = async (_all=false) => {
  var config = {
    method: "GET",
    url: _all ? `${API()}/Referential/AllTimeoffTypes` : `${API()}/Referential/TimeoffTypes`,
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

export const getNonRegularTimeoffTypes = async () => {
  var config = {
    method: "GET",
    url: `${API()}/Referential/NonRegularTimeoffTypes`,
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

export const getDepartments = async () => {
  var config = {
    method: "GET",
    url: `${API()}/Referential/Departments`,
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

export const getPayrollPeriods = async () => {
  var config = {
    method: "GET",
    url: `${API()}/Referential/PayrollPeriods`,
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

export const getPayrollCodes = async () => {
  var config = {
    method: "GET",
    url: `${API()}/Referential/PayrollCodes`,
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

export const getTimesheetStatuses = async () => {
  var config = {
    method: "GET",
    url: `${API()}/Referential/TimesheetStatuses`,
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

export const getTimesheetEntryStatuses = async () => {
  var config = {
    method: "GET",
    url: `${API()}/Referential/TimesheetEntryStatuses`,
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

export const getTimeoffStatuses = async () => {
  var config = {
    method: "GET",
    url: `${API()}/Referential/TimeoffStatuses`,
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

export const getTimeoffLabels = async () => {
  var config = {
    method: "GET",
    url: `${API()}/Referential/TimeoffLabels`,
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

export const getServiceOrders = async () => {
  var config = {
    method: "GET",
    url: `${API()}/Referential/ServiceOrders`,
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

export const getJobs = async () => {
  var config = {
    method: "GET",
    url: `${API()}/Referential/Jobs`,
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

export const getJobTasks = async () => {
  var config = {
    method: "GET",
    url: `${API()}/Referential/JobTasks`,
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

export const getLaborCode = async () => {
  var config = {
    method: "GET",
    url: `${API()}/Referential/LaborCodes`,
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

export const getCustomerNumbers = async () => {
  var config = {
    method: "GET",
    url: `${API()}/Referential/CustomerNumber`,
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

export const getProfitCenters = async () => {
  var config = {
    method: "GET",
    url: `${API()}/Referential/ProfitCenters`,
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
