using System;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace ChatBot.Extensions
{
    public static class WaterfallExtensions
    {
        /// <summary>
        /// Gets the enum value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stepContext">The step context.</param>
        /// <returns>T.</returns>
        public static T GetEnumValueFromChoice<T>(this WaterfallStepContext stepContext) where T : Enum
        {
            var result = (FoundChoice) stepContext.Result;
            Enum.TryParse(typeof(T), result.Value, true, out var enumResult);
            return (T) enumResult;
        }
    }
}