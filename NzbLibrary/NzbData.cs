using Newtonsoft.Json;

namespace NzbLibrary
{
    public class NzbData
    {
        [JsonProperty("channel")]
        public NzbChannel Channel { get; set; }
    }
}