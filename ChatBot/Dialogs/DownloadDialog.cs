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
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ChatBot.Dialogs
{
    /// <summary>
    /// The download dialog.
    /// Implements the <see cref="CustomComponentDialog" />
    /// </summary>
    /// <seealso cref="CustomComponentDialog" />
    public class DownloadDialog : CustomComponentDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadDialog" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="activityHelpers">The activity helpers.</param>
        /// <param name="accessors">The accessors.</param>
        /// <param name="movieDownloadDialog">The movie download dialog.</param>
        /// <param name="tvDownloadDialog">The tv download dialog.</param>
        public DownloadDialog(IConfiguration configuration, ILogger<DownloadDialog> logger, HelperService activityHelpers, MultiTurnPromptsBotAccessors accessors, MovieDownloadDialog movieDownloadDialog, TvDownloadDialog tvDownloadDialog) 
            : base(DialogNames.DownloadDialog, accessors, configuration, activityHelpers, logger)
        {
            AddDialog(movieDownloadDialog);
            AddDialog(tvDownloadDialog);

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                AskDownloadTypeAsync,
                HandleDownloadTypeAsync,

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
            var userProfile = await Accessors.UserProfile.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);
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