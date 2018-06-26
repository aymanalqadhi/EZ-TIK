using tik4net.Objects;

namespace EZ_TIK.Models
{
    [TikEntity("tool/user-manager/profile")]
    public class UserManagerProfile
    {
        [TikProperty(".id", IsMandatory = true, IsReadOnly = true)]
        public string Id { get; set; }

        [TikProperty("owner", IsMandatory = true)]
        public string Onwer { get; set; }

        [TikProperty("name", IsMandatory = true)]
        public string Name { get; set; }

        [TikProperty("price")]
        public int Price { get; set; }

        [TikProperty("validity")]
        public string Validity { get; set; }

        [TikProperty("starts-at")]
        public string StartsAt { get; set; }

        [TikProperty("name-for-users")]
        public string NameForUsers { get; set; }

        [TikProperty("comment")]
        public string Comment { get; set; }

        [TikProperty("override-shared-users")]
        public string OverrideSharedUsers { get; set; }
    }
}
