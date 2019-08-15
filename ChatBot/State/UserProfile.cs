using ChatBot.Business.Main.Models;

namespace ChatBot.State
{
    /// <summary>
    /// User profile for handling profile information.
    /// </summary>
    public class UserProfile
    {
        public InitialOptions MainAction { get; set; }

        public DownloadType DownloadType { get; set; }

        public MovieDownloadModel MovieDownload { get; set; } = new MovieDownloadModel();
    }
}