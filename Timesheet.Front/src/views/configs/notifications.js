import React, { useContext, useEffect, useState } from "react";
import { Table } from "react-bootstrap";
import { toast } from "react-toastify";
import SpinnerComponent from "../../components/spinner/spinner";
import { ThemeContext } from "../../context/themeContext";
import { getNotificationsSettings, updateNotificationsSetting } from "../../redux/actions/settings";
import { sendTestNotification } from "../../redux/actions/notifications";
import { displayError, displaySuccess } from "../../services/toast";
import { useSelector } from "react-redux";

const Notifications = () => {
  const { setTitle } = useContext(ThemeContext);
  const loading = useSelector((state) => state.auth.showLoading);

  useEffect(() => {
    setTitle("Notifications");
  }, [setTitle]);

  const [settings, setSettings] = useState([]);

  const fetchSettings = async () => {
    await getNotificationsSettings()
      .then((resp) => setSettings(resp))
      .catch((err) => displayError(err, "Error while fetching Settings" ));
  };

  useEffect(() => {
    fetchSettings();
  }, []);

  const triggerTestNotification = async () => {
    await sendTestNotification()
      .then((sent) => {
        if(sent === false){
          displayError(sent, "Sending Test email failed");
        }else{
          displaySuccess("Test email Sent succesfully");
        }
      })
      .catch((err) => displayError("Sending Test email failed"));
  }
  

  const getCheckbox = (populations, name, id) => {
    var population = populations.find((p) => p.name === name);
    return (
      <input
        type="checkbox"
        className="form-check-input"
        id="sheetCheckApprovedSecondary"
        checked={population.isActive}
        onChange={(e) => {
          var pops = populations
            .filter((p) => p !== population && p.isActive)
            .map((p) => p.value);
          if (e.target.checked) {
            pops.push(population.value);
          }
          updateNotificationsSetting({
            id,
            populations: [...pops],
          })
          .then((res) => {
            toast.success("Update successfull", {
              position: "top-right",
              autoClose: 5000,
              hideProgressBar: false,
              closeOnClick: true,
              pauseOnHover: true,
              draggable: true,
              progress: undefined,
            });
            fetchSettings();
          })
          .catch((err) => {
            toast.error(
              err.response.data.message ? err.response.data.message : "Error",
              {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
              }
            );
          });
        }}
      />
    );
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
        <button className="btn btn-danger" type="button" onClick={() => triggerTestNotification()}>Send Test notification</button>
        </div>
      </div>          
      <div className="card">
        <div className="card-body">
          <div className="w-100 table-responsive">
            <div style={{ display: "flex" }}>
              <div style={{ flex: 1 }}>
                <h4>Timesheet Notification</h4>
              </div>
            </div>
            <Table responsive>
              <thead>
                <tr>
                  <th>Status</th>
                  <th>Employee</th>
                  <th>Primary Approver</th>
                  <th>Secondary Approver</th>
                  <th>Administrator</th>
                </tr>
              </thead>
              <tbody>
                {settings
                  .filter((setting) => setting.groupName === "TIMESHEET")
                  .map((setting, i) => (
                    <tr key={i}>
                      <td>{setting.action.replaceAll("_", " ")}</td>
                      {setting.populations.map((population, id) => (
                        <td key={id}>
                          <div className="form-check custom-checkbox mb-3 checkbox-info">
                            {getCheckbox(setting.populations, population.name, setting.id)}
                          </div>
                        </td>
                      ))}
                    </tr>
                  ))}
              </tbody>
            </Table>
          </div>
        </div>
      </div>
      <div className="card">
        <div className="card-body">
          <div className="w-100 table-responsive">
            <h4>Timeoff Notification</h4>
            <Table responsive>
              <thead>
                <tr>
                  <th>Status</th>
                  <th>Employee</th>
                  <th>Primary Approver</th>
                  <th>Secondary Approver</th>
                  <th>Administrator</th>
                </tr>
              </thead>
              <tbody>
                {settings
                  .filter((setting) => setting.groupName === "TIMEOFF")
                  .map((setting, i) => (
                    <tr key={i}>
                      <td>{setting.action.replaceAll("_", " ")}</td>
                      {setting.populations.map((population, id) => (
                        <td key={id}>
                          <div className="form-check custom-checkbox mb-3 checkbox-info">
                            {getCheckbox(setting.populations, population.name, setting.id)}
                          </div>
                        </td>
                      ))}
                    </tr>
                  ))}
              </tbody>
            </Table>
          </div>
        </div>
      </div>
    </>
  );
};

export default Notifications;
