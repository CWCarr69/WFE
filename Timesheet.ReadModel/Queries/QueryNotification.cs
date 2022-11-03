using Timesheet.Application.Notifications.Queries;
using Timesheet.Domain.ReadModels.Notifications;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.Infrastructure.ReadModel.Queries
{
    public class QueryNotification : IQueryNotification
    {
        private readonly IDatabaseService _dbService;

        public QueryNotification(IDatabaseService dbService)
        {
            this._dbService = dbService;
        }

        public async Task<IEnumerable<NotificationDetails>> GetNotifications()
        {
            var query = $"Select * from notifications order by [group]";
            var notifications = await _dbService.QueryAsync<NotificationDetails>(query);

            return notifications;
        }
    }
}