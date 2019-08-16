using ChatBot.Business.Main.Models;

namespace ChatBot.State
{
    public class MovieDownloadModel
    {
        public string Title { get; set; }

        public MovieQuality Quality { get; set; }

        public bool QualitySet { get; set; }
    }
}