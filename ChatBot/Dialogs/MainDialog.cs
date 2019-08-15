using System;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Base;
using ChatBot.Business.Main.Models;
using ChatBot.Constants;
using ChatBot.Extensions;
using ChatBot.Helpers;
using ChatBot.Properties;
using ChatBot.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ChatBot.Dialogs
{
    /// <summary>
    /// Application Main Dialog.
    /// Implements the <see cref="ComponentDialog" />
    /// </summary>
    /// <seealso cref="ComponentDialog" />
    public class MainDialog : CustomComponentDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainDialog" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="activityHelpers">The activity helpers.</param>
        /// <param name="accessors">The accessors.</param>
        /// <param name="downloadDialog">The download dialog.</param>
        public MainDialog(IConfiguration configuration, ILogger<MainDialog> logger, HelperService activityHelpers, MultiTurnPromptsBotAccessors accessors, DownloadDialog downloadDialog) 
            : base(DialogNames.MainDialog, accessors, configuration, activityHelpers, logger)
        {
            AddDialog(downloadDialog);

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                RunInitialStepAsync,
                HandleInitialStepAsnc,
                FinalStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// run initial step as an asynchronous operation.
        /// </summary>
        /// <param name="stepContext">The step context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;DialogTurnResult&gt;.</returns>
        private async Task<DialogTurnResult> RunInitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(ChoicePromptDialog, ActivityHelpers.GetChoicesFromEnum<InitialOptions>(Resources.MainDialog_InitalStep_Message, null), cancellationToken);
        }

        /// <summary>
        /// Handles the result of the initial step.
        /// </summary>
        /// <param name="stepContext">The step context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;DialogTurnResult&gt;.</returns>
        private async Task<DialogTurnResult> HandleInitialStepAsnc(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Get the current profile object from user state.
            var userProfile = await Accessors.UserProfile.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

            userProfile.MainAction = stepContext.GetEnumValueFromChoice<InitialOptions>(); ;

            switch (userProfile.MainAction)
            {
                case InitialOptions.DownloadSomething:
                    return await stepContext.BeginDialogAsync(DialogNames.DownloadDialog, null, cancellationToken);
                default:
                    return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// Shows the final thank you message and ends the chat.
        /// </summary>
        /// <param name="stepContext">The step context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;DialogTurnResult&gt;.</returns>
        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(Resources.MainDialog_FinalStep_Message), cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}