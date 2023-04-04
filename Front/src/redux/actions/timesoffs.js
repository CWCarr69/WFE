import { API } from "../../constants";
import instance from "../../services/axiosInstance";

export const getTimeoffsEmployeeHistory = async (employee, approval, page) => {
  var config = {
    method: "GET",
    url: `${API()}/Employee/${employee}/Timeoff/History?page=${page}&requireApproval=${approval}`,
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

export const getTimeoffsEmployeeEntriesHistory = async (
  employee,
  approval,
  date
) => {
  var config = {
    method: "GET",
    url: `${API()}/Employee/${employee}/Timeoff/History/Entries?start=${date}&requireApproval=${approval}`,
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

export const getTimeoffsEmployeeMonthStatistics = async (employee) => {
  var config = {
    method: "GET",
    url: `${API()}/Employee/${employee}/Timeoff/MonthsStatistics`,
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

export const getTimeoffEmployeeDetails = async (employee, timeoffId) => {
  var config = {
    method: "GET",
    url: `${API()}/Employee/${employee}/Timeoff/${timeoffId}`,
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

export const getTimeoffEmployeeSummary = async (employee, timeoffId) => {
  var config = {
    method: "GET",
    url: `${API()}/Employee/${employee}/Timeoff/${timeoffId}/Summary`,
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

export const getPendingTimesoffs = async (direct, page) => {
  var config = {
    method: "GET",
    url: `${API()}/Employee/Timeoff/Pending?directReport=${direct}&page=${page}`,
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

export const addTimeoff = async (data) => {
  var config = {
    method: "POST",
    url: `${API()}/Employee/timeoff`,
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

export const submitTimeoff = async (data) => {
  var config = {
    method: "PUT",
    url: `${API()}/Employee/timeoff/Submit`,
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

export const deleteTimeoff = async (timeoff, employee) => {
  var config = {
    method: "DELETE",
    url: `${API()}/Employee/timeoff/${timeoff}?employeeId=${employee}`,
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

export const rejectTimeOff = async (data) => {
  var config = {
    method: "PUT",
    url: `${API()}/Employee/timeoff/Reject`,
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

export const approveTimeOff = async (data) => {
  var config = {
    method: "PUT",
    url: `${API()}/Employee/timeoff/Approve`,
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

export const addEntry = async (data) => {
  var config = {
    method: "POST",
    url: `${API()}/Employee/timeoff/Entry`,
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

export const updateEntry = async (data) => {
  var config = {
    method: "PUT",
    url: `${API()}/Employee/timeoff/Entry`,
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

export const deleteEntry = async (timeoff, entry, employee) => {
  var config = {
    method: "DELETE",
    url: `${API()}/Employee/timeoff/${timeoff}/Entry/${entry}?employeeId=${employee}`,
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

export const updateComment = async (data) => {
  var config = {
    method: "PUT",
    url: `${API()}/Employee/timeoff/${data.timeoffId}/AddComment`,
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
