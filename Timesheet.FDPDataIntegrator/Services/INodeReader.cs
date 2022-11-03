namespace Timesheet.FDPDataIntegrator.Services
{
    public interface INodeReader
    {
        T Read<T>(string response);
    }
}