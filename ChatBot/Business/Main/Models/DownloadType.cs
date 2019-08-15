using System.ComponentModel;

namespace ChatBot.Business.Main.Models
{
    /// <summary>
    /// The download type
    /// </summary>
    public enum DownloadType
    {
        [Description("TV")]
        Tv,
        [Description("Movie")]
        Movie
    }
}