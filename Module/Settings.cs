using Bygdrift.Warehouse.Helpers.Attributes;

namespace Module
{
    public class Settings
    {
        [ConfigSetting]
        public string DataFromSetting { get; set; }
                
        [ConfigSecret(NotSet = NotSet.ThrowError)]
        public string FTPConnectionString { get; set; }
    }
}
