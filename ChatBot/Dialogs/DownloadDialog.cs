using System;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Business.Main.Models;
using ChatBot.Constants;
using ChatBot.Properties;
using ChatBot.State;
using Core.Base;
using Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.State;

namespace ChatBot.Dialogs
{

    /// <summary>
    /// Class DownloadDialog.
    /// Implements the <see cref="CustomComponentDialog{UserProfile}" />
    /// </summary>
    /// <seealso cref="CustomComponentDialog{UserProfile}" />
    public class DownloadDialog : CustomComponentDialog<UserProfile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadDialog" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="movieDownloadDialog">The movie download dialog.</param>
        /// <param name="tvDownloadDialog">The tv download dialog.</param>
        public DownloadDialog(ILogger<DownloadDialog> logger, IServiceProvider serviceProvider, MovieDownloadDialog movieDownloadDialog, TvDownloadDialog tvDownloadDialog) : base(DialogNames.DownloadDialog, logger, serviceProvider)
        {
            AddDialog(movieDownloadDialog);
            AddDialog(tvDownloadDialog);

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                AskDownloadTypeAsync,
                HandleDownloadTypeAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// Asks the type of the download.
        /// </summary>
        /// <param name="stepContext">The step context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;DialogTurnResult&gt;.</returns>
        private async Task<DialogTurnResult> AskDownloadTypeAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(ChoicePromptDialog, ActivityHelpers.GetChoicesFromEnum<DownloadType>(Resources.DownloadDialog_DownloadType_Message, null), cancellationToken);
        }

        /// <summary>
        /// Handles the type of the download.
        /// </summary>
        /// <param name="stepContext">The step context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;DialogTurnResult&gt;.</returns>
        private async Task<DialogTurnResult> HandleDownloadTypeAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Get the current profile object from user state.
            var userProfile = await Accessors.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);
            userProfile.DownloadType = stepContext.GetEnumValueFromChoice<DownloadType>();

            switch (userProfile.DownloadType)
            {
                case DownloadType.Tv:
                    return await stepContext.BeginDialogAsync(DialogNames.TvDownload, null, cancellationToken);
                case DownloadType.Movie:
                    return await stepContext.BeginDialogAsync(DialogNames.MovieDownload, null, cancellationToken);
                default:
                    return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }
    }
}