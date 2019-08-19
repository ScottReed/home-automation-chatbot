using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ChatBot.Dialogs;
using ChatBot.Extensions;
using Core.Settings;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace ChatBot.Helpers
{
    /// <summary>
    /// Helpers for working with actions.
    /// </summary>
    public sealed class HelperService
    {
        private readonly ILogger<HelperService> _logger;
        private readonly SettingsService _settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelperService" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="settingsService">The settings service.</param>
        public HelperService(ILogger<HelperService> logger, SettingsService settingsService)
        {
            _logger = logger;
            _settingsService = settingsService;
        }

        /// <summary>
        /// Creates the simple im multi choice.
        /// </summary>
        /// <typeparam name="T">The type of enum</typeparam>
        /// <param name="question">The question.</param>
        /// <returns>Activity.</returns>
        public Activity CreateSimpleImMultiChoice<T>(string question) where T : Enum
        {
            var enumDictionary = GetDescriptionValueArray<T>();

            var reply = MessageFactory.Text(question);
            reply.SuggestedActions = new SuggestedActions
            {
                Actions = enumDictionary.Select(val => new CardAction { Title = val.Value, Type = ActionTypes.ImBack, Value = val.Key}).ToList()
            };

            return reply;
        }

        /// <summary>
        /// Gets the description value array using the value and description.
        /// </summary>
        /// <typeparam name="T">The type of enum</typeparam>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public Dictionary<string, string> GetDescriptionValueArray<T>() where T : Enum
        {
            var values = Enum.GetValues(typeof(T)).Cast<Enum>();
            var dictionary = new Dictionary<string, string>();

            foreach (var value in values)
            {
                dictionary.Add(value.ToString(), value.GetEnumDescription());
            }

            return dictionary;
        }

        /// <summary>
        /// Gets the choices from enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="question">The question.</param>
        /// <param name="retry">The retry.</param>
        /// <returns>PromptOptions.</returns>
        public PromptOptions GetChoicesFromEnum<T>(string question, string retry) where T : Enum
        {
            var enumDictionary = GetDescriptionValueArray<T>();

            return new PromptOptions()
            {
                Prompt = MessageFactory.Text(question),
                Choices = enumDictionary.Select(val =>
                    new Choice
                    {
                        Value  = val.Value,
                        Action = 
                            new CardAction
                            {
                                Title = val.Value,
                                Type = ActionTypes.ImBack,
                                Text = val.Value,
                                Value = val.Value
                            },
                        Synonyms = null
                    }).ToList(),
                RetryPrompt = MessageFactory.Text(retry)
            };
        }

        /// <summary>
        /// Gets the card choices from options.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <param name="list">The list.</param>
        /// <param name="layoutType">Type of the layout.</param>
        /// <param name="hasImages">if set to <c>true</c> [has images].</param>
        /// <returns>Activity.</returns>
        public Activity GetCardChoicesFromOptions(string question, IEnumerable<AttachmentOption> list, string layoutType, bool hasImages)
        {
            var reply = MessageFactory.Text(question);
            reply.AttachmentLayout = layoutType;

            foreach (var listItem in list)
            {
                var cardButtons = new List<CardAction>();

                CardAction plButton = new CardAction()
                {
                    Value = listItem.Title,
                    Title = listItem.Title,
                    Type = ActionTypes.ImBack
                };

                cardButtons.Add(plButton);

                HeroCard plCard = new HeroCard()
                {
                    Buttons = cardButtons
                };

                if (hasImages)
                {
                    var cardImages = new List<CardImage>
                    {
                        new CardImage(url: listItem.ImageUrl)
                    };

                    plCard.Images = cardImages;
                }

                var attachment = plCard.ToAttachment();
                reply.Attachments.Add(attachment);
            }

            return reply;
        }

        /// <summary>
        /// Formats the TMDB poster path.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>System.String.</returns>
        public string FormatTmdbPosterPath(string id)
        {
            return "http://image.tmdb.org/t/p/w154" + id;
        }

        /// <summary>
        /// FTPs the bytes.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>Task.</returns>
        public async Task FtpBytes(byte[] bytes, string filename)
        {
            try
            {
                var path = _settingsService.FtpSettings.FtpPath + $"/{filename}.nzb";

                // Get the object used to communicate with the server.
                var request = (FtpWebRequest)WebRequest.Create(path);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // Get network credentials.
                request.Credentials = new NetworkCredential(_settingsService.FtpSettings.Username, _settingsService.FtpSettings.Password);

                // Write the bytes into the request stream.
                request.ContentLength = bytes.Length;

                using (var requeststream = await request.GetRequestStreamAsync())
                {
                    await requeststream.WriteAsync(bytes, 0, bytes.Length);
                    requeststream.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, null);
            }
        }
    }
}