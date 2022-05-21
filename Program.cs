using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using System.Text;

namespace Project_9;
class Program
{
    static string token = System.IO.File.ReadAllText(@"C:\Users\nikit\Desktop\SkillBox\token.txt");
    static string path = @"C:\Users\nikit\Desktop\SkillBox\Archive";
    static ITelegramBotClient bot = new TelegramBotClient(token);
    

    public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
        if(update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
        {
            var message = update.Message;
            
            if ((message.Text != null) && (message.Text.ToLower() == "/start"))
            {
                await botClient.SendTextMessageAsync(message.Chat, "Выберите действие:");
                await botClient.SendTextMessageAsync(message.Chat, "Загрузить файл на сревер /input");
                await botClient.SendTextMessageAsync(message.Chat, "Выгрузить файл с сервера /output");
                await botClient.SendTextMessageAsync(message.Chat, "Просмотреть файлы на сервере /view");
                return;
            }
            if ((message.Text != null) && (message.Text.ToLower() == "/view"))
            {
                string[] dir = Directory.GetFiles(path);
                foreach (string el in dir)
                    await botClient.SendTextMessageAsync(message.Chat, el.Substring(path.Length+1));
            }
            
            if (message.Document != null)
            {
                Console.WriteLine($"<<File ID>> = {message.Document.FileId}");
                var file = await bot.GetFileAsync(message.Document.FileId);
                FileStream fs = new FileStream(path + message.Document.FileName, FileMode.Create);
                await bot.DownloadFileAsync(file.FilePath, fs);
                fs.Close();
                fs.Dispose();
                await botClient.SendTextMessageAsync(message.Chat, "Файл загружен");
                return;
            }
            
            if (message.Audio != null)
            {
                Console.WriteLine($"<<File ID>> = {message.Audio.FileId}");
                var file = await bot.GetFileAsync(message.Audio.FileId);
                FileStream fs = new FileStream(path + message.Audio.FileName, FileMode.Create);
                await bot.DownloadFileAsync(file.FilePath, fs);
                fs.Close();
                fs.Dispose();
                await botClient.SendTextMessageAsync(message.Chat, "Аудио файл загружен");
                return;
            }
            string fileNameInput = "maxresdefault.jpg";
            string filePath = path + fileNameInput;
            using (var form = new MultipartFormDataContent())
            {
                form.Add(new StringContent(message.Chat.Id.ToString(), Encoding.UTF8), "chat_id");
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    form.Add(new StreamContent(fileStream), "photo", filePath.Split('\\').Last());
                    using (var client = new HttpClient())
                    {
                        await client.PostAsync($"https://api.telegram.org/bot{token}/sendPhoto", form);
                        Console.WriteLine("Файл отправлен успешно!");
                    }
                }
            }
           
            await botClient.SendTextMessageAsync(message.Chat, "Я не умею общаться. Только скачивать и выгружать файлы.");
        }
    }

    public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
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
        //bot.StartReceiving(HandleUpdateAsync,HandleErrorAsync,receiverOptions,cancellationToken);
        
        Console.ReadLine();
    }
}