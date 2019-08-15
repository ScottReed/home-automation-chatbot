using System.Threading;
using System.Threading.Tasks;
using ChatBot.Business.Main.Models;
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
        protected readonly IConfiguration Configuration;
        protected readonly ILogger Logger;
        protected readonly HelperService ActivityHelpers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainDialog" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="activityHelpers">The activity helpers.</param>
        /// <param name="accessors">The accessors.</param>
        public MainDialog(IConfiguration configuration, ILogger<MainDialog> logger, HelperService activityHelpers, MultiTurnPromptsBotAccessors accessors) : base(nameof(MainDialog), accessors)
        {
            Configuration = configuration;
            Logger = logger;
            ActivityHelpers = activityHelpers;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                RunInitialStepAsync,
                HandleInitialStepAsnc,
                HandleSecondStepAsync
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
            return await stepContext.PromptAsync(nameof(ChoicePrompt), ActivityHelpers.GetChoicesFromEnum<InitialOptions>(Resources.MainDialog_InitalStep_Message, null), cancellationToken);
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

            var initialOption = stepContext.GetEnumValueFromChoice<InitialOptions>();
            userProfile.MainAction = initialOption;

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("What can I help you with today?\nSay something like \"Book a flight from Paris to Berlin on March 22, 2020\"") }, cancellationToken);
        }

        private async Task<DialogTurnResult> HandleSecondStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Get the current profile object from user state.
            var userProfile = await Accessors.UserProfile.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("test") }, cancellationToken);
        }
    }
}