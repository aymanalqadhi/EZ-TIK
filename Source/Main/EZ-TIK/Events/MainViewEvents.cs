using Prism.Events;

namespace EZ_TIK.Events
{
    internal class ToggleNotificationsTabEvent : PubSubEvent { }
    internal class UnseenNotificationsCountChangedEvent : PubSubEvent<int?> { }
}
