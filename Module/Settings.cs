using Bygdrift.Warehouse.Helpers.Attributes;

namespace Module
{
    public class Settings
    {
        [ConfigSecret(NotSet = NotSet.ShowLogInfo, ErrorMessage = "Cutomerdata will not be loaded - FTP connectionstring missing.")]
        public string FTPConnectionCustomerData { get; set; }

        [ConfigSecret(NotSet = NotSet.ThrowError)]
        public string FTPConnectionMeterReadings { get; set; }
    }
}
