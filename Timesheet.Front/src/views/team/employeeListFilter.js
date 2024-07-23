import React, { useState } from "react";

const EmployeeListFilter = ({onChange}) => {
    const [filter, setFilter] = useState("");

    return  (
    <div className="form-head mb-4 d-flex flex-wrap align-items-center">
        <div className="input-group">
            <button className="input-group-text">
            <i className="flaticon-381-search-2 text-primary"></i>
            </button>
            <input type="text" className="form-control" placeholder="Search Employee Name..." 
                value={filter} 
                onChange={(event) => {
                    setFilter(event.target.value);
                    onChange(event.target.value);
                }}
            />
        </div>
    </div>
    )
}

export default EmployeeListFilter;