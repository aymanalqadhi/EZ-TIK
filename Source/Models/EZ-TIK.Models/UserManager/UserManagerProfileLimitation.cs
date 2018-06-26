using tik4net.Objects;

namespace EZ_TIK.Models
{
    [TikEntity("tool/user-manager/profile/limitation")]
    public class UserManagerProfileLimitation
    {
        [TikProperty("name")]
        public string Name { get; set; }

        [TikProperty("owner")]
        public string Owner { get; set; }

        [TikProperty("download-limit", DefaultValue = "0")]
        public long DownloadLimit { get; set; }

        [TikProperty("upload-limit", DefaultValue = "0")]
        public long UploadLimit { get; set; }

        [TikProperty("transfer-limit", DefaultValue = "0")]
        public long TransferLimit { get; set; }

        [TikProperty("uptime-limit")]
        public string UptimeLimit { get; set; }

        [TikProperty("group-name")]
        public string GroupName { get; set; }

        [TikProperty("ip-pool")]
        public string IpPoop { get; set; }

        [TikProperty("address-list")]
        public string AddressList { get; set; }
    }
}
