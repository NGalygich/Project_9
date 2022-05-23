using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using System.Text;

namespace Project_9;
class Program
{
    static string token = System.IO.File.ReadAllText(@"C:\Users\nikit\Desktop\SkillBox\token.txt");
    static string path = @"C:\Users\nikit\Desktop\SkillBox\Archive\";
    static bool flag = false, flagVideo = false;
    static string type1;
    static string type2;
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
                await botClient.SendTextMessageAsync(message.Chat, "Выгрузить файл (кроме видео) c сревера /output");
                await botClient.SendTextMessageAsync(message.Chat, "Выгрузить видео с сервера /output_video");
                await botClient.SendTextMessageAsync(message.Chat, "Просмотреть файлы на сервере /view");
                return;
            }
            else if ((message.Text != null) && (message.Text.ToLower() == "/view"))
            {
                string[] dir = Directory.GetFiles(path);
                foreach (string el in dir){
                    await botClient.SendTextMessageAsync(message.Chat, el.Substring(path.Length));
                }
                return;
            }
            else if ((message.Text != null) && (message.Text.ToLower() == "/output")){
                flagVideo = false;
                flag = true;
                return;
            }  
            else if ((message.Text != null) && (message.Text.ToLower() == "/output_video")){
                flagVideo = true;
                flag = true;
                return;
            }  
            else if ((message.Text != null) && (flag == false))
                await botClient.SendTextMessageAsync(message.Chat, "Я не умею общаться. Только скачивать и выгружать файлы.");
            
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
            if ((message.Text != null) && (flag == true)){
                 if (flagVideo == true){
                    type1 = "video";
                    type2 = "Video";
                }
                else {
                    type1 = "document";
                    type2 = "Document";
                }
                string fileNameInput = message.Text;
                string[] dir = Directory.GetFiles(path);
                foreach (string el in dir){
                    if (message.Text == el.Substring(path.Length))
                    {
                        string filePath = path + fileNameInput;
                        using (var form = new MultipartFormDataContent())
                        {
                            form.Add(new StringContent(message.Chat.Id.ToString(), Encoding.UTF8), "chat_id");
                            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                            {   
                                form.Add(new StreamContent(fileStream), type1, filePath.Split('\\').Last());
                                using (var client = new HttpClient())
                                {
                                    await client.PostAsync($"https://api.telegram.org/bot{token}/send{type2}", form);
                                    Console.WriteLine($"Файл отправлен успешно!");
                                }
                            }
                            flag = false;
                            flagVideo = false;
                            break;
                        }
                    } 
                }
                if (flag == true) await botClient.SendTextMessageAsync(message.Chat, "Выбраный файл не найден. Попробуйте ввести еще раз.");
                return;
            }
        }
    }
    public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
    }
    static void Main()
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