using Newtonsoft.Json;

namespace NzbLibrary
{
    public class NzbItem
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }
    }
}