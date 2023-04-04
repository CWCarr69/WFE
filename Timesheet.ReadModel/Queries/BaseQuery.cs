namespace Timesheet.Infrastruture.ReadModel.Queries
{
    public enum ADD_AND { AND_BEFORE, AND_AFTER, AND_BOTH, AND_NONE }
    public  class BaseQuery
    {

        protected string AddWhereClauseForDirectReports(string approverId, bool directReports, string query, string employeeIdKey = "e.id", string whereKey="WHERE", string replace=null, ADD_AND addAnd = ADD_AND.AND_NONE)
        {
            var approverIdParam = "@approverId";
            var where = string.Empty;
            if (directReports && approverId is not null)
            {
                where = $"{whereKey} (primaryApproverId = {approverIdParam} or secondaryApproverId = {approverIdParam})";
            }

            else if (approverId is not null)
            {
                where = $"{whereKey} ({employeeIdKey} IN (SELECT employeeId FROM EmployeeHierarchy WHERE managerId = {approverIdParam}))";
            }

            if(where != string.Empty)
            {
                where = addAnd switch
                {
                    ADD_AND.AND_BOTH => $" AND {where} AND ",
                    ADD_AND.AND_BEFORE => $" AND {where} ",
                    ADD_AND.AND_AFTER => $" {where} AND ",
                    _ => where
                };
            }

            return replace != null ? query.Replace(replace, where): $"{query} {where}";
        }

        protected string Paginate(int page, int itemsPerPage, string query, string orderBy)
        {
            query = $@" {query} ORDER BY {orderBy}
                OFFSET { itemsPerPage * (page - 1)}
                ROWS FETCH NEXT { itemsPerPage} ROWS ONLY";
            return query;
        }
    }
}
