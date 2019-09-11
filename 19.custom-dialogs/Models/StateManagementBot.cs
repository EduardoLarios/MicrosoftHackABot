using Microsoft.Bot.Builder;

namespace DiaBOT.Models
{
    public class StateManagementBot
    {
        private BotState _conversationState;
        private BotState _userState;

        public StateManagementBot(ConversationState conversationState, UserState userState)
        {
            _conversationState = conversationState;
            _userState = userState;
        }
    }
}