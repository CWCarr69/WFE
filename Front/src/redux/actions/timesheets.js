import { API } from "../../constants";
import instance from "../../services/axiosInstance";

export const getTimesheetEmployeeById = async (id, employee) => {
  var config = {
    method: "GET",
    url: `${API()}/Timesheet/${id}/Employee/${employee}`,
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

export const getTimesheetEmployeeSummaryByDate = async (id, employee) => {
  var config = {
    method: "GET",
    url: `${API()}/Timesheet/${id}/SummaryByDate/Employee/${employee}`,
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

export const getTimesheetEmployeeSummaryByPayroll = async (id, employee) => {
  var config = {
    method: "GET",
    url: `${API()}/Timesheet/${id}/SummaryByPayroll/Employee/${employee}`,
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

export const getTimesheetEmployeeHistory = async (employee) => {
  var config = {
    method: "GET",
    url: `${API()}/Timesheet/History/Employee/${employee}`,
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

export const getTimesheetEmployeeEntriesHistory = async (employee, date) => {
  var config = {
    method: "GET",
    url: `${API()}/Timesheet/History/Entries/Employee/${employee}?start=${date}`,
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

export const getCurrentEmployeeTimesheetPeriod = async (employee) => {
  var config = {
    method: "GET",
    url: `${API()}/Timesheet/CurrentTimesheetPeriod/Employee/${employee}`,
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

export const getPendingTimessheets = async (direct, page) => {
  var config = {
    method: "GET",
    url: `${API()}/Employee/Timesheet/Pending?directReport=${direct}&page=${page}`,
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

export const addTimesheetEntry = async (data) => {
  var config = {
    method: "POST",
    url: `${API()}/Timesheet/Entries`,
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

export const submitTimesheet = async (data) => {
  var config = {
    method: "PUT",
    url: `${API()}/Timesheet/Submit`,
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

export const approveTimesheet = async (data) => {
  var config = {
    method: "PUT",
    url: `${API()}/Timesheet/Approve`,
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

export const rejectTimesheet = async (data) => {
  var config = {
    method: "PUT",
    url: `${API()}/Timesheet/Reject`,
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

export const finalizeTimesheet = async (data) => {
  var config = {
    method: "PUT",
    url: `${API()}/Timesheet/Finalize`,
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

export const addTimesheetException = async (data) => {
  var config = {
    method: "POST",
    url: `${API()}/Timesheet/Exceptions`,
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

export const exportPeriod = async (period, department, employeeId) => {
  let criteriaParams = department ? `department=${department}` : "";
  criteriaParams = criteriaParams ? `${criteriaParams}&` : criteriaParams;
  criteriaParams = employeeId ? `${criteriaParams}employeeId=${employeeId}` : criteriaParams;
  criteriaParams = criteriaParams ? `?${criteriaParams}` : criteriaParams;

  var config = {
    method: "GET",
    url: `${API()}/Timesheet/${period}/Export${criteriaParams}`,
  };

    // Creating an anchor(a) tag of HTML
    const a = document.createElement("a");

    // Passing the blob downloading url
    a.setAttribute("href", config.url);

    // Performing a download with click
    a.click();

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

export const exportPeriodAfterFinalize = async (period) => {
  var config = {
    method: "GET",
    url: `${API()}/Timesheet/${period}/Export/AfterFinalize`,
  };

  // Creating an anchor(a) tag of HTML
  const a = document.createElement("a");

  // Passing the blob downloading url
  a.setAttribute("href", config.url);

  // Performing a download with click
  a.click();

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

export const review = async (period) => {
  var config = {
    method: "GET",
    url: `${API()}/Timesheet/Review?payrollPeriod=${period}&itemsPerpage=100000`,
  };

  return await instance(config)
    .then((res) => {
      return res.data;
    })
    .catch((err) => {
      return err;
    });
};

export const deleteTimesheet = async (data) => {
  var config = {
    method: "DELETE",
    url: `${API()}/Timesheet/Entries`,
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

export const updateComment = async (data) => {
  var config = {
    method: "PUT",
    url: `${API()}/Employee/timesheet/AddComment`,
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