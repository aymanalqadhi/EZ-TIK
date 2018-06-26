using tik4net.Objects;

namespace EZ_TIK.Models
{
    [TikEntity("tool/user-manager/profile/profile-limitation")]
    public class UserManagerProfileProfileLimitation
    {
        [TikProperty("profile")]
        public string Profile { get; set; }

        [TikProperty("limitation")]
        public string Limitation { get; set; }

        [TikProperty("from-time")]
        public string FromTime { get; set; }

        [TikProperty("till-time")]
        public string TillTime { get; set; }

        [TikProperty("weekdays")]
        public string WeekDays { get; set; }
    }
}
