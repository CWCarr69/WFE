using Timesheet.Domain.Exceptions;

namespace Timesheet.Domain.Models.Notifications
{
    public class Notification : Entity
    {
        private Notification(string id, int population, NotificationType group, string action) : base(id)
        {
            Population = population;
            Group = group;
            Action = action;
        }

        public int Population { get; private set; }
        public NotificationType Group { get; private set; }
        public string Action { get; private set; }

        public static Notification Create(int population, NotificationType group, string action)
        {
            if(population < 0 && population % 2 != 0)
            {
                throw new InvalidNotificationPopulationException(population);
            }
            return new Notification(GenerateId(), population, group, action);
        }

        public void UpdatePopulation(int population)
        {
            if (population < 0 && population % 2 != 0)
            {
                throw new InvalidNotificationPopulationException(population);
            }
            this.Population = population;
        }
    }
}
