import React, { Fragment } from "react";
import { Button } from "react-bootstrap";
import { exportPeriod, exportPeriodAfterFinalize } from "../../redux/actions/timesheets";
import { displayError, displaySuccess } from "../../services/toast";

const TimesheetExportPanel = ({showFinalizeButton, period, department, employee}) => {

    const exportRaw = async () => {
        await exportPeriod(period, department, employee)
        .then((res) => {})
        .catch((err) => displayError(err, "Error while exporting timesheet"));
    };

    const exportFinalized = async () => {
        await exportPeriodAfterFinalize(period)
        .then((_) => { displaySuccess("Successful export of finalized data"); })
        .catch((err) => displayError(err, "Error while exporting timesheet"));
    };

    return  (     
        <div className="col-xl-12 col-lg-12">
        {showFinalizeButton && (
          <Fragment>
            <Button className="me-2 btn-xxs" variant="outline-primary" onClick={exportFinalized}>
              <i className="fas fa-download me-3"></i>
              Finalized Timesheet
            </Button>
          </Fragment>
        )}

        <Button className="me-2 btn-xxs" variant="outline-primary" onClick={exportRaw}>
          <i className="fas fa-download me-3"></i>
          To Csv
        </Button>
      </div>
    );
}

export default TimesheetExportPanel;