import React, { useCallback, useContext, useEffect, useState } from "react";
import { Button, Col, Row } from "react-bootstrap";
import "react-datepicker/dist/react-datepicker.css";
import { toast } from "react-toastify";
import { ThemeContext } from "../../context/themeContext";
import { getEmployeeById } from "../../redux/actions/employees";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import {
  getCustomerNumbers,
  getJobs,
  getJobTasks,
  getLaborCode,
  getPayrollPeriods,
  getProfitCenters,
  getServiceOrders,
  getTimesheetEntryStatuses,
} from "../../redux/actions/referentials";
import Select from "react-select";
import CreatableSelect from "react-select/creatable";
import { addTimesheetEntry } from "../../redux/actions/timesheets";
import SpinnerComponent from "../../components/spinner/spinner";
import { useSelector } from "react-redux";

const NewEntry = ({ match }) => {
  const { setTitle } = useContext(ThemeContext);
  const loading = useSelector((state) => state.auth.showLoading);

  const [employee, setEmployee] = useState({});

  const [data, setData] = useState({
    employeeId: match.params.id,
  });
  const [periods, setPeriods] = useState([]);
  const [servicesOrder, setServicesOrder] = useState([]);
  const [jobs, setJobs] = useState([]);
  const [jobTasks, setJobTasks] = useState([]);
  const [labors, setLabors] = useState([]);
  const [customersNumber, setCustomersNumber] = useState([]);
  const [profitCenters, setProfitCenters] = useState([]);
  const [status, setStatus] = useState([]);

  useEffect(() => {
    setTitle(`${employee.fullName ? employee.fullName : ""} - Timesheet`);
  }, [setTitle, employee]);

  const fetchEmployee = useCallback(async () => {
    await getEmployeeById(match.params.id)
      .then((resp) => {
        setEmployee(resp);
      })
      .catch((err) => {
        toast.error(
          err.response.data.message ? err.response.data.message : "Error"
        );
      });
  }, [match.params.id]);

  const fetchPayrollPeriods = async () => {
    await getPayrollPeriods()
      .then((resp) => {
        setPeriods(
          resp.map((e) => {
            return {
              value: e.code,
              label: e.code,
            };
          })
        );
      })
      .catch((err) => {
        toast.error(
          err.response.data.message ? err.response.data.message : "Error"
        );
      });
  };

  const fetchServicesOrder = async () => {
    await getServiceOrders()
      .then((resp) => {
        setServicesOrder(
          resp.map((e) => {
            return {
              value: e.key,
              label: e.key,
              description: e.value,
            };
          })
        );
      })
      .catch((err) => {
        toast.error(
          err.response.data.message ? err.response.data.message : "Error"
        );
      });
  };

  const fetchJobs = async () => {
    await getJobs()
      .then((resp) => {
        setJobs(
          resp.map((e) => {
            return {
              value: e.key,
              label: e.key,
              description: e.value,
            };
          })
        );
      })
      .catch((err) => {
        toast.error(
          err.response.data.message ? err.response.data.message : "Error"
        );
      });
  };

  const fetchJobTaks = async () => {
    await getJobTasks()
      .then((resp) => {
        setJobTasks(
          resp.map((e) => {
            return {
              value: e.key,
              label: e.key,
              description: e.value,
            };
          })
        );
      })
      .catch((err) => {
        toast.error(
          err.response.data.message ? err.response.data.message : "Error"
        );
      });
  };

  const fetchLabors = async () => {
    await getLaborCode()
      .then((resp) => {
        setLabors(
          resp.map((e) => {
            return {
              value: e,
              label: e,
            };
          })
        );
      })
      .catch((err) => {
        toast.error(
          err.response.data.message ? err.response.data.message : "Error"
        );
      });
  };

  const fetchCustomerNumbers = async () => {
    await getCustomerNumbers()
      .then((resp) => {
        setCustomersNumber(
          resp.map((e) => {
            return {
              value: e,
              label: e,
            };
          })
        );
      })
      .catch((err) => {
        toast.error(
          err.response.data.message ? err.response.data.message : "Error"
        );
      });
  };

  const fetchProfitCenters = async () => {
    await getProfitCenters()
      .then((resp) => {
        setProfitCenters(
          resp.map((e) => {
            return {
              value: e,
              label: e,
            };
          })
        );
      })
      .catch((err) => {
        toast.error(
          err.response.data.message ? err.response.data.message : "Error"
        );
      });
  };

  const fetchStatus = async () => {
    await getTimesheetEntryStatuses()
      .then((resp) => {
        setStatus(
          resp.map((e) => {
            return {
              value: e.type,
              label: e.name,
            };
          })
        );
      })
      .catch((err) => {
        toast.error(
          err.response.data.message ? err.response.data.message : "Error"
        );
      });
  };

  useEffect(() => {
    fetchEmployee();
    fetchPayrollPeriods();
    fetchServicesOrder();
    fetchJobs();
    fetchJobTaks();
    fetchLabors();
    fetchCustomerNumbers();
    fetchProfitCenters();
    fetchStatus();
  }, [fetchEmployee]);

  const saveEntry = async () => {
    await addTimesheetEntry(data)
      .then((res) => {
        toast.success("Successful add", {
          position: "top-right",
          autoClose: 5000,
          hideProgressBar: false,
          closeOnClick: true,
          pauseOnHover: true,
          draggable: true,
        });
        window.location.replace(`/timesheets/${match.params.id}`);
      })
      .catch((err) => {
        toast.error(
          err.response.data.message ? err.response.data.message : "Error add",
          {
            position: "top-right",
            autoClose: 5000,
            hideProgressBar: false,
            closeOnClick: true,
            pauseOnHover: true,
            draggable: true,
          }
        );
      });
  };

  return loading ? (
    <div
      style={{
        display: "flex",
        alignItems: "center",
        alignContent: "center",
        justifyContent: "center",
        height: "100%",
      }}
    >
      <SpinnerComponent />
    </div>
  ) : (
    <>
      <div className="card">
        <div className="card-body">
          <div>
            <Row>
              <Col lg={10}>
                <h4>Edit Timesheet Entry</h4>
              </Col>
            </Row>
          </div>
          <div className="row mb-3 mt-5 align-items-center">
            <Row>
              <div
                className="col-sm-4 align-content-md-between"
                style={{ display: "flex", alignItems: "center" }}
              >
                <label>Work Date</label>
              </div>
              <div className="col-sm-3 mt-sm-0">
                <DatePicker
                  selected={data.workDate}
                  className="form-control"
                  value={data.workDate}
                  dateFormat="MM/dd/yyyy"
                  onChange={(e) =>
                    setData({
                      ...data,
                      workDate: e,
                    })
                  }
                />
              </div>
            </Row>
            <Row className="mt-4">
              <div
                className="col-sm-4 align-content-md-between"
                style={{ display: "flex", alignItems: "center" }}
              >
                <label>Customer</label>
              </div>
              <div className="col-sm-3 mt-sm-0">
                <Select
                  className="basic-single"
                  classNamePrefix="select"
                  name="classes"
                  options={customersNumber}
                  value={
                    customersNumber.filter(
                      (p) => p.value === data.customerNumber
                    )[0]
                  }
                  onChange={(e) =>
                    setData({ ...data, customerNumber: e.value })
                  }
                />
              </div>
            </Row>
            {/* <Row className="mt-4">
              <div
                className="col-sm-4 align-content-md-between"
                style={{ display: "flex", alignItems: "center" }}
              >
                <label>Payroll Period</label>
              </div>
              <div className="col-sm-3 mt-sm-0">
                <Select
                  className="basic-single"
                  classNamePrefix="select"
                  name="classes"
                  options={periods}
                  value={periods.filter((p) => p.value === data.payrollCode)[0]}
                  onChange={(e) => setData({ ...data, payrollCode: e.value })}
                />
              </div>
            </Row> */}
            <Row className="mt-4">
              <div
                className="col-sm-4 align-content-md-between"
                style={{ display: "flex", alignItems: "center" }}
              >
                <label>Quantity</label>
              </div>
              <div className="col-sm-3 mt-sm-0">
                <input
                  type="text"
                  className="form-control"
                  placeholder="Quantity"
                  value={data.quantity}
                  onChange={(e) =>
                    setData({
                      ...data,
                      quantity: e.target.value,
                    })
                  }
                />
              </div>
            </Row>
            <Row className="mt-4">
              <div
                className="col-sm-4 align-content-md-between"
                style={{ display: "flex", alignItems: "center" }}
              >
                <label>Service Order</label>
              </div>
              <div className="col-sm-3 mt-sm-0">
                <CreatableSelect
                  isClearable
                  options={servicesOrder}
                  onChange={(e) => {
                    setData({
                      ...data,
                      serviceOrderNumber: e ? e.value : "",
                      serviceOrderDescription: e ? e.description : "",
                    });
                  }}
                />
              </div>
            </Row>
            <Row className="mt-4">
              <div
                className="col-sm-4 align-content-md-between"
                style={{ display: "flex", alignItems: "center" }}
              >
                <label>Service Order Description</label>
              </div>
              <div className="col-sm-3 mt-sm-0">
                <input
                  type="text"
                  className="form-control"
                  placeholder="Service Order Description"
                  value={data.serviceOrderDescription}
                  onChange={(e) =>
                    setData({
                      ...data,
                      serviceOrderDescription: e.target.value,
                    })
                  }
                />
              </div>
            </Row>
            <Row className="mt-4">
              <div
                className="col-sm-4 align-content-md-between"
                style={{ display: "flex", alignItems: "center" }}
              >
                <label>Job</label>
              </div>
              <div className="col-sm-3 mt-sm-0">
                <CreatableSelect
                  isClearable
                  options={jobs}
                  onChange={(e) => {
                    setData({
                      ...data,
                      jobNumber: e ? e.value : "",
                      jobDescription: e ? e.description : "",
                    });
                  }}
                />
              </div>
            </Row>
            <Row className="mt-4">
              <div
                className="col-sm-4 align-content-md-between"
                style={{ display: "flex", alignItems: "center" }}
              >
                <label>Job Description</label>
              </div>
              <div className="col-sm-3 mt-sm-0">
                <input
                  type="text"
                  className="form-control"
                  placeholder="Job Description"
                  value={data.jobDescription}
                  onChange={(e) =>
                    setData({
                      ...data,
                      jobDescription: e.target.value,
                    })
                  }
                />
              </div>
            </Row>
            <Row className="mt-4">
              <div
                className="col-sm-4 align-content-md-between"
                style={{ display: "flex", alignItems: "center" }}
              >
                <label>Job Task</label>
              </div>
              <div className="col-sm-3 mt-sm-0">
                <CreatableSelect
                  isClearable
                  options={jobTasks}
                  onChange={(e) => {
                    setData({
                      ...data,
                      jobTaskNumber: e ? e.value : "",
                      jobTaskDescription: e ? e.description : "",
                    });
                  }}
                />
              </div>
            </Row>
            <Row className="mt-4">
              <div
                className="col-sm-4 align-content-md-between"
                style={{ display: "flex", alignItems: "center" }}
              >
                <label>Job Task Description</label>
              </div>
              <div className="col-sm-3 mt-sm-0">
                <input
                  type="text"
                  className="form-control"
                  placeholder="Job Task Description"
                  value={data.jobTaskDescription}
                  onChange={(e) =>
                    setData({
                      ...data,
                      jobTaskDescription: e.target.value,
                    })
                  }
                />
              </div>
            </Row>
            <Row className="mt-4">
              <div
                className="col-sm-4 align-content-md-between"
                style={{ display: "flex", alignItems: "center" }}
              >
                <label>Labor</label>
              </div>
              <div className="col-sm-3 mt-sm-0">
                <Select
                  className="basic-single"
                  classNamePrefix="select"
                  name="classes"
                  options={labors}
                  value={labors.filter((p) => p.value === data.laborCode)[0]}
                  onChange={(e) => setData({ ...data, laborCode: e.value })}
                />
              </div>
            </Row>
            <Row className="mt-4">
              <div
                className="col-sm-4 align-content-md-between"
                style={{ display: "flex", alignItems: "center" }}
              >
                <label>Profit Center</label>
              </div>
              <div className="col-sm-3 mt-sm-0">
                <Select
                  className="basic-single"
                  classNamePrefix="select"
                  name="classes"
                  options={profitCenters}
                  value={
                    profitCenters.filter(
                      (p) => p.value === data.profitCenterNumber
                    )[0]
                  }
                  onChange={(e) =>
                    setData({ ...data, profitCenterNumber: e.value })
                  }
                />
              </div>
            </Row>
            <Row className="mt-4">
              <div
                className="col-sm-4 align-content-md-between"
                style={{ display: "flex", alignItems: "center" }}
              >
                <label>Status</label>
              </div>
              <div className="col-sm-3 mt-sm-0">
                <Select
                  className="basic-single"
                  classNamePrefix="select"
                  name="classes"
                  options={status}
                  value={status.filter((p) => p.value === data.status)[0]}
                  onChange={(e) => setData({ ...data, status: e.value })}
                />
              </div>
            </Row>
            <Row>
              <Col lg={10} />
              <Col>
                <div className="d-sx-inline-block">
                  <Button onClick={() => saveEntry()}>Save</Button>
                </div>
              </Col>
            </Row>
          </div>
        </div>
      </div>
    </>
  );
};

export default NewEntry;
