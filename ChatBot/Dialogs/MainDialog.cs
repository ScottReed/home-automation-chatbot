﻿using System;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Business.Main.Models;
using ChatBot.Constants;
using ChatBot.Properties;
using ChatBot.State;
using Core.Base;
using Core.Extensions;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.State;

namespace ChatBot.Dialogs
{
    /// <summary>
    /// Application Main Dialog.
    /// Implements the <see cref="CustomComponentDialog{UserProfile}" />
    /// </summary>
    /// <seealso cref="CustomComponentDialog{UserProfile}" />
    public class MainDialog : CustomComponentDialog<UserProfile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainDialog" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="downloadDialog">The download dialog.</param>
        public MainDialog(ILogger<MainDialog> logger, IServiceProvider serviceProvider, DownloadDialog downloadDialog) : base(DialogNames.MainDialog, logger, serviceProvider)
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
            var userProfile = await Accessors.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

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
            return await stepContext.ReplaceDialogAsync(nameof(WaterfallDialog), cancellationToken: cancellationToken);
        }
    }
}