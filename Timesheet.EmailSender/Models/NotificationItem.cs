namespace Timesheet.EmailSender.Models
{
    public class NotificationItem
    {
        public string NotificationId { get; private set; }
        public string EmployeeEmail { get; private set; }
        public string RequestManagerName { get; private set; }
        public string RequestStatus { get; private set; }
        public string RequestCreated { get; private set; }
        public string RequestHours { get; private set; }
        public string RequestTotalHours { get; private set; }
        public string RequestType { get; private set; }
        public string RequestEmployeeComments { get; private set; }
        public string RequestSupervisorComments { get; private set; }
        public string RequestId { get; private set; }
        public string Subject { get; private set; }


        public bool Sent { get; private set; }

        private IDictionary<string, string> _requestData;

        private IDictionary<string, string> RequestData
        {
            get { return _requestData ?? (_requestData = ToDictionary()); }
        }


        internal void SetCompleted() => Sent = true;

        internal IDictionary<string, string> ToDictionary()
        {
            var result = new Dictionary<string, string>();

            result.Add(nameof(RequestId), RequestId);
            result.Add(nameof(RequestManagerName), RequestManagerName);
            result.Add(nameof(RequestStatus), RequestStatus);
            result.Add(nameof(RequestCreated), RequestCreated);
            result.Add(nameof(RequestHours), RequestHours);
            result.Add(nameof(RequestTotalHours), RequestTotalHours);
            result.Add(nameof(RequestType), RequestType);
            result.Add(nameof(RequestEmployeeComments), RequestEmployeeComments);
            result.Add(nameof(RequestSupervisorComments), RequestSupervisorComments);

            return result;
        }

        public string this[string dataName]
        {
            get {
                RequestData.TryGetValue(dataName, out var dataValue);
                return dataValue ?? String.Empty; 
            }
        }
    }
}