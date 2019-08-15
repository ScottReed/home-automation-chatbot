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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ChatBot.Dialogs
{
    public class MovieDownloadDialog : CustomComponentDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MovieDownloadDialog"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="activityHelpers">The activity helpers.</param>
        /// <param name="accessors">The accessors.</param>
        public MovieDownloadDialog(IConfiguration configuration, ILogger<DownloadDialog> logger, HelperService activityHelpers, MultiTurnPromptsBotAccessors accessors)
            : base(DialogNames.MovieDownload, accessors, configuration, activityHelpers, logger)
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                AskWhatFilmAsync,
                HandleWhatFilmUserIsLookingForAsync,
                AskWhatQualityAsync,
                HandleWhatQualityAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// Ask what film user is looking for.
        /// </summary>
        /// <param name="stepContext">The step context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;DialogTurnResult&gt;.</returns>
        private async Task<DialogTurnResult> AskWhatFilmAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(Resources.MovieDialog_WhatMovie_Message) }, cancellationToken);
        }

        /// <summary>
        /// Handle what film user is looking for.
        /// </summary>
        /// <param name="stepContext">The step context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;DialogTurnResult&gt;.</returns>
        private async Task<DialogTurnResult> HandleWhatFilmUserIsLookingForAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await Accessors.UserProfile.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);
            userProfile.MovieDownload.Title = stepContext.Result.ToString();

            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Ask what quality the user is looking for.
        /// </summary>
        /// <param name="stepContext">The step context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;DialogTurnResult&gt;.</returns>
        private async Task<DialogTurnResult> AskWhatQualityAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(ChoicePromptDialog, ActivityHelpers.GetChoicesFromEnum<MovieQuality>(Resources.MovieDialog_WhatQuality_Message, null), cancellationToken);;
        }

        private async Task<DialogTurnResult> HandleWhatQualityAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await Accessors.UserProfile.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

            userProfile.MovieDownload.Quality = stepContext.GetEnumValueFromChoice<MovieQuality>();

            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }
    }
}