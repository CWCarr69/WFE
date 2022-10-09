using Timesheet.EmailSender.Models;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.EmailSender.Repositories
{
    internal class NotificationRepository : INotificationRepository
    {
        private readonly IDatabaseService _dbServices;

        public NotificationRepository(IDatabaseService dbServices)
        {
            _dbServices = dbServices;
        }

        IEnumerable<NotificationItem> INotificationRepository.Get()
        {
            string query = "";
            return _dbServices.Query<NotificationItem>(query);
        }

        void INotificationRepository.CompleteTransaction()
        {
            _dbServices.Execute("");
        }
    }
}