// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EmptyBot v4.5.0

using System.Linq;
using ChatBot.Bots;
using ChatBot.Dialogs;
using ChatBot.Helpers;
using ChatBot.State;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ChatBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Create the credential provider to be used with the Bot Framework Adapter.
            services.AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>();

            // Create the Bot Framework Adapter.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
            services.AddSingleton<IStorage, MemoryStorage>();

            // Create the User state. (Used in this bot's Dialog implementation.)
            services.AddSingleton<UserState>();

            // Create the Conversation state. (Used by the Dialog system itself.)
            services.AddSingleton<ConversationState>();

            // The Dialog that will be run by the bot.
            services.AddSingleton<MainDialog>();
            services.AddSingleton<DownloadDialog>();
            services.AddSingleton<MovieDownloadDialog>();
            services.AddSingleton<TvDownloadDialog>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, AutomationDialogBot<MainDialog>>();

            ConfigureCustomServices(services);
        }

        /// <summary>
        /// Configures the custom services.#
        /// </summary>
        /// <param name="services">The services.</param>
        private void ConfigureCustomServices(IServiceCollection services)
        {
            services.AddSingleton<HelperService>();

            var memoryStorage = new MemoryStorage();

            // Create and register state accessors.
            // Accessors created here are passed into the IBot-derived class on every turn.
            services.AddSingleton(sp =>
            {
                // We need to grab the conversationState we added on the options in the previous step
                var conversationState = new ConversationState(memoryStorage);
                var userState = new UserState(memoryStorage);

                // Create the custom state accessor.
                // State accessors enable other components to read and write individual properties of state.
                return new MultiTurnPromptsBotAccessors(conversationState, userState)
                {
                    ConversationDialogState = conversationState.CreateProperty<DialogState>("DialogState"),
                    UserProfile = userState.CreateProperty<UserProfile>("UserProfile"),
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}
