import React, { useState, useEffect } from "react";
import Select from "react-select";
import { getDepartments, getPayrollPeriods } from "../../redux/actions/referentials";
import { getEmployee } from "../../redux/actions/employees";
import moment from "moment";
import { displayError } from "../../services/toast";
import { DefaultGlobalDateFormat } from "../../services/util";

const TimesheetFilter = ({ onPayrollTypeChanged, onPayrollPeriodChanged, onDepartmentChanged, onEmployeeChanged, totalQuantity }) => {
    const typeOptions = [
        { value: "hourly", label: "Hourly" },
        { value: "salary", label: "Salary" }
    ];
    const [types, setTypes] = useState([]);
    const [periods, setPeriods] = useState([]);
    const [departments, setDepartments] = useState([]);
    const [employees, setEmployees] = useState([]);

    const [selectedType, setSelectedType] = useState(typeOptions[0]); // Added state for selected payroll type

    const [selectedPeriod, setSelectedPeriod] = useState(null);

    const fetchTypes = async () => {
        setTypes(typeOptions);
    }
    const fetchEmployees = async () => {
        await getEmployee()
            .then((resp) => {
                let dts = [{value: "",label: ""}];
                resp.forEach((e) => dts.push({value: e.id, label: `${e.fullName} - ${e.id}`}));
                setEmployees(dts);
            })
            .catch((err) => displayError(err, "Error while fectching employees"));
        };
    
    const fetchPayrollPeriods = async (type) => {
        setPeriods(null);
        setSelectedPeriod(null);
        await getPayrollPeriods(type)
        .then((resp) => {
            let adaptedPeriods = resp.map((e) => {
                return {
                    value: e.code,
                    label: `${e.code} [${moment(e.startDate).format(DefaultGlobalDateFormat)}-${moment(e.endDate).format(DefaultGlobalDateFormat)}]`,
                    start: e.startDate?.substring(0, 10),
                    end: e.endDate?.substring(0, 10)
                };
            });
            setPeriods(adaptedPeriods);
            if (adaptedPeriods?.length > 0 && selectedPeriod === null) {
                onPayrollPeriodChanged(adaptedPeriods[0].value);
                setSelectedPeriod(adaptedPeriods[0]);
            }
        })
        .catch((err) => displayError(err, "Error while fectching Payroll periods"));
    };
    
    const fetchDepartments = async () => {
    await getDepartments()
        .then((resp) => {
        resp
        .filter((r) => r.departmentId !== null)
        .map((e) => {
            let dts = [{value: "", label: ""}];
            resp.forEach((e) => {
            dts.push({value: e.departmentId, label: e.departmentName});
            setDepartments(dts);
            });
        })
        })
        .catch((err) => displayError(err, "Error while fectching Departments periods"));
    };

    useEffect(() => {
        fetchTypes();
        fetchEmployees();
        fetchPayrollPeriods(selectedType?.value); // Pass selectedType if available
        fetchDepartments();
    }, []);

    return (
        <div>
        <div className="basic-form">
          <div className="row mt-4">
            <div className="form-group mb-1 col-sm-2">
              <label>Payroll Type</label>
              <Select
                  className="basic-single"
                  classNamePrefix="select"
                  name="payrollType"
                  options={types}
                  value={selectedType}
                  onChange={(e) => {
                      setSelectedType(e);
                      onPayrollTypeChanged && onPayrollTypeChanged(e?.value);
                      fetchPayrollPeriods(e?.value); // Fetch periods for selected type
                  }}
              />
            </div>
            <div className="form-group mb-1 col-md-3">
              <label>Payroll Period</label>
              <Select className="basic-single" classNamePrefix="select" name="classes"
                options={periods}
                value={selectedPeriod}
                onChange={(e) => {
                    onPayrollPeriodChanged(e.value);
                    setSelectedPeriod(e);
                }}
              />
            </div>
            <div className="form-group mb-1 col-md-3">
              <label>Employee</label>
              <Select
                className="basic-single"
                classNamePrefix="select"
                name="classes"
                options={employees}
                onChange={(e) => onEmployeeChanged(e)}
              />
            </div>
            <div className="form-group mb-1 col-md-3">
              <label>Department</label>
              <Select
                className="basic-single"
                classNamePrefix="select"
                name="classes"
                options={departments}
                onChange={(e) => onDepartmentChanged(e)}
              />
            </div>
            <div className="form-group mb-1 col-md-1">
              <label>Total Quantity</label>
              <label>
                <strong>{totalQuantity}</strong>
              </label>
            </div>
          </div>
          {/* <div className="row">
            <div className="mb-1 col-md-12">
              From <strong>{moment(selectedPeriod.start).format(DefaultGlobalDateFormat)}</strong> to <strong>{moment(selectedPeriod.nd).format(DefaultGlobalDateFormat)}</strong>
            </div>
          </div> */}
        </div>
      </div>
    );
}

export default TimesheetFilter;
