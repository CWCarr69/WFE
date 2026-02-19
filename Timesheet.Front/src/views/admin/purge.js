import React, { useState, useContext, useEffect } from 'react';
import { toast } from "react-toastify";
import { Modal, Button } from "react-bootstrap";
import { ThemeContext } from "../../context/themeContext";
import { purgeTables } from '../../redux/actions/purge';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

const Purge = () => {
  const { setTitle } = useContext(ThemeContext);
  const [isPurging, setIsPurging] = useState(false);
  const [result, setResult] = useState(null);
  const [purgeDate, setPurgeDate] = useState(null);
  const [showConfirmModal, setShowConfirmModal] = useState(false);

  useEffect(() => {
    setTitle("Purge Data");
  }, [setTitle]);

  const handlePurgeClick = () => {
    if (!purgeDate) {
      toast.warning('Please select a purge date');
      return;
    }
    setShowConfirmModal(true);
  };

  const handleConfirmPurge = async () => {
    setShowConfirmModal(false);
    setIsPurging(true);
    setResult(null);
    
    await purgeTables(purgeDate)
      .then((resp) => {
        setResult(resp.result);
        toast.success('Purge completed successfully');
      })
      .catch((err) => {
        toast.error(err?.response?.data?.message || 'Purge failed. Please try again.');
      })
      .finally(() => {
        setIsPurging(false);
      });
  };

  const handleCancelPurge = () => {
    setShowConfirmModal(false);
  };

  return (
    <div className="admin-purge">
      <div className="card">
        <div className="card-header">
          <h4 className="card-title">Purge Data</h4>
        </div>
        <div className="card-body">
          <p className="alert alert-warning">
            <i className="fa fa-exclamation-triangle me-2"></i>
            This action will permanently delete data before the selected date from the system. 
            Please proceed with caution.
          </p>
          
          <div className="form-group mb-3">
            <label className="form-label">
              <strong>Purge data before this date:</strong>
            </label>
            <DatePicker
              selected={purgeDate}
              onChange={(date) => setPurgeDate(date)}
              className="form-control"
              dateFormat="yyyy-MM-dd"
              maxDate={new Date()}
              placeholderText="Select purge cutoff date"
              showMonthDropdown
              showYearDropdown
              dropdownMode="select"
            />
            {purgeDate && (
              <small className="form-text text-muted">
                All data before {purgeDate.toLocaleDateString()} will be permanently deleted.
              </small>
            )}
          </div>

          <button 
            className="btn btn-danger btn-sm" 
            onClick={handlePurgeClick} 
            disabled={isPurging || !purgeDate}
          >
            {isPurging ? (
              <>
                <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                Purging...
              </>
            ) : (
              <>
                <i className="fa fa-trash me-2"></i>
                Purge Data
              </>
            )}
          </button>

          {result && (
            <div className="alert alert-info mt-3">
              <strong>Purge Result:</strong>
              <div className="mt-2">
                <p className="mb-1"><strong>Total Records Deleted:</strong> {result.totalDeleted}</p>
                <p className="mb-1"><strong>Purge Date:</strong> {new Date(result.purgeDate).toLocaleDateString()}</p>
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
      <Modal show={showConfirmModal} onHide={handleCancelPurge} centered>
        <Modal.Header closeButton>
          <Modal.Title>
            <i className="fa fa-exclamation-triangle text-danger me-2"></i>
            Confirm Data Purge
          </Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <div className="alert alert-danger">
            <strong>WARNING:</strong> This action cannot be undone!
          </div>
          <p>
            You are about to permanently delete all data before:
          </p>
          <p className="text-center">
            <strong className="fs-5 text-danger">
              {purgeDate?.toLocaleDateString('en-US', { 
                year: 'numeric', 
                month: 'long', 
                day: 'numeric' 
              })}
            </strong>
          </p>
          <p className="mb-0">
            This will affect the following data:
          </p>
          <ul className="mt-2">
            <li>Timesheet Entries</li>
            <li>Timeoff Entries</li>
            <li>Timesheet Comments</li>
            <li>Timesheet Exceptions</li>
            <li>Related Timesheet Records</li>
          </ul>
          <p className="text-danger mb-0">
            <strong>Are you absolutely sure you want to proceed?</strong>
          </p>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={handleCancelPurge}>
            <i className="fa fa-times me-2"></i>
            Cancel
          </Button>
          <Button variant="danger" onClick={handleConfirmPurge}>
            <i className="fa fa-trash me-2"></i>
            Yes, Purge Data
          </Button>
        </Modal.Footer>
      </Modal>
    </div>
  );
};  

export default Purge;