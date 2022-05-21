using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;

namespace Project_9;
class Program
{
    static ITelegramBotClient bot = new TelegramBotClient("5313407205:AAHpDxT2Pjo_0VTSJRtqLuDutSvi1bea7Bk");
    public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Некоторые действия
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
        if(update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
        {
            var message = update.Message;
            /*
            if (message.Text.ToLower() == "/start")
            {
                await botClient.SendTextMessageAsync(message.Chat, "Выберите действие:");
                await botClient.SendTextMessageAsync(message.Chat, "Загрузить файл на сревер /input");
                await botClient.SendTextMessageAsync(message.Chat, "Выгрузить файл с сервера /output");
                await botClient.SendTextMessageAsync(message.Chat, "Просмотреть файлы на сервере /view");
                return;
            }
            */
            if (message.Document != null)
            {
                Console.WriteLine($"<<File ID>> = {message.Document.FileId}");
                var file = await bot.GetFileAsync(message.Document.FileId);
                FileStream fs = new FileStream("_" + message.Document.FileName, FileMode.Create);
                await bot.DownloadFileAsync(file.FilePath, fs);
                fs.Close();
                fs.Dispose();
                await botClient.SendTextMessageAsync(message.Chat, "Файл загружен");
                return;
            }
            /*
            else if (message.Audio != null)
            {
                Console.WriteLine($"<<File ID>> = {message.Audio.FileId}");
                var file = await bot.GetFileAsync(message.Audio.FileId);
                FileStream fs = new FileStream("_" + message.Audio.FileName, FileMode.Create);
                await bot.DownloadFileAsync(file.FilePath, fs);
                fs.Close();
                fs.Dispose();
                await botClient.SendTextMessageAsync(message.Chat, "Аудио файл загружен");
                return;
            }
            */
            await botClient.SendTextMessageAsync(message.Chat, "Я не умею общаться. Только скачивать и выгружать файлы. /start");
        }
    }

    public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        // Некоторые действия
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
    }


    static void Main(string[] args)
    {
        Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { }, // receive all update types
        };

        bot.StartReceiving(HandleUpdateAsync,HandleErrorAsync,receiverOptions,cancellationToken);

        Console.ReadLine();
    }
}