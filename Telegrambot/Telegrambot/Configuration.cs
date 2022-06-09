using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegrambot
{
    public static class Configuration
    {
        public static readonly string BotToken = "5267025345:AAEN2QUBeYOVFWub9gWlQqn7zhKujFoDm4M";
        public static readonly InlineKeyboardMarkup InlineKeyboard = new(
                   new[]
                   {

                    new []
                    {

                        InlineKeyboardButton.WithCallbackData("USD", "USD"),
                        InlineKeyboardButton.WithCallbackData("EUR", "EUR"),
                        InlineKeyboardButton.WithCallbackData("PLN", "PLN"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("CHF", "CHF"),
                        InlineKeyboardButton.WithCallbackData("GBP", "GBP"),
                        InlineKeyboardButton.WithCallbackData("SEK", "SEK"),
                    },
                     new []
                    {
                        InlineKeyboardButton.WithCallbackData("XAU", "XAU"),
                        InlineKeyboardButton.WithCallbackData("CAD", "CAD"),

                    },
                   });
    }
}
