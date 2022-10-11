namespace Timesheet.FDPDataIntegrator
{
    internal class FDPSettings
    {
        public string FDP_Username { get; private set; } = "FPIntegration@wilsonfire.com";
        public string FDP_Password { get; private set; } = "0iRP0qilgyYM7SCipFmp";

        public static FDPSettings CreateFromConfigurationList(IEnumerable<(string Name, string Value)> configurations)
        {
            var fdpSettings = new FDPSettings();
            foreach (var setting in configurations)
            {
                if (nameof(FDP_Username) == setting.Name)
                {
                    fdpSettings.FDP_Username = setting.Value;
                }

                if (nameof(FDP_Password) == setting.Name)
                {
                    fdpSettings.FDP_Password = setting.Value;
                }
            }

            if(string.IsNullOrEmpty(fdpSettings.FDP_Username)
                || string.IsNullOrEmpty(fdpSettings.FDP_Password))
            {
                throw new Exception("FDP Configurations are not available");
            }

            return fdpSettings;
        }
    }
}
