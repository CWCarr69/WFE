import React, { useContext, useEffect } from "react";
import SpinnerComponent from "../../components/spinner/spinner";

//Import
import { ThemeContext } from "../../context/themeContext";
import Timeoff from "./timeoff";
import Timesheet from "./timesheet";
import OrphanTimesheet from "./orphanTimesheet";
import { useSelector } from "react-redux";

const Home = () => {
  const { setTitle } = useContext(ThemeContext);
  const loading = useSelector((state) => state.auth.showLoading);

  useEffect(() => {
    setTitle("Dashboard");
  }, [setTitle]);

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
      <div className="row">
        <div className="col-xl-6">
          <Timeoff />
        </div>
        <div className="col-xl-6">
          <Timesheet />
        </div>
      </div>
      <div className="row">
        <div id="orphan-timesheets" className="col-12">
          <OrphanTimesheet />
        </div>
      </div>
    </>
  );
};
export default Home;
