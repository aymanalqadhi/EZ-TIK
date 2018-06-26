using EZ_TIK.ViewModels;
using Prism.Events;
using tik4net.Objects.Ip.Hotspot;

namespace EZ_TIK.Events
{
    public class AddHotspotUserEvent : PubSubEvent<AddHotspotUserViewModel> { }
    public class AddMultipleHotspotUsersEvent : PubSubEvent<AddMultipleHotspotUsersViewModel> { }
    public class AddHotspotUserProfileEvent : PubSubEvent<HotspotUserProfileViewModel> { }
   
}
