import React, { useCallback, useContext, useEffect, useState } from "react";
import { ThemeContext } from "../../context/themeContext";
import SpinnerComponent from "../../components/spinner/spinner";
import { useSelector } from "react-redux";
import { displayError } from "../../services/toast";
import { getReports } from "../../redux/actions/reports";

const Reports = () => {
  const { setTitle } = useContext(ThemeContext);
  const loading = useSelector((state) => state.auth.showLoading);

  useEffect(() => {
    setTitle("Reports");
  }, [setTitle]);

  const [reports, setReports] = useState([]);

  const fetchReports = useCallback(async () => {
    await getReports(t)
      .then((resp) => setReports(resp.items))
      .catch((err) => displayError(err, "Error while retrieving fetching data"))
  }, []);

  useEffect(() => fetchReports(), [fetchReports]);

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
    <div className="report-reports">
      
    </div>
  );
};

export default Reports;