using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace ChatBot.State
{
    /// <summary>
    /// Handles multi turn userstate and profile.
    /// </summary>
    public class MultiTurnPromptsBotAccessors
    {
        // Initializes a new instance of the class.
        public MultiTurnPromptsBotAccessors(ConversationState conversationState, UserState userState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
        }

        /// <summary>
        /// Gets or sets the state of the conversation dialog.
        /// </summary>
        /// <value>The state of the conversation dialog.</value>
        public IStatePropertyAccessor<DialogState> ConversationDialogState { get; set; }

        /// <summary>
        /// Gets or sets the user profile.
        /// </summary>
        /// <value>The user profile.</value>
        public IStatePropertyAccessor<UserProfile> UserProfile { get; set; }

        public ConversationState ConversationState { get; }

        public UserState UserState { get; }
    }
}