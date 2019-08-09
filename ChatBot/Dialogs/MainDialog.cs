using System.Threading;
using System.Threading.Tasks;
using ChatBot.Business.Main.Models;
using ChatBot.Helpers;
using ChatBot.Properties;
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
    public class MainDialog : ComponentDialog
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
        public MainDialog(IConfiguration configuration, ILogger<MainDialog> logger, HelperService activityHelpers) : base(nameof(MainDialog))
        {
            Configuration = configuration;
            Logger = logger;
            ActivityHelpers = activityHelpers;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                RunInitialStepAsync,
                HandleInitialStepAsnc
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
            var activity = ActivityHelpers.CreateSimpleImMultiChoice<InitialOptions>(Resources.MainDialog_InitalStep_Message);
            await stepContext.PromptAsync()

                ChoiceFactory.


            await stepContext.Context.SendActivityAsync(activity, cancellationToken);
            return await stepContext.NextAsync(null, cancellationToken);
        }

        /// <summary>
        /// Handles the result of the initial step.
        /// </summary>
        /// <param name="stepContext">The step context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;DialogTurnResult&gt;.</returns>
        private async Task<DialogTurnResult> HandleInitialStepAsnc(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("What can I help you with today?\nSay something like \"Book a flight from Paris to Berlin on March 22, 2020\"") }, cancellationToken);
        }
    }
}