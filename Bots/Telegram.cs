using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using tlg = Telegram.Bot; //У меня по другому в коде не видет спесы фреймворка
using System.Collections;

namespace HomeWork_9.Bots
{
    class Telegram
    {
        private static string token = File.ReadAllText(@"D:\Учеба\BotTlgToken.txt");                    //Читаем токен из файла.
        private static tlg.TelegramBotClient botWorker = new tlg.TelegramBotClient(token);             //Клиент бота.

        private delegate void commandDelegate(tlg.Args.MessageEventArgs botArgs);                                 //Делегат для выполнения команд бота
        private static Dictionary<string, commandDelegate> botCommand = new Dictionary<string, commandDelegate>()  //Справочник комманд.
        {
            { "/start" , commandStart},
            {"/searchByName",commandSearchByName},
            {"/searchByType",commandSearchByType},
            {"/getByName",commandGetByName},
            {"/getByType",commandGetByType},
            {"/countFiles",commandCountFiles},
            {"/FlesSize",commandFlesSize}
        };   

        
        public bool Runniing { get { return botWorker.IsReceiving; } }       //Пробрасываем признак работы бота.
        
        /// <summary>
        /// Запуск бота
        /// </summary>
        public void Start()
        {
            botWorker.OnMessage += BotLstner;
            botWorker.StartReceiving();
        }

        /// <summary>
        /// Остановка бота.
        /// </summary>
        public void Stop()
        {
            botWorker.OnMessage -= BotLstner;
            botWorker.StopReceiving();
        }

        
        /// <summary>
        /// Пррослушшиватель сообщений бота
        /// </summary>
        /// <param name="sendedr"></param>
        /// <param name="Args"></param>
        private static void BotLstner(object sendedr, tlg.Args.MessageEventArgs Args)
        {
            botCommand[Args.Message.Text](Args);
        }


        #region Методы для комманд бота
        private static void commandStart(tlg.Args.MessageEventArgs botArgs)
        {
            botWorker.SendTextMessageAsync(botArgs.Message.Chat.Id,
                        @"Разрешенные комманды: 
                        /start
                        /searchByName
                        /searchByType
                        /getByName
                        /getByType
                        /countFiles
                        /FlesSize");
        }

        private static void commandSearchByName(tlg.Args.MessageEventArgs botArgs)
        {
            botWorker.SendTextMessageAsync(botArgs.Message.Chat.Id, "Поиск по имени");
        }

        private static void commandSearchByType(tlg.Args.MessageEventArgs botArgs)
        {
            botWorker.SendTextMessageAsync(botArgs.Message.Chat.Id, "Поиск по типу");
        }

        private static void commandGetByName(tlg.Args.MessageEventArgs botArgs)
        {
            botWorker.SendTextMessageAsync(botArgs.Message.Chat.Id, "Получение по имени");
        }

        private static void commandGetByType(tlg.Args.MessageEventArgs botArgs)
        {
            botWorker.SendTextMessageAsync(botArgs.Message.Chat.Id, "Получение по типу");
        }

        private static void commandCountFiles(tlg.Args.MessageEventArgs botArgs)
        {
            botWorker.SendTextMessageAsync(botArgs.Message.Chat.Id, "Количество файлов");
        }

        private static void commandFlesSize(tlg.Args.MessageEventArgs botArgs)
        {
            botWorker.SendTextMessageAsync(botArgs.Message.Chat.Id, "Размер файлов");
        }
        #endregion
    }
}
