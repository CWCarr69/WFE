namespace Timesheet.Web.Api.Middleware
{
    internal class Problem
    {
        private string _message;
        private int _code;
        private string _type;

        public Problem(string message, int code, string type)
        {
            this._message = message;
            this._code = code;
            this._type = type;
        }
    }
}