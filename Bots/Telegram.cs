using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using tlg = Telegram.Bot; //У меня по другому в коде не видет спесы фреймворка
using System.Collections;
using Telegram.Bot.Types.ReplyMarkups;

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
            {"/countFiles",commandCountFiles},
            {"/getFile",commandGetFile},
            {"/FilesSize",commandFilesSize}
        };

        private static string SelectedCommand = String.Empty;           //Будем запоминать какую команду использовали

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
            if (Args.Message.Type != tlg.Types.Enums.MessageType.Text)
            {
                FileHelper.SaveToFile(Args.Message, botWorker);
            }
            else
            {
                try
                {

                    SelectedCommand = Args.Message.Text; //Записываем какую команду выбрали
                    botCommand[Args.Message.Text](Args);
                }
                catch
                {
                    botWorker.SendTextMessageAsync(Args.Message.Chat.Id,"Команда не разрешена.");
                    commandStart(Args);
                }
            }
        }

        private static void SelectedType(object sendedr, tlg.Args.MessageEventArgs Args)
        {

            if (Args.Message.Type != tlg.Types.Enums.MessageType.Text)
            {
                FileHelper.SaveToFile(Args.Message, botWorker);
                return;
            }
            string ftype = Args.Message.Text.Trim('/').ToString();

            if (Args.Message.Type != tlg.Types.Enums.MessageType.Text)
                SelectedType(sendedr, Args);
            if(SelectedCommand == String.Empty || Args.Message.Text == "/return")
            {
                botWorker.OnMessage -= SelectedType;
                botWorker.OnMessage += BotLstner;
                commandStart(Args);
                return;
            }

            #region Проверка что передали именно разрешенный тип.
            bool IsFileTypes = false;
            foreach(var t in Enum.GetNames(typeof(FileTypes)))
            {
                if (ftype == t)
                    IsFileTypes = true;
            }
            if (!IsFileTypes)    //Если не разрешенный тип то выходим.
            {
                botWorker.SendTextMessageAsync(Args.Message.Chat.Id, $"Команда не распознана.\n\nРазрешенные типы для команды {SelectedCommand}:" +
                $"\n\t/{FileTypes.Audio}\n\t/{FileTypes.Document}\n\t/{FileTypes.Photo}\n\t/{FileTypes.Video}\n\t/{FileTypes.All}\n\n\n\t/return - Возврат к коммандам");
                return;
            }

            #endregion

            switch (SelectedCommand)
            {
                case "/searchByType":
                    botWorker.SendTextMessageAsync(Args.Message.Chat.Id, FileHelper.SearchByType((FileTypes)Enum.Parse(typeof(FileTypes), ftype)));
                    break;
                case "/FilesSize":
                    botWorker.SendTextMessageAsync(Args.Message.Chat.Id, FileHelper.FilesSize((FileTypes)Enum.Parse(typeof(FileTypes), ftype)));
                    break;
                case "/countFiles":
                    botWorker.SendTextMessageAsync(Args.Message.Chat.Id, FileHelper.CountFiles((FileTypes)Enum.Parse(typeof(FileTypes), ftype)));
                    break;
                case "/return":
                    botWorker.OnMessage += BotLstner;
                    botWorker.OnMessage -= SelectedType;
                    break;
                default:
                    botWorker.SendTextMessageAsync(Args.Message.Chat.Id, $"Разрешенные типы для команды {SelectedCommand}:" +
                $"\n\t/{FileTypes.Audio}\n\t/{FileTypes.Document}\n\t/{FileTypes.Photo}\n\t/{FileTypes.Video}\n\t/{FileTypes.All}\n\n\n\t/return - Возврат к коммандам");
                    break;
            }
        }

        private static void SelectedName(object sendedr, tlg.Args.MessageEventArgs Args)
        {
            if (Args.Message.Type != tlg.Types.Enums.MessageType.Text)
            {
                FileHelper.SaveToFile(Args.Message, botWorker);
                return;
            }
            if (SelectedCommand == String.Empty || Args.Message.Text == "/return")
            {
                botWorker.OnMessage -= SelectedName;
                botWorker.OnMessage += BotLstner;
                commandStart(Args);
                return;
            }
            switch (SelectedCommand)
            {
                case "/searchByName":
                    botWorker.SendTextMessageAsync(Args.Message.Chat.Id, FileHelper.SearchByName(Args.Message.Text));
                    break;
                default:
                    botWorker.SendTextMessageAsync(Args.Message.Chat.Id, $"Введите полное имя файла или его часть.\nЛибо введите /return для возврата назад");
                    break;
            }

            commandStart(Args);                    //Отображаем стартовое сообщение
            botWorker.OnMessage += BotLstner;
            botWorker.OnMessage -= SelectedName;         //Переключение на главный прослушиватель

        }

        private static void GetFile(object sendedr, tlg.Args.MessageEventArgs Args)
        {
            string[] message = Args.Message.Text.Split(' ');
            if (Args.Message.Type != tlg.Types.Enums.MessageType.Text)
            {
                FileHelper.SaveToFile(Args.Message, botWorker);
                return;
            }
            if (SelectedCommand == String.Empty || Args.Message.Text == "/return")
            {
                botWorker.OnMessage -= GetFile;
                botWorker.OnMessage += BotLstner;
                commandStart(Args);
                return;
            }

            switch(message[0])
            {
                case "/getName":
                    FileHelper.GetFile(botWorker, Args.Message, FileName: message[1]);
                    break;
                case "/getType":
                    FileHelper.GetFile(botWorker, Args.Message, ftype: (FileTypes)Enum.Parse(typeof(FileTypes), message[1]));
                    break;
                default:
                    botWorker.SendTextMessageAsync(Args.Message.Chat.Id, $"Команда не распознана.\n\n\nРазрешенные типы для команды {SelectedCommand}:" +
                $"\n\t\"/getType тип файла\" - получить файлы типа. Разрешено указывать:\n\t\t\t" +
                $"All - все файлы.\n\t\t\tVideo\n\t\t\tAudio\n\t\t\tDocument\n\t\t\tPhoto" +
                $" \n\t\"/getName полное/часть имени файла\" - получить файл по имени\n\n\n\t/return - Возврат к коммандам");
                    break;
            }
        }

        #region Методы для комманд бота. В botArgs передаем аргументы сообщения

        private static void commandStart(tlg.Args.MessageEventArgs botArgs)
        {
            botWorker.SendTextMessageAsync(botArgs.Message.Chat.Id,
                        @"Разрешенные комманды: 
                        /start - Запуск бота.
                        /searchByName - Отобразить список сохраненых файлов по части и полному имени файла.
                        /searchByType - Отобразить список сохраненых файлов по типам.
                        /getFile - Получить файлов.
                        /countFiles - Показать количество файлов.
                        /FilesSize - Показать занимаемый размер файлов");
        }

        private static void commandSearchByName(tlg.Args.MessageEventArgs botArgs)
        {
            botWorker.OnMessage -= BotLstner;
            botWorker.SendTextMessageAsync(botArgs.Message.Chat.Id, "Введите полное имя файла или его часть.");
            botWorker.OnMessage += SelectedName;
        }

        private static void commandSearchByType(tlg.Args.MessageEventArgs botArgs)
        {
            botWorker.OnMessage -= BotLstner;
            botWorker.SendTextMessageAsync(botArgs.Message.Chat.Id, $"Разрешенные типы для команды {SelectedCommand}:" +
                $"\n\t/{FileTypes.Audio}\n\t/{FileTypes.Document}\n\t/{FileTypes.Photo}\n\t/{FileTypes.Video}\n\t/{FileTypes.All}\n\n\n\t/return - Возврат к коммандам");
            botWorker.OnMessage += SelectedType;
        }

        private static void commandCountFiles(tlg.Args.MessageEventArgs botArgs)
        {
            botWorker.OnMessage -= BotLstner;
            botWorker.SendTextMessageAsync(botArgs.Message.Chat.Id, $"Разрешенные типы для команды {SelectedCommand}:" +
                $"\n\t/{FileTypes.Audio}\n\t/{FileTypes.Document}\n\t/{FileTypes.Photo}\n\t/{FileTypes.Video}\n\t/{FileTypes.All}\n\n\n\t/return - Возврат к коммандам");
            botWorker.OnMessage += SelectedType;
        }

        private static void commandFilesSize(tlg.Args.MessageEventArgs botArgs)
        {
            botWorker.OnMessage -= BotLstner;
            botWorker.SendTextMessageAsync(botArgs.Message.Chat.Id, $"Разрешенные типы для команды {SelectedCommand}:" +
                $"\n\t/{FileTypes.Audio}\n\t/{FileTypes.Document}\n\t/{FileTypes.Photo}\n\t/{FileTypes.Video}\n\t/{FileTypes.All}\n\n\n\t/return - Возврат к коммандам");
            botWorker.OnMessage += SelectedType;
        }

        private static void commandGetFile(tlg.Args.MessageEventArgs botArgs)
        {
            botWorker.OnMessage -= BotLstner;
            botWorker.SendTextMessageAsync(botArgs.Message.Chat.Id, $"Разрешенные типы для команды {SelectedCommand}:" +
                $"\n\t\"/getType тип файла\" - получить файлы типа. Разрешено указывать:\n\t\t\t" +
                $"All - все файлы.\n\t\t\tVideo\n\t\t\tAudio\n\t\t\tDocument\n\t\t\tPhoto" +
                $" \n\t\"/getName полное/часть имени файла\" - получить файл по имени\n\n\n\t/return - Возврат к коммандам");
            botWorker.OnMessage += GetFile;
        }

        #endregion



        }
}
