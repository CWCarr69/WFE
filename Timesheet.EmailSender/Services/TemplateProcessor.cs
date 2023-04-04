using Mustache;

namespace Timesheet.EmailSender.Services
{
    internal class TemplateProcessor : ITemplateProcessor
    {
        private string _webAppUri;
        private readonly string _templatesBasePath;

        private string _currentTemplatePath;
        private HtmlFormatCompiler _compiler;

        public TemplateProcessor(string webAppUri, string templatesBasePath)
        {
            if(webAppUri is null || templatesBasePath is null)
            {
                throw new Exception("Cannot instanciate TemplateProcessor. Missing webAppUri or templatesBasePath configurations");
            }
            _webAppUri = webAppUri;
            _templatesBasePath = templatesBasePath;
        }

        public void SetTemplate(string templatePath)
        {
            _currentTemplatePath = Path.Combine(_templatesBasePath, templatePath);
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

        public string GetWebAppUri() => _webAppUri;
        public string GetTemplatesBasePath() => _templatesBasePath;
    }
}
