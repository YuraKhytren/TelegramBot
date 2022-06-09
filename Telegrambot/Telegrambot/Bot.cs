using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegrambot.Service;

namespace Telegrambot
{
    public class Bot
    {
        private readonly IService _service;

        public Bot(IService service)
        {
            _service = service;
        }
        public async Task StartProgram()
        {
            TelegramBotClient? Bot;

            Bot = new TelegramBotClient(Configuration.BotToken);

            User me = await Bot.GetMeAsync();
            Console.Title = me.Username ?? "My awesome Bot";

            using var cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new() { AllowedUpdates = { } };
            Bot.StartReceiving(HandleUpdateAsync,
                               HandleErrorAsync,
                               receiverOptions,
                               cts.Token);

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();

        }
        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            var handler = update.Type switch
            {

                UpdateType.Message => BotOnMessageReceived(botClient, update.Message!),
                UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage!),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery!),

            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }
        private async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            DateTime date;
            Console.WriteLine($"Receive message type: {message.Type}");
            if (message.Type != MessageType.Text)
                return;

            if (DateTime.TryParse(message.Text, out date))
            {
                await SendInlineKeyboard(botClient, message);
            }
            else
            {
                var action = message.Text!.Split(' ').First() switch
                {
                    "/start" => EnterDateMassege(botClient, message),
                    "/end" => EndProgram(botClient, message),
                    _ => Usage(botClient, message)

                };

                Message sentMessage = await action;
                Console.WriteLine($"The message was sent with id: {sentMessage.MessageId}");

            }

            async Task<Message> EndProgram(ITelegramBotClient botClient, Message message)
            {
                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: "Thank you for using our bot",
                                                            replyMarkup: new ReplyKeyboardRemove()
                                                            );
            }

            async Task<Message> EnterDateMassege(ITelegramBotClient botClient, Message message)
            {
                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: "Enter Date format: 25.10.2020\n And choose curency"
                                                            );
            }

            async Task<Message> SendInlineKeyboard(ITelegramBotClient botClient, Message message)
            {
                await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: message.Text,
                                                            replyMarkup: Configuration.InlineKeyboard);
            }
            async Task<Message> Usage(ITelegramBotClient botClient, Message message)
            {
                const string usage = "Incorrect date\nWelcome. This bot shows the hryvnia exchange rate on a entered date .\n" +
                                     "/start   - start a program\n";

                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: usage,
                                                            replyMarkup: new ReplyKeyboardRemove());
            }


        }
        private async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            DateTime dateTime = Convert.ToDateTime(callbackQuery.Message.Text);
            var curencyList = await _service.GetCurencyList(dateTime);

            if (curencyList.ExchangeRate == null || curencyList.ExchangeRate.Count == 0)
            {
                await botClient.SendTextMessageAsync(
                 chatId: callbackQuery.Message.Chat.Id,
                 text: $"Date should not be later than -- {DateTime.Now.ToString("MM,dd,yyyy")}\n To end the program, press /end");
            }

            Curency currentCurency = curencyList.ExchangeRate.FirstOrDefault(c => c.Currency == callbackQuery.Data);

            if (currentCurency != null)
            {
                if (currentCurency.PurchaseRate == 0 || currentCurency.SaleRate == 0)
                {
                    await botClient.SendTextMessageAsync(
                     chatId: callbackQuery.Message.Chat.Id,
                     text: $"1{currentCurency.BaseCurrency} -- 1{currentCurency.Currency}\n" + "no exchange rate data\n To end the program, press /end");
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: $"1{currentCurency.BaseCurrency} -- 1{currentCurency.Currency}\n" +
                        $"PurchaseRate {currentCurency.PurchaseRate}-- SaleRate {currentCurency.SaleRate}\n To end the program, press /end");

                }

            }
            else
            {
                await botClient.SendTextMessageAsync(
                 chatId: callbackQuery.Message.Chat.Id,
                 text: "Invalid Currency\n To end the program, press /end");
            }

        }
    }
}
