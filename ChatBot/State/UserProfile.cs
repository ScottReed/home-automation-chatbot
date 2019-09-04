using ChatBot.Business.Main.Models;
using Models.State;

namespace ChatBot.State
{
    public class UserProfile : BaseUserProfile
    {
        public InitialOptions MainAction { get; set; }

        public DownloadType DownloadType { get; set; }

        public MovieDownloadModel MovieDownload { get; set; } = new MovieDownloadModel();
    }
}