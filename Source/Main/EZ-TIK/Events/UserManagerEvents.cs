using EZ_TIK.ViewModels;
using Prism.Events;

namespace EZ_TIK.Events
{
    public class AddUserManagerUserEvent : PubSubEvent<AddUserManagerUserViewModel> { }

    public class AddMultipleUserManagerUsersEvent : PubSubEvent<AddMultipleUserManagerUsersViewModel> { }
}
