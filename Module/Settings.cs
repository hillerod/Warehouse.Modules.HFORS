using Bygdrift.Warehouse.Attributes;

namespace Module
{
    public class Settings
    {
        [ConfigSecret(NotSet = NotSet.ShowLogInfo, ErrorMessage = "Cutomerdata will not be loaded - FTP connectionstring missing.")]
        public string FTPConnectionCustomerData { get; set; }

        [ConfigSecret(NotSet = NotSet.ThrowError)]
        public string FTPConnectionMeterReadings { get; set; }

        [ConfigSecret(NotSet = NotSet.ThrowError)]
        public string FTPConnectionDeliverToME2 { get; set; }

        [ConfigSetting(NotSet = NotSet.ShowLogInfo, Default = 6, ErrorMessage = "MonthsToKeepMeteringsPerHour is not set, so as standard, it is set to 6 months.")]
        public int MonthsToKeepMeteringsPerHour { get; set; }
    }
}
