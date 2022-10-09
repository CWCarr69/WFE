using Timesheet.EmailSender.Models;

namespace Timesheet.EmailSender.Services
{
    internal class TemplateProcessor : ITemplateProcessor
    {
        private readonly string _defaultMailTemplate;

        private List<string> _properties;
        public List<string> Properties
        {
            get
            {
                if(_properties is null)
                {
                    _properties = typeof(NotificationItem)
                        .GetType().GetProperties().Select(p => p.Name)
                        .ToList();
                }
                return _properties;
            }
        }

        public TemplateProcessor(string templatePath)
        {
            _defaultMailTemplate = File.ReadAllText(templatePath);
        }

        public string ProcessNotification(NotificationItem item)
        {
            var message = _defaultMailTemplate;

            Properties.ForEach(p =>
            {
                message = message.Replace($"@{p}", item[p]);
            });

            return message;
        }
    }
}
