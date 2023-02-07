namespace Timesheet.Infrastruture.ReadModel.Queries
{
    public  class BaseQuery
    {
        protected string AddWhereClauseForDirectReports(string approverId, bool directReports, string query)
        {
            var employeeIdParam = "@approverId";
            var where = string.Empty;
            if (directReports && approverId is not null)
            {
                where = $"WHERE primaryApproverId = {employeeIdParam} or (primaryApproverId is null AND secondaryApproverId = {employeeIdParam})";

            }
            else if (approverId is not null)
            {
                where = $"WHERE primaryApproverId = {employeeIdParam} or secondaryApproverId = {employeeIdParam}";
            }

            return $"{query} {where}";
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
