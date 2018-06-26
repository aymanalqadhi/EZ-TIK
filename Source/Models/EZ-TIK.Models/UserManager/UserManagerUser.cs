using tik4net.Objects;
using tik4net.Objects.Ip.Hotspot;

namespace EZ_TIK.Models
{
    [TikEntity("tool/user-manager/user")]
    public class UserManagerUser
    {
        [TikProperty(".id", IsMandatory = true, IsReadOnly = true)]
        public string Id { get; private set; }


        [TikProperty("name")]
        public string Name { get; set; }


        [TikProperty("username")]
        public string Username { get; set; }


        [TikProperty("customer", IsMandatory = true)]
        public string Customer { get; set; }


        [TikProperty("password")]
        public string Password { get; set; }

        [TikProperty("actual-profile", UnsetOnDefault = true)]
        public string ActualProfile { get; set; }


        [TikProperty("shared-users", DefaultValue = "unlimited")]
        public string SharedUsers { get; set; }


        [TikProperty("email")]
        public string Email { get; set; }


        [TikProperty("comment")]
        public string Comment { get; set; }


        [TikProperty("last-seen", DefaultValue = "never")]
        public string LastSeen { get; set; }

        [TikProperty("disabled")]
        public bool Disabled { get; set; }

        [TikProperty("uptime-used", IsReadOnly = true)]
        public string UptimeUsed { get; private  set; }


        [TikProperty("download-used", IsReadOnly = true)]
        public string DownloadUsed { get; private set; }


        [TikProperty("ip-address", DefaultValue = "0.0.0.0")]
        public string IpAddress { get; private set; }

    }
}