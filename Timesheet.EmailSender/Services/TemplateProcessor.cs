using Mustache;

namespace Timesheet.EmailSender.Services
{
    internal class TemplateProcessor : ITemplateProcessor
    {
        private string _currentTemplatePath;
        private HtmlFormatCompiler _compiler;

        public void SetTemplate(string templatePath)
        {
            _currentTemplatePath = templatePath;
            _compiler = new HtmlFormatCompiler();
        }

        public string ProcessNotification<T>(T item)
        {
            var format = File.ReadAllText(_currentTemplatePath);
            var generator = _compiler.Compile(format);

            var message = generator.Render(new
            {
                Model = item
            });

            return message;
        }
    }
}
