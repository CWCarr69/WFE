import React, { useContext, useEffect, useState } from "react";
import { ThemeContext } from "../../context/themeContext";
import iconPic from "../../assets/images/browser/icon1.png";
import { getSettings, updateSetting } from "../../redux/actions/settings";
import { toast } from "react-toastify";
import SpinnerComponent from "../../components/spinner/spinner";
import { useSelector } from "react-redux";

const Settings = () => {
  const { setTitle } = useContext(ThemeContext);
  const loading = useSelector((state) => state.auth.showLoading);

  const [settings, setSettings] = useState([]);

  const [toSave, setToSave] = useState([]);

  const fetchSettings = async () => {
    await getSettings()
      .then((resp) => {
        setSettings(resp);
        let array = [];
        resp.forEach((s) => {
          s.settings.forEach((st) => array.push(st));
        });
        setToSave(array);
      })
      .catch((err) => {
        toast.error( err.response.data.message ? err.response.data.message : "Error while retrieving Settings" );
      });
  };

  useEffect(() => {
    setTitle("Settings");
  }, [setTitle]);

  useEffect(() => {
    fetchSettings();
  }, []);

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
      {settings.map((s, i) => (
        <div className="card" key={i}>
          <div className="card-body">
            <div className="w-100 ">
              <h4 style={{ textTransform: "uppercase" }}>{s.group}</h4>
              {s.settings.map((x, id) => (
                <div className="mb-3 mt-3" key={id}>
                  <label style={{ textTransform: "capitalize" }}>
                    {x.name.replaceAll("_", " ")}
                  </label>
                  <div className="input-group mb-3">
                    <input
                      type="text"
                      className="form-control"
                      value={x.value}
                      onChange={(e) => {
                        let array = [...toSave];
                        array.forEach((s) => {
                          if (s.id === x.id) {
                            s.value = e.target.value;
                          }
                        });
                        setToSave(array);
                      }}
                    />

                    <button
                      className="btn btn-danger"
                      type="button"
                      onClick={() =>
                        updateSetting({
                          id: x.id,
                          value: x.value,
                        })
                      }
                    >
                      Save
                    </button>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
      ))}
      <div className="card">
        <div className="card-body">
          <div className="w-100 ">
            <h4>Images</h4>
          </div>
          <div className="row mb-3 mt-4">
            <div className="col-md-3 col-xxl-12">
              <label>Logo</label>
              <div className="col-lg-4 col-xl-4 col-sm-4 col-6 int-col mb-1">
                <img src={iconPic} alt="profileImage" className="img-fluid" />
              </div>
            </div>
            <div className="col-md-9 mt-2 mt-sm-0 ">
              <button className="btn btn-danger pull-right" type="button">
                Upload New Document
              </button>
            </div>
          </div>
          <div className="row mb-3 mt-5">
            <div className="col-md-3 col-xxl-12">
              <label>Favicon</label>
              <div className="col-lg-4 col-xl-4 col-sm-4 col-6 int-col mb-1">
                <img src={iconPic} alt="profileImage" className="img-fluid" />
              </div>
            </div>
            <div className="col-md-9 mt-2 mt-sm-0 ">
              <button className="btn btn-danger pull-right" type="button">
                Upload New Document
              </button>
            </div>
          </div>
          <div className="row mb-3 mt-5">
            <div className="col-md-3 col-xxl-12">
              <label>Web api favicon</label>
              <div className="col-lg-4 col-xl-4 col-sm-4 col-6 int-col mb-1">
                <img src={iconPic} alt="profileImage" className="img-fluid" />
              </div>
            </div>
            <div className="col-md-9 mt-2 mt-sm-0 ">
              <button className="btn btn-danger pull-right" type="button">
                Upload New Document
              </button>
            </div>
          </div>
        </div>
      </div>
    </>
  );
};

export default Settings;
