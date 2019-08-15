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
    /// Dialog for download TV shows.
    /// Implements the <see cref="CustomComponentDialog" />
    /// </summary>
    /// <seealso cref="TvDownloadDialog" />
    public class TvDownloadDialog : CustomComponentDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TvDownloadDialog"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="activityHelpers">The activity helpers.</param>
        /// <param name="accessors">The accessors.</param>
        public TvDownloadDialog(IConfiguration configuration, ILogger<DownloadDialog> logger, HelperService activityHelpers, MultiTurnPromptsBotAccessors accessors)
            : base(DialogNames.TvDownload, accessors, configuration, activityHelpers, logger)
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }
    }
}