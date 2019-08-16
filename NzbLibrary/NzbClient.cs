using System;
using System.Net;
using System.Threading.Tasks;
using Core.Settings;
using Newtonsoft.Json;

namespace NzbLibrary
{
    /// <summary>
    /// Handles interactions with Nzbs.in. This class cannot be inherited.
    /// </summary>
    public sealed class NzbClient
    {
        private const string NzbUrlStart = "https://nzbs.in/api?apikey=";

        /// <summary>
        /// Gets or sets the API key.
        /// </summary>
        /// <value>The API key.</value>
        public string ApiKey { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NzbClient"/> class.
        /// </summary>
        /// <param name="settingsService">The settings service.</param>
        public NzbClient(SettingsService settingsService)
        {
            ApiKey = settingsService.ApiSettings.NzbsInKey;
        }

        /// <summary>
        /// Searches for movie.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;NzbData&gt;.</returns>
        public async Task<NzbData> SearchForMovieAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(ApiKey))
            {
                throw new ArgumentNullException("ApiKey cannot be empty");
            }

            var url = $"{NzbUrlStart}{ApiKey}&t=movie&imdbid={id}&o=json";

            using (var webClient = new WebClient())
            {
                var result = await webClient.DownloadStringTaskAsync(url);
                return JsonConvert.DeserializeObject<NzbData>(result);
            }
        }
    }
}