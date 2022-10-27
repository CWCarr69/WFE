
using Timesheet.Domain.DomainEvents;

namespace Timesheet.Domain.Models.Holidays
{
    public class Holiday : AggregateRoot
    {
        public DateTime Date { get; private set; }
        public string Description { get; private set; }
        public string Notes { get; private set; }
        public bool IsRecurrent { get; private set; }

        private Holiday(string id, DateTime date, string description, string Notes, bool isRecurrent)
            : base(id)
        {
            Date = date.Date;
            Description = description;
            this.Notes = Notes;
            IsRecurrent = isRecurrent;
        }

        public static Holiday Create(DateTime date, string description, string Notes, bool isRecurrent)
        {
            var holiday = new Holiday(GenerateId(), date, description, Notes, isRecurrent);
            holiday.RaiseDomainEvent(new HolidayAdded(holiday.Id, holiday.Date, holiday.Description));
            return holiday;
        }

        public void UpdateInformations(string description, string notes)
        {
            Description = description ?? Description;
            Notes = notes ?? Notes;

            RaiseDomainEvent(new HolidayGeneralInformationsUpdated(this.Id, Description, Notes));
        }
        
        public void SetAsRecurrent()
        {
            this.IsRecurrent = true;
        }

        public void Delete()
        {
            RaiseDomainEvent(new HolidayDeleted(Id));
        }

        public override string ToString() => $"{Date} - {Description}";

    }
}
