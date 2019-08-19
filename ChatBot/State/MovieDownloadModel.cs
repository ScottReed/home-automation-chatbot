using System.Collections.Generic;
using ChatBot.Business.Main.Models;
using ChatBot.Helpers;

namespace ChatBot.State
{
    public class MovieDownloadModel
    {
        public string Title { get; set; }

        public MovieQuality Quality { get; set; }

        public bool QualitySet { get; set; }

        public IEnumerable<AttachmentOption> SearchOptions { get; set; }

        public IEnumerable<AttachmentOption> NzbOptions { get; set; }
    }
}