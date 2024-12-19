using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;

var botClient = new TelegramBotClient("7602022367:AAEtWzIlKxOu9ltoNhsIXucPwLq7TrmoKWo");
using var cts = new CancellationTokenSource();

// Настройка обработчиков
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>() // получать все виды обновлений
};

botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken: cts.Token);

var me = await botClient.GetMeAsync();
Console.WriteLine($"Бот {me.Username} запущен... Нажмите Enter для остановки.");
Console.ReadLine();

cts.Cancel();

// Обработка обновлений
async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
{
    if (update.Type == UpdateType.Message && update.Message?.Text != null)
    {
        await HandleMessageAsync(bot, update.Message, cancellationToken);
    }
    else if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
    {
        await HandleCallbackQueryAsync(bot, update.CallbackQuery, cancellationToken);
    }
}

async Task HandleMessageAsync(ITelegramBotClient bot, Message message, CancellationToken cancellationToken)
{
    if (message.Text == "/start")
    {
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Исследовать кладовку", "closet"),
                InlineKeyboardButton.WithCallbackData("Посетить кухню", "kitchen")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Выйти на балкон", "balcony"),
                InlineKeyboardButton.WithCallbackData("Отдохнуть в гостиной", "living_room")
            }
        });

        await bot.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Добро пожаловать на домашнюю экскурсию! Выберите направление:",
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);
    }
}

async Task HandleCallbackQueryAsync(ITelegramBotClient bot, CallbackQuery callbackQuery, CancellationToken cancellationToken)
{
    var responseText = callbackQuery.Data switch
    {
        "closet" => "Вы нашли сундук с воспоминаниями в кладовке!",
        "kitchen" => "Кухня шумит, как настоящий тропический курорт!",
        "balcony" => "С балкона открывается прекрасный вид на ваш двор!",
        "living_room" => "Гостиная – это уютное место для отдыха!",
        _ => "Этот маршрут ещё строится!"
    };

    await bot.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
    await bot.SendTextMessageAsync(
        chatId: callbackQuery.Message.Chat.Id,
        text: responseText,
        cancellationToken: cancellationToken);
}

Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
{
    Console.WriteLine($"Произошла ошибка: {exception.Message}");
    return Task.CompletedTask;
}
