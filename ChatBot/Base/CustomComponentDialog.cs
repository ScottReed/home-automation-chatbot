using System;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Helpers;
using ChatBot.State;
using Core.Settings;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ChatBot.Base
{
    public class CustomComponentDialog : ComponentDialog
    {
        protected readonly IConfiguration Configuration;
        protected readonly HelperService ActivityHelpers;
        protected readonly ILogger Logger;
        protected readonly SettingsService Settings;
        protected readonly UserState UserState;
        protected readonly IStatePropertyAccessor<UserProfile> Accessors;

        protected const string TextPromptDialog = nameof(TextPrompt);
        protected const string ChoicePromptDialog = nameof(ChoicePrompt);

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomComponentDialog" /> class.
        /// </summary>
        /// <param name="dialogId">The dialog identifier.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <exception cref="ArgumentNullException">dialogId</exception>
        /// <exception cref="System.ArgumentNullException">dialogId</exception>
        public CustomComponentDialog(string dialogId, ILogger logger, IServiceProvider serviceProvider) : base(dialogId)
        {
            Configuration = serviceProvider.GetService<IConfiguration>();
            ActivityHelpers = serviceProvider.GetService<HelperService>();
            Logger = logger;
            Settings = serviceProvider.GetService<SettingsService>();
            UserState = serviceProvider.GetService<UserState>();
            Accessors = UserState.CreateProperty<UserProfile>("UserProfile");

            if (string.IsNullOrEmpty(dialogId))
            {
                throw new ArgumentNullException(nameof(dialogId));
            }

            AddDialog(new TextPrompt(TextPromptDialog));
            AddDialog(new ChoicePrompt(ChoicePromptDialog));
        }
    }
}