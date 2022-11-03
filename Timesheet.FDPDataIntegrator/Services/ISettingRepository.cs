namespace Timesheet.FDPDataIntegrator.Services
{
    internal interface ISettingRepository
    {
        FDPSettings GetFDPParameters();
    }
}