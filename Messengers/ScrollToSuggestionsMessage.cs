using CommunityToolkit.Mvvm.Messaging.Messages;

namespace macaroni_dev.Messengers;

public class ScrollToSuggestionsMessage : ValueChangedMessage<bool>
{
    public ScrollToSuggestionsMessage(bool value) : base(value) { }
}