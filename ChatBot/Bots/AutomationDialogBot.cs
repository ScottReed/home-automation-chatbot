// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Base;
using ChatBot.Extensions;
using ChatBot.Properties;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace ChatBot.Bots
{
    /// <summary>
    /// Automation chat bot.
    /// Implements the <see cref="ActivityHandler" />
    /// </summary>
    /// <seealso cref="ActivityHandler" />
    public class AutomationDialogBot<T> : DialogBot<T> where T : Dialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationDialogBot{T}"/> class.
        /// </summary>
        /// <param name="conversationState">State of the conversation.</param>
        /// <param name="userState">State of the user.</param>
        /// <param name="dialog">The dialog.</param>
        /// <param name="logger">The logger.</param>
        public AutomationDialogBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger) : base(conversationState, userState, dialog, logger)
        {
        }

        /// <summary>
        /// on members added as an asynchronous operation.
        /// </summary>
        /// <param name="membersAdded">A list of all the users that have been added in the conversation update.</param>
        /// <param name="turnContext">The context object for this turn.</param>
        /// <param name="cancellationToken">A cancellatio`n token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var welcomeMessage = MessageFactory.Text(Resources.AutomationChatBot_WelcomeMessage);
                    await turnContext.SendActivityAsync(welcomeMessage, cancellationToken);

                    // Run the Dialog with the new message Activity.
                    await Dialog.Run(turnContext, ConversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
                }
            }
        }
    }
}
