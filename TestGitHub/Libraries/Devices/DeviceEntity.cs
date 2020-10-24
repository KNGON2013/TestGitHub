using System;

namespace TestGitHub.Libraries.Devices
{
    public class DeviceEntity
    {
        public Guid ClassGuid { get; set; }

        public string DeviceID { get; set; }

        public string Caption { get; set; }

        public string Description { get; set; }

        public string ExtractDeviceID { get; set; }

        public string Vendor { get; set; }

        public string Product { get; set; }

        public string FriendlyName { get; set; }

        public string Service { get; set; }

        public string Status { get; set; }

        public string StatusInfo { get; set; }
    }
}
