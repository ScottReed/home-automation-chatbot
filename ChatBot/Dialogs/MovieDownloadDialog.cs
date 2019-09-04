using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Business.Main.Models;
using ChatBot.Constants;
using ChatBot.Properties;
using ChatBot.State;
using Core.Base;
using Core.Extensions;
using Core.Helpers;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.State;
using NzbLibrary;
using TMDbLib.Client;

namespace ChatBot.Dialogs
{
    /// <summary>
    /// Class MovieDownloadDialog.
    /// Implements the <see cref="CustomComponentDialog{UserProfile}" />
    /// </summary>
    /// <seealso cref="CustomComponentDialog{UserProfile}" />
    public class MovieDownloadDialog : CustomComponentDialog<UserProfile>
    {
        private readonly NzbClient _nzbClient;
        private const string QualityWaterFall = "QualityWaterFall";
        private const string TitleWaterFall = "TitleWaterfall";
        private const string SearchWaterfall = "SearchWaterfall";

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieDownloadDialog" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="nzbClient">The client.</param>
        public MovieDownloadDialog(ILogger<DownloadDialog> logger, IServiceProvider serviceProvider, NzbClient nzbClient) : base(DialogNames.MovieDownload, logger, serviceProvider)
        {
            _nzbClient = nzbClient;
            AddDialog(new WaterfallDialog(QualityWaterFall, new WaterfallStep[]
            {
                AskWhatQualityAsync,
                HandleWhatQualityAsync
            }));

            AddDialog(new WaterfallDialog(TitleWaterFall, new WaterfallStep[]
            {
                AskWhatFilmAsync,
                HandleWhatFilmUserIsLookingForAsync
            }));

            AddDialog(new WaterfallDialog(SearchWaterfall, new WaterfallStep[]
            {
                HandleMovieSearch,
                GetNzbs,
                FtpNzb
            }));

            // The initial child Dialog to run.
            InitialDialogId = TitleWaterFall;
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
            var userProfile = await Accessors.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);
            userProfile.MovieDownload.Title = stepContext.Result.ToString();

            if (userProfile.MovieDownload.QualitySet)
            {
                return await stepContext.ReplaceDialogAsync(SearchWaterfall, cancellationToken: cancellationToken);
            }
            else
            {
                return await stepContext.ReplaceDialogAsync(QualityWaterFall, cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// Ask what quality the user is looking for.
        /// </summary>
        /// <param name="stepContext">The step context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;DialogTurnResult&gt;.</returns>
        private async Task<DialogTurnResult> AskWhatQualityAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(ChoicePromptDialog, ActivityHelpers.GetChoicesFromEnum<MovieQuality>(Resources.MovieDialog_WhatQuality_Message, null), cancellationToken);
        }

        /// <summary>
        /// handle what quality as an asynchronous operation.
        /// </summary>
        /// <param name="stepContext">The step context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;DialogTurnResult&gt;.</returns>
        private async Task<DialogTurnResult> HandleWhatQualityAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await Accessors.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

            userProfile.MovieDownload.Quality = stepContext.GetEnumValueFromChoice<MovieQuality>();
            userProfile.MovieDownload.QualitySet = true;

            return await stepContext.ReplaceDialogAsync(SearchWaterfall, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Handles movie searches.
        /// </summary>
        /// <param name="stepContext">The step context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;DialogTurnResult&gt;.</returns>
        private async Task<DialogTurnResult> HandleMovieSearch(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await Accessors.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

            var client = new TMDbClient(Settings.ApiSettings.MovieDatabaseApiKey);
            var result = await client.SearchMovieAsync(userProfile.MovieDownload.Title, cancellationToken: cancellationToken);

            if (result.TotalResults == 0)
            {
                var noResultsActivity = MessageFactory.Text(Resources.MovieDialog_ImdbSearch_NoResults);

                await stepContext.Context.SendActivityAsync(noResultsActivity, cancellationToken);
                return await stepContext.ReplaceDialogAsync(TitleWaterFall, cancellationToken: cancellationToken);
            }
            else
            {
                var options = result.Results.Select(resultItem => 
                    new AttachmentOption
                    {
                        ImageUrl = ActivityHelpers.FormatTmdbPosterPath(resultItem.PosterPath),
                        Value = resultItem.Id.ToString(),
                        Title = resultItem.Title
                    }).ToList();

                userProfile.MovieDownload.SearchOptions = options;

                var activity = ActivityHelpers.GetCardChoicesFromOptions(Resources.MovieDialog_SelectMovie_Message, options, AttachmentLayoutTypes.Carousel, true);
                return await stepContext.PromptAsync(TextPromptDialog, new PromptOptions
                {
                    Prompt = activity
                }, cancellationToken);
            }                
        }

        /// <summary>
        /// Gets the Nnb files.
        /// </summary>
        /// <param name="stepContext">The step context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;DialogTurnResult&gt;.</returns>
        private async Task<DialogTurnResult> GetNzbs(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await Accessors.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);
            var stringResult = stepContext.Result.ToString();
            var tvdbId = userProfile.MovieDownload.SearchOptions.FirstOrDefault(sr => sr.Title == stringResult).Value;

            var searchId = int.Parse(tvdbId);
            var imdbId = await GetMovieImdbId(searchId);
            var rawImdbId = imdbId.Replace("tt", string.Empty);
            var result = await _nzbClient.SearchForMovieAsync(rawImdbId);

            var stringFilter = userProfile.MovieDownload.Quality == MovieQuality.Quality720P ? "720p" : "1080p";
            var movies = result.Channel.Items.Where(m => m.Title.Contains(stringFilter)).ToList();

            if (movies.Count == 0)
            {
                var noResultsActivity = MessageFactory.Text(Resources.MovieDialog_ImdbSearch_NoResults);

                await stepContext.Context.SendActivityAsync(noResultsActivity, cancellationToken);
                return await stepContext.ReplaceDialogAsync(TitleWaterFall, cancellationToken: cancellationToken);
            }
            else
            {
                var options = movies.Select(resultItem =>
                    new AttachmentOption
                    {
                        Value = resultItem.Link,
                        Title = resultItem.Title
                    }).ToList();

                userProfile.MovieDownload.NzbOptions = options;

                var activity = ActivityHelpers.GetCardChoicesFromOptions(Resources.MovieDialog_SelectMovie_Message, options, AttachmentLayoutTypes.List, false);
                return await stepContext.PromptAsync(TextPromptDialog, new PromptOptions { Prompt = activity }, cancellationToken);
            }
        }

        /// <summary>
        /// FTPs the NZB.
        /// </summary>
        /// <param name="stepContext">The step context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;DialogTurnResult&gt;.</returns>
        private async Task<DialogTurnResult> FtpNzb(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await Accessors.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);
            var stringResult = stepContext.Result.ToString();

            var item = userProfile.MovieDownload.NzbOptions.FirstOrDefault(r => r.Title == stringResult);
            var bytes = await GetNzb(item.Value);
            await ActivityHelpers.FtpBytes(bytes, item.Title);

            userProfile.MovieDownload.QualitySet = false;

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Gets the NZB.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <returns>Task&lt;System.Byte[]&gt;.</returns>
        private async Task<byte[]> GetNzb(string link)
        {
            using (var client = new WebClient())
            {
                return await client.DownloadDataTaskAsync(link);
            }
        }

        /// <summary>
        /// Gets the movie imdb identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        private async Task<string> GetMovieImdbId(int id)
        {
            var client = new TMDbClient(Settings.ApiSettings.MovieDatabaseApiKey);
            var result = await client.GetMovieExternalIdsAsync(id);
            return result.ImdbId;
        }
    }
}