using Microsoft.Extensions.Configuration;

namespace Core.Settings
{
    public sealed class SettingsService
    {
        /// <summary>
        /// Gets or sets the API settings.
        /// </summary>
        /// <value>The API settings.</value>
        public ApiSettings ApiSettings { get; set; }

        /// <summary>
        /// Gets or sets the FTP settings.
        /// </summary>
        /// <value>The FTP settings.</value>
        public FtpSettings FtpSettings { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsService"/> class.
        /// </summary>
        public SettingsService(IConfiguration configuration)
        {
            var apiSettings = new ApiSettings();
            configuration.GetSection("ApiSettings").Bind(apiSettings);

            var ftpSettings = new FtpSettings();
            configuration.GetSection("FtpSettings").Bind(ftpSettings);

            ApiSettings = apiSettings;
            FtpSettings = ftpSettings;
        }
    }
}