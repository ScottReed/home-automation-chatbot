using System;
using System.Collections.Generic;
using System.Linq;
using ChatBot.Extensions;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace ChatBot.Helpers
{
    /// <summary>
    /// Helpers for working with actions.
    /// </summary>
    public sealed class HelperService
    {
        /// <summary>
        /// Creates the simple im multi choice.
        /// </summary>
        /// <typeparam name="T">The type of enum</typeparam>
        /// <param name="question">The question.</param>
        /// <returns>Activity.</returns>
        public Activity CreateSimpleImMultiChoice<T>(string question) where T : Enum
        {
            var enumDictionary = GetDescriptionValueArray<T>();

            var reply = MessageFactory.Text(question);
            reply.SuggestedActions = new SuggestedActions
            {
                Actions = enumDictionary.Select(val => new CardAction { Title = val.Value, Type = ActionTypes.ImBack, Value = val.Key}).ToList()
            };

            return reply;
        }

        /// <summary>
        /// Gets the description value array using the value and description.
        /// </summary>
        /// <typeparam name="T">The type of enum</typeparam>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public Dictionary<string, string> GetDescriptionValueArray<T>() where T : Enum
        {
            var values = Enum.GetValues(typeof(T)).Cast<Enum>();
            var dictionary = new Dictionary<string, string>();

            foreach (var value in values)
            {
                dictionary.Add(value.ToString(), value.GetEnumDescription());
            }

            return dictionary;
        }


        public PromptOptions GetChoicesFromEnum<T>(string question, string retry) where T : Enum
        {
            var enumDictionary = GetDescriptionValueArray<T>();

            return new PromptOptions()
            {
                Prompt = MessageFactory.Text(question),
                Choices = enumDictionary.Select(val => new Choice {}  new CardAction {Title = val.Value, Type = ActionTypes.ImBack, Value = val.Key})
                RetryPrompt = MessageFactory.Text(retry)
            };

            ChoiceFactory
        }
    }
}