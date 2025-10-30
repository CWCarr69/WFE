import React, {Fragment, useMemo, useEffect} from "react";
import {
    useTable,
    useGlobalFilter,
    useFilters,
    usePagination,
    useGroupBy,
    useExpanded,
} from "react-table";
import { COLUMNS } from "./data";
import { approveTimesheet, rejectTimesheet } from "../../redux/actions/timesheets";
import { Pagination } from "react-bootstrap";
import { Link } from "react-router-dom";
import { displayError, displaySuccess } from "../../services/toast";

const TimesheetDatatable = ({data, onDecisionTaken}) => {
    const columns = useMemo(() => COLUMNS, []);

    const initGroupBy = useMemo(() => ["employee"], []);

    const tableInstance = useTable(
      {
        columns,
        data,
        initialState: {
          pageIndex: 0,
          groupBy: initGroupBy,
          isAllRowsExpanded: true,
        },
      },
      useFilters,
      useGlobalFilter,
      useGroupBy,
      useExpanded,
      usePagination
    );
    
    const {
      getTableProps,
      getTableBodyProps,
      headerGroups,
      prepareRow,
      state,
      page,
      gotoPage,
      pageCount,
      nextPage,
      previousPage,
      canNextPage,
      canPreviousPage,
    } = tableInstance;
    
    const { pageIndex } = state;

    useEffect(() => tableInstance.toggleAllRowsExpanded(false), [tableInstance]);

    const takeDecision = async (timesheet, action, successMessage, errorMessage) => {
        await action({
            employeeId: timesheet.id,
            timesheetId: timesheet.timesheetId,
        })
        .then((res) => {
            displaySuccess(successMessage);
            onDecisionTaken();
        })
        .catch((err) => displayError(err, errorMessage));
    }; 

    const approve = async (timesheet) => takeDecision(timesheet, approveTimesheet, "Timesheet approved", "Error while approving timesheet");
    const reject = async (timesheet) => takeDecision(timesheet, rejectTimesheet, "Timesheet rejected", "Error while rejecting timesheet");

    const decisionControl = (displayName, action, color, employeeEntries) => (
        <span
        className={`badge badge-rounded badge-${color} mx-2`}
        style={{ cursor: "pointer" }}
        onClick={() => action(employeeEntries) }
        >
            {displayName}
        </span>
    );

    const renderGroup = (rows, isExpanded, expandedProps) => {
        let original = rows[0].original;
        let isRejected = original.timesheetStatus === "REJECTED";

        return (
        <tr className="fw-bold" style={{textDecoration: isRejected ? 'line-through' : 'none' }}>
            <td className="py-2">
                <div style={{ display: "flex" }}>
                    {
                    <span {...expandedProps}>
                        {isExpanded
                        ? <i className="fas fa-chevron-circle-up" style={{ cursor: "pointer" }}></i>
                        : <i className="fas fa-chevron-circle-down" style={{ cursor: "pointer" }}></i>
                        }
                    </span>
                    }
                </div>
            </td>
            <td className="py-2" colSpan={2}>{rows.length > 0 && `${original.employee} - ${original.id}`}</td>
            <td className="py-2" colSpan={2}>Status: {rows.length > 0 && original.timesheetStatus}</td>
            <td className="py-2">Total: {original.total}</td>
            <td className="py-2">Overtime: {original.overtime}</td>
            <td className="py-2" colSpan={6}>
                {original.authorizedActions.find((a) => a.name === "APPROVE") && decisionControl("Approve", approve, "primary", original)}
                {original.authorizedActions.find((a) => a.name === "REJECT") && decisionControl("Reject", reject, "danger", original)}
            </td>
        </tr>
    )}

    const renderItems = (row) => {
        let isRejected = row.original.isRejected
        return (
        <tr {...row.getRowProps()} className={`${row.original.isOrphan && 'text-danger'}`}  style={{textDecoration: isRejected ? 'line-through' : 'none' }}>
            {row.cells.map((cell, id) => {
                return (
                    <td className="py-2" {...cell.getCellProps()} key={id}>
                        {cell.render("Cell", {original : row.original})}
                    </td>
                );
            })}
        </tr>
        )
    };

    const renderHeader = (headerGroup, i) => (
        <tr {...headerGroup.getHeaderGroupProps()} key={i}>
        {headerGroup.headers.map((column, id) => (
            <Fragment key={id}>
            {column.Header !== "Id" ? (
                <th {...column.getHeaderProps()}>
                {column.render("Header")}
                {column.canFilter ? column.render("Filter") : null}
                </th>
            ) : (
                <th style={{ width: 10 }}></th>
            )}
            </Fragment>
        ))}
        </tr>
    );

    return (
        <div className="table-responsive mt-4">
        <table
          {...getTableProps()}
          className="table dataTable display table-striped table-bordered"
        >
          <thead>{headerGroups.map((headerGroup, i) => renderHeader(headerGroup, i))}</thead>
          <tbody {...getTableBodyProps()} className="">
            {page.map((row, i) => {
              prepareRow(row);
              return <Fragment key={i}>{row.isGrouped 
                ? renderGroup(row.leafRows, row.isExpanded, row.getToggleRowExpandedProps()) 
                : renderItems(row)}</Fragment>
            })}
          </tbody>
        </table>
        <div id="example_wrapper" className="dataTables_wrapper">
          <div className="d-sm-flex text-center justify-content-between align-items-center mt-1">
            <div className="dataTables_info" />
            <Pagination
              size={"sx"}
              className={`pagination-gutter pagination- pagination-circle`}
            >
              <li className="page-item page-indicator">
                <Link className="page-link" to="#" onClick={() => previousPage()} disabled={!canPreviousPage}>
                  <i className="la la-angle-left" />
                </Link>
              </li>
              {Array.from({ length: pageCount }).map((x, i) => (
                <Pagination.Item key={i + 1} active={pageIndex === i} onClick={() => gotoPage(i)}> {i + 1}</Pagination.Item>
              ))}
              <li className="page-item page-indicator">
                <Link className="page-link" to="#" onClick={() => nextPage()} disabled={!canNextPage}>
                  <i className="la la-angle-right" />
                </Link>
              </li>
            </Pagination>
          </div>
        </div>
      </div>
    );
}

export default TimesheetDatatable;