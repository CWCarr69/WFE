import React from "react";
import { finalizeTimesheet } from "../../redux/actions/timesheets";
import { displayError, displaySuccess } from "../../services/toast";
import { Button, Col, Pagination, Row } from "react-bootstrap";

const TimesheetFinalizeAction = ({canFinalize, currentTimesheetId, onDone}) => {
    const finalize = async () => {
        await finalizeTimesheet({ timesheetId: currentTimesheetId })
        .then((res) => {
            displaySuccess("Successful finalize");
            onDone();
        })
        .catch((err) => displayError(err, "Error while finalizing timesheet. Please retry"));
    };

    return  (     
        <div>
            <Row>
                <Col style={{ display: "flex", alignItems: "flex-end", justifyContent: "flex-end" }}>
                    {canFinalize && (
                    <div className="d-inline-block">
                        <Button variant="primary" onClick={finalize}> Finalize </Button>
                    </div>
                    )}
                </Col>
            </Row>
        </div>
    );
}

export default TimesheetFinalizeAction;