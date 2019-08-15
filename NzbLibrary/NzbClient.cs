using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NzbLibrary
{
    public sealed class NzbClient
    {
        private const string NzbUrlStart = "https://nzbs.in/api?apikey=00d451a880f2a1b358161ff0f5d65672";

        /// <summary>
        /// Searches for movie.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;NzbData&gt;.</returns>
        public async Task<NzbData> SearchForMovieAsync(string id)
        {
            var url = $"{NzbUrlStart}&t=movie&imdbid={id}&o=json";

            using (var webClient = new WebClient())
            {
                var result = await webClient.DownloadStringTaskAsync(url);
                return JsonConvert.DeserializeObject<NzbData>(result);
            }
        }
    }
}