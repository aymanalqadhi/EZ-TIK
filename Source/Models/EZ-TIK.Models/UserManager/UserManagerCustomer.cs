using tik4net.Objects;

namespace EZ_TIK.Models
{
    [TikEntity("tool/user-manager/customer")]
    public class UserManagerCustomer
    {
        [TikProperty(".id", IsReadOnly = true, IsMandatory = true)]
        public string Id { get; set; }

        [TikProperty("login")]
        public string Login { get; set; }

        [TikProperty("password")]
        public string Password { get; set; }

        [TikProperty("backup-allowed")]
        public bool BackupAllowed { get; set; }

        [TikProperty("signup-allowed")]
        public bool SignupAllowed { get; set; }

        [TikProperty("permissions")]
        public string Permissions { get; set; }
    }
}
