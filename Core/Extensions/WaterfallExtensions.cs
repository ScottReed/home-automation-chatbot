using System;
using System.Linq;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace Core.Extensions
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
            var foundChoice = (FoundChoice)stepContext.Result;
            var values = Enum.GetValues(typeof(T)).Cast<Enum>();

            foreach (var value in values)
            {
                var description = value.GetEnumDescription();

                if (string.Equals(value.ToString(), foundChoice.Value, StringComparison.InvariantCultureIgnoreCase)  || string.Equals(description, foundChoice.Value, StringComparison.InvariantCultureIgnoreCase))
                {
                    return (T) value;
                }
            }

            return default(T);
        }
    }
}