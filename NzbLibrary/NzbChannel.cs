using System.Collections.Generic;
using Newtonsoft.Json;

namespace NzbLibrary
{
    public class NzbChannel
    {
        [JsonProperty("item")]
        public IEnumerable<NzbItem> Items { get; set; }
    }
}