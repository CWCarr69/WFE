import moment from "moment";
import React, {
  Fragment,
  useCallback,
  useContext,
  useEffect,
  useState,
} from "react";
import { Button, Toast } from "react-bootstrap";
import { toast } from "react-toastify";
//** Import Image */
import SpinnerComponent from "../../components/spinner/spinner";
import { ThemeContext } from "../../context/themeContext";
import {
  getEmployeeApprovers,
  getEmployeeBenefits,
  getEmployeeById,
  getEmployeeCalculatedBenefits,
  updateBenefits,
} from "../../redux/actions/employees";
import { useSelector } from "react-redux";

const Profile = ({ match }) => {
  const { setTitle } = useContext(ThemeContext);
  const loading = useSelector((state) => state.auth.showLoading);
  const user = useSelector((state) => state.auth.auth);

  const [employee, setEmployee] = useState({});
  const [benefit, setBenefit] = useState({
    details: [],
  });

  const [upBenefits, setUpBenefits] = useState({});

  const [editBenefit, setEditBenefit] = useState(false);

  const [approvers, setApprovers] = useState({});

  useEffect(() => {
    setTitle("Profile");
  }, [setTitle]);

  const fetchBenefits = useCallback(async () => {
    await getEmployeeCalculatedBenefits(match.params.id)
      .then((resp) => {
        setBenefit(resp);
      })
      .catch((err) => {
        toast.error(
          err.response.data.message ? err.response.data.message : "Error"
        );
      });

    await getEmployeeBenefits(match.params.id)
      .then((resp) => {
        setUpBenefits(resp);
      })
      .catch((err) => {
        toast.error(
          err.response.data.message ? err.response.data.message : "Error"
        );
      });
  }, [match.params.id]);

  const fetchApprovers = useCallback(async () => {
    await getEmployeeApprovers(match.params.id)
      .then((resp) => {
        setApprovers(resp);
      })
      .catch((err) => {
        toast.error(
          err.response.data.message ? err.response.data.message : "Error"
        );
      });
  }, [match.params.id]);

  useEffect(() => {
    async function fetchData() {
      await getEmployeeById(match.params.id)
        .then((resp) => {
          setEmployee(resp);
        })
        .catch((err) => {
          toast.error(
            err.response.data.message ? err.response.data.message : "Error"
          );
        });
    }
    fetchData();
  }, [match.params]);

  useEffect(() => {
    fetchBenefits();
  }, [fetchBenefits]);

  useEffect(() => {
    fetchApprovers();
  }, [fetchApprovers]);

  const updateEmployeeBenefits = async () => {
    let data = {
      employeeId: match.params.id,
      vacationHours: Number(upBenefits.vacationHours),
      personalHours: Number(upBenefits.personalHours),
      rolloverHours: Number(upBenefits.rolloverHours),
      considerFixedBenefits: upBenefits.considerFixedBenefits,
    };

    await updateBenefits(data)
      .then((res) => {
        toast.success("Successful update", {
          position: "top-right",
          autoClose: 5000,
          hideProgressBar: false,
          closeOnClick: true,
          pauseOnHover: true,
          draggable: true,
        });
        fetchBenefits();
        setEditBenefit(false);
      })
      .catch((err) => {
        toast.error(
          err.response.data.message
            ? err.response.data.message
            : "Error update",
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
    <Fragment>
      <div className="row">
        <div className="col-xl-12">
          <div className="row">
            <div className="col-lg-12">
              <div className="card">
                <div className="card-body">
                  <h3>Employee Information</h3>
                  <div className="row">
                    <div className="col-lg-6">
                      <dl className="mt-4">
                        <dt>Employee Login</dt>
                        <dd>{employee.login}</dd>
                        <dt>Employee Name</dt>
                        <dd>{employee.fullName}</dd>
                        <dt>Employee Id</dt>
                        <dd>{employee.id}</dd>
                      </dl>
                    </div>
                    <div className="col-lg-6">
                      <dl className="mt-4">
                        <dt>Department</dt>
                        <dd>{employee.department}</dd>
                        <dt>Hire Date</dt>
                        <dd>
                          {moment(employee.employmentDate).format(
                            "MMMM DD, YYYY"
                          )}
                        </dd>
                      </dl>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div className="col-xl-12">
          <div className="row">
            <div className="col-lg-12">
              <div className="card">
                <div className="card-body">
                  <h3>Primary/Secondary Approver</h3>
                  <div className="row">
                    <div className="col-lg-6">
                      <dl className="mt-4">
                        <dt>Primary Approver Id</dt>
                        <dd>{approvers.primaryApproverId}</dd>
                        <dt>Secondary Approver Id</dt>
                        <dd>{approvers.secondaryApproverId}</dd>
                      </dl>
                    </div>
                    <div className="col-lg-6">
                      <dl className="mt-4">
                        <dt>Primary Approver</dt>
                        <dd>{approvers.primaryApproverFullName}</dd>
                        <dt>Secondary Approver</dt>
                        <dd>{approvers.secondaryApproverFullName}</dd>
                      </dl>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div className="col-xl-12">
          <div className="row">
            <div className="col-lg-12">
              <div className="card">
                <div className="card-body">
                  <h3>
                    Hours Information{" "}
                    {!editBenefit && user.isAdministrator && (
                      <span
                        style={{ marginLeft: 30, cursor: "pointer" }}
                        onClick={() => setEditBenefit(true)}
                      >
                        <i class="fa fa-pencil-alt"></i>
                      </span>
                    )}
                  </h3>
                  <div className="row">
                    <div className="col-lg-4">
                      <dl className="mt-4">
                        <dt>Eligible Vacation Hours</dt>
                        <dd>{benefit.eligibleVacationHours}</dd>
                        <dt>Eligible Personal Hours</dt>
                        <dd>{benefit.eligiblePersonalHours}</dd>
                        <dt>Rollover Hours</dt>
                        <dd>{benefit.rolloverHours}</dd>
                      </dl>
                    </div>
                    {editBenefit && user.isAdministrator && (
                      <div className="col-lg-6">
                        <div className="row" style={{ alignItems: "baseline" }}>
                          <div className="col-sm-1 mt-2 mt-sm-0">
                            <input
                              type="checkbox"
                              className="custom-control-input"
                              id="checkbox5"
                              defaultChecked={upBenefits.considerFixedBenefits}
                              onChange={(e) => {
                                setUpBenefits({
                                  ...upBenefits,
                                  considerFixedBenefits:
                                    !upBenefits.considerFixedBenefits,
                                });
                              }}
                            />
                          </div>
                          <div className="col-sm-4 align-content-md-between">
                            <label>Consider Manual Benefits</label>
                          </div>
                        </div>
                        <div
                          className="row mt-4"
                          style={{ alignItems: "center" }}
                        >
                          <div className="col-sm-3">
                            <label>Vacation Hours</label>
                          </div>
                          <div className="col-sm-3 mt-2 mt-sm-0">
                            <input
                              type="text"
                              className="form-control"
                              placeholder="Vacation Hours"
                              value={upBenefits.vacationHours}
                              onChange={(e) =>
                                setUpBenefits({
                                  ...upBenefits,
                                  vacationHours: e.target.value,
                                })
                              }
                            />
                          </div>
                        </div>
                        <div
                          className="row mt-4"
                          style={{ alignItems: "center" }}
                        >
                          <div className="col-sm-3 align-content-md-between">
                            <label>Personal Hours</label>
                          </div>
                          <div className="col-sm-3 mt-2 mt-sm-0">
                            <input
                              type="text"
                              className="form-control"
                              placeholder="Personal Hours"
                              value={upBenefits.personalHours}
                              onChange={(e) =>
                                setUpBenefits({
                                  ...upBenefits,
                                  personalHours: e.target.value,
                                })
                              }
                            />
                          </div>
                        </div>
                        <div
                          className="row mt-4"
                          style={{ alignItems: "center" }}
                        >
                          <div className="col-sm-3 align-content-md-between">
                            <label>Rollover Hours</label>
                          </div>
                          <div className="col-sm-3 mt-2 mt-sm-0">
                            <input
                              type="text"
                              className="form-control"
                              placeholder="Rollover Hours"
                              value={upBenefits.rolloverHours}
                              onChange={(e) =>
                                setUpBenefits({
                                  ...upBenefits,
                                  rolloverHours: e.target.value,
                                })
                              }
                            />
                          </div>
                        </div>
                        <div
                          className="row col-lg-6 mt-4"
                          style={{ justifyContent: "space-between" }}
                        >
                          <Button
                            variant="primary"
                            size="sm"
                            style={{ alignItems: "center", width: "40%" }}
                            onClick={() => setEditBenefit(false)}
                          >
                            cancel
                          </Button>
                          <Button
                            variant="danger"
                            size="sm"
                            style={{ alignItems: "center", width: "50%" }}
                            onClick={() => updateEmployeeBenefits()}
                          >
                            save
                          </Button>
                        </div>
                      </div>
                    )}
                  </div>
                  <div className="row mt-lg-5">
                    <table className="table customer-table display mb-4 fs-14 dataTable card-table no-footer">
                      <tbody>
                        <tr>
                          <td></td>
                          <td>Balance</td>
                          <td>Used</td>
                          <td>Scheduled</td>
                        </tr>
                        {benefit.details.map((d) => (
                          <tr>
                            <td>{d.type}</td>
                            <td>{d.balance}</td>
                            <td>{d.used}</td>
                            <td>{d.scheduled}</td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Fragment>
  );
};

export default Profile;
