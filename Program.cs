using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;


namespace Project_9
{

    class Program
    {
        static ITelegramBotClient bot = new TelegramBotClient("5317518652:AAE69hx6ltXEHOfXm7EsHFnXYkd0g-O48vk");
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if(update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Выберите команду.");
                    await botClient.SendTextMessageAsync(message.Chat, "/view - Просмотр сохраненных файлов;");
                    await botClient.SendTextMessageAsync(message.Chat, "/save - Сохранить файл;");
                    await botClient.SendTextMessageAsync(message.Chat, "/download - Скачать файл;");
                    return;
                }
                if (message.Text.ToLower() == "/view")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Просмотр сохраненных файлов;");
                    return;
                }
                if (message.Text.ToLower() == "/save")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Сохранить файл;");
                    return;
                }
                 if (message.Text.ToLower() == "/download")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Скачать файл;");
                    return;
                }
                await botClient.SendTextMessageAsync(message.Chat, "Привет, выбери команду. Чтобы просмотреть команды введи /start");
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
            var receiverOptions = new ReceiverOptions {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken);
            Console.ReadLine();
        }
    }
}