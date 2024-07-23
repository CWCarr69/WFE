import { ColumnFilter } from "../../components/table/filteringTable/columnFilter";
import { Link } from "react-router-dom";

export const COLUMNS = [
  {
    Header: "First Name",
    accessor: "employee",
    Filter: ColumnFilter,
    Cell: ({ original }) => (
      <Link to={original.urlToEmployee} className="brand-logo">
        {original.employee}
      </Link>
    )
  },
  {
    Header: "Department",
    accessor: "department",
    Filter: ColumnFilter,
  },
  {
    Header: "Work Date",
    accessor: "work",
    Filter: ColumnFilter,
  },
  {
    Header: "Payroll Code",
    accessor: "payrollCod",
    Filter: ColumnFilter,
    Cell: ({ original }) => (
      <Link to={original.urlToTimesheet} className="brand-logo">
        {original.payrollCod}
      </Link>
    )
  },
  {
    Header: "Quantity",
    accessor: "quantity",
    Filter: ColumnFilter,
  },
  {
    Header: "Description",
    accessor: "description",
    Filter: ColumnFilter,
  },
  {
    Header: "Service Order No",
    accessor: "service",
    Filter: ColumnFilter,
  },
  {
    Header: "Job No",
    accessor: "job",
    Filter: ColumnFilter,
  },
  {
    Header: "Job Task No",
    accessor: "task",
    Filter: ColumnFilter,
  },
  {
    Header: "Profit Center",
    accessor: "center",
    Filter: ColumnFilter,
  },
  {
    Header: "Delete",
    accessor: "delete",
    Filter: ColumnFilter,
    Cell: ({ original }) => (
      original.delete && <a><i style={{ cursor: "pointer" }} className="flaticon-381-multiply-1 text-danger" onClick={original.deleteAction}></i></a>
    )
  },
];