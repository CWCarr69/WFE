import React, { useState, useContext, useEffect } from 'react';
import { toast } from "react-toastify";
import { Modal, Button } from "react-bootstrap";
import { ThemeContext } from "../../context/themeContext";
import { payPeriodsCreate } from '../../redux/actions/payperiods';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

const PayPeriods = () => {
  const { setTitle } = useContext(ThemeContext);
  const [isWorking, setIsWorking] = useState(false);
  const [result, setResult] = useState(null);
  const [payPeriods, setPayPeriods] = useState(null);
  const [showConfirmModal, setShowConfirmModal] = useState(false);

  useEffect(() => {
    setTitle("Pay Periods");
  }, [setTitle]);

  const handlePayPeriodClick = () => {
    if (!payPeriods) {
      toast.warning('Please select a pay period to start with');
      return;
    }
    setShowConfirmModal(true);
  };

  const handleConfirmPayPeriods = async () => {
    setShowConfirmModal(false);
    setIsWorking(true);
    setResult(null);
    
    await payPeriodsCreate(payPeriods)
      .then((resp) => {
        setResult(resp.result);
        toast.success('Pay periods created completed successfully');
      })
      .catch((err) => {
        toast.error(err?.response?.data?.message || 'Pay period creation failed. Please try again.');
      })
      .finally(() => {
        setIsWorking(false);
      });
  };

  const handleCancelPayPeriods = () => {
    setShowConfirmModal(false);
  };

  return (
    <div className="admin-payperiod">
      <div className="card">
        <div className="card-header">
          <h4 className="card-title">Pay Periods</h4>
        </div>
        <div className="card-body">
          <p className="alert alert-warning">
            <i className="fa fa-exclamation-triangle me-2"></i>
            This action will create pay periods starting from the selected date.
            Please proceed with caution.
          </p>
          
          <div className="form-group mb-3">
            <label className="form-label">
              <strong>Start creating pay periods from this date:</strong>
            </label>
            <DatePicker
              selected={payPeriods}
              onChange={(date) => setPayPeriods(date)}
              className="form-control"
              dateFormat="yyyy-MM-dd"
              minDate={new Date(new Date().setFullYear(new Date().getFullYear() - 6))}
              maxDate={new Date(new Date().setFullYear(new Date().getFullYear() + 2))}
              placeholderText="Select start date for pay periods"
              showMonthDropdown
              showYearDropdown
              dropdownMode="select"
            />
            {payPeriods && (
              <small className="form-text text-muted">
                Pay periods will be created starting from {payPeriods.toLocaleDateString()}.
              </small>
            )}
          </div>

          <button 
            className="btn btn-danger btn-sm" 
            onClick={handlePayPeriodClick}
            disabled={isWorking || !payPeriods}
          >
            {isWorking ? (
              <>
                <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                Creating...
              </>
            ) : (
              <>
                <i className="fa fa-trash me-2"></i>
                Create Pay Periods
              </>
            )}
          </button>

          {result && (
            <div className="alert alert-info mt-3">
              <strong>Creation Result:</strong>
              <div className="mt-2">
                <p className="mb-1"><strong>Total Pay Periods Created:</strong> {result.totalCreated}</p>
                <p className="mb-1"><strong>Start Date:</strong> {new Date(result.startDate).toLocaleDateString()}</p>
                {result.deletedCounts && (
                  <div className="mt-2">
                    <strong>Details:</strong>
                    <ul className="list-unstyled ms-3 mt-1">
                      {Object.entries(result.deletedCounts).map(([table, count]) => (
                        <li key={table}>
                          <span className="badge bg-secondary me-2">{count}</span>
                          {table}
                        </li>
                      ))}
                    </ul>
                  </div>
                )}
              </div>
            </div>
          )}
        </div>
      </div>

      {/* Confirmation Modal */}
      <Modal show={showConfirmModal} onHide={handleCancelPayPeriods} centered>
        <Modal.Header closeButton>
          <Modal.Title>
            <i className="fa fa-exclamation-triangle text-danger me-2"></i>
            Confirm Pay Period Creation
          </Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <div className="alert alert-warning">
            <strong>WARNING:</strong> This action will create new pay periods starting from the selected date!
          </div>
          <p>
            You are about to create pay periods starting from the following date:
          </p>
          <p className="text-center">
            <strong className="fs-5 text-danger">
              {payPeriods?.toLocaleDateString('en-US', { 
                year: 'numeric', 
                month: 'long', 
                day: 'numeric' 
              })}
            </strong>
          </p>
          <p className="text-danger mb-0">
            <strong>Are you absolutely sure you want to proceed?</strong>
          </p>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={handleCancelPayPeriods}>
            <i className="fa fa-times me-2"></i>
            Cancel
          </Button>
          <Button variant="danger" onClick={handleConfirmPayPeriods}>
            <i className="fa fa-trash me-2"></i>
            Yes, Create Pay Periods
          </Button>
        </Modal.Footer>
      </Modal>
    </div>
  );
};  

export default PayPeriods;