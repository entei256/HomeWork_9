using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using tlgtype =Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using System.Threading;
using Telegram.Bot.Types.InputFiles;

namespace HomeWork_9.Bots
{
    static class FileHelper
    {
        private static string path = Directory.GetCurrentDirectory() +"\\Downloads\\";   //Переменная корнегово пути.
        private static Dictionary<string, List<FileInfo>> Catalogs = new Dictionary<string, List<FileInfo>>();      //Справочник Каталогов и файлов.



        /// <summary>
        /// Поиск по имени файла.
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static string SearchByName(string FileName)
        {
            StringBuilder resoult = new StringBuilder();
            
            LoadCatalogsAndFiles();

            foreach(var catalog in Catalogs)
            {
                foreach(var file in catalog.Value)
                {
                    if (file.Name.Contains(FileName))
                    {
                        resoult.Append(String.Format("Имя файла: {0}\n\r",file.Name));
                    }
                }
            }
            if (resoult.Length <= 0)
            {
                return "Ни чего не найдено!";
            }

            return resoult.ToString();
        }

        /// <summary>
        /// Поиск по типу файла.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string SearchByType(FileTypes type)
        {
            StringBuilder resoult = new StringBuilder();

            LoadCatalogsAndFiles();

            foreach(var catalog in Catalogs)
            {
                if(type == FileTypes.All)            
                {
                    foreach(var file in catalog.Value)
                    {
                        resoult.AppendLine(String.Format("/{0}",file.Name));    //Выводим все файлы
                    }
                }
                else if (catalog.Key.EndsWith(type.ToString()))
                {
                    foreach (var file in catalog.Value)
                    {
                        resoult.AppendLine(String.Format("{0}", file.Name));     //Выводим файлы нужного типа.
                    }
                    break;
                }
            }

            return resoult.ToString();
        }
        

        /// <summary>
        /// Подсчет количества файлов
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string CountFiles(FileTypes type = FileTypes.All)
        {
            int resoult = 0;

            LoadCatalogsAndFiles();

            foreach (var Catalog in Catalogs)   //Проходимся по всем вложенные каталоги и проходимся по каждому
            {
                if(type == FileTypes.All)
                {
                    resoult += Catalog.Value.Count;        //Считаем сколько файлов в каталогах.
                }
                else if (Catalog.Key.EndsWith(type.ToString()))
                {
                    resoult += Catalog.Value.Count;        //Считаем сколько файлов в нужном каталоге.
                }
            }

            return String.Format("Количество файлов: {0}",resoult);
        }

        /// <summary>
        /// Подсчет размера всех файлов.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string FilesSize(FileTypes type = FileTypes.All)
        {
            Double resoult = 0; //Long что бы не конвертировать.
            int measure = 0;     //Единица измерения

            LoadCatalogsAndFiles();

            foreach (var Catalog in Catalogs)   //Получаем все вложенные каталоги и проходимся по каждому
            {
                if(type == FileTypes.All)
                {
                    foreach (var file in Catalog.Value) //Проходимся по всем файлам.
                    {
                        resoult += file.Length;
                    }
                }
                else if (Catalog.Key.EndsWith(type.ToString()))
                {
                    foreach (var file in Catalog.Value) //Проходимся по всем файлам вложенного каталога.
                    {
                        resoult += file.Length;
                    }
                    break;
                }
            }

            while((resoult / 1024) > 1)           //Подсчитываем еденицу измерения
            {
                resoult = resoult / 1024;
                measure += 1;
            }


            return String.Format("Размер файлов : {0:0.00} {1}", resoult,(EMeasure)measure );
        }

        /// <summary>
        /// Сохранение в файл.
        /// </summary>
        /// <param name="message">Мередать сообщение полученное ботом</param>
        /// <param name="bot">передать самого бота</param>
        internal static async void SaveToFile(tlgtype.Message message,TelegramBotClient bot)
        {
            string FileName = String.Empty;  //Временная переменная где храним название файла.
            tlgtype.File file = null;        //Временный обьек для сохранения файла.
            

            if (!System.IO.Directory.Exists(path + message.Type))     //Проверяем что папка существует. Если нет , то создаем ее.
                Directory.CreateDirectory(path + message.Type);

            

            switch (message.Type)                           //Проверяем тип сообщения.
            {
                case MessageType.Photo:                      //Если фото.
                    tlgtype.PhotoSize[] photos = message.Photo;      //Получаем массив фото.
                    FileStream fsPoto;
                    for (int iter = 0; iter <= photos.Length -1; iter++)               //Проходимся по массиву.
                    {
                        FileName = String.Format("{0}_{2}.{1}", DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss_fffff"), "jpg",iter.ToString()); //Формируем имя файла для сохранения.
                        file = await bot.GetFileAsync(message.Photo[iter].FileId);                             //Получает файл во временную переменную.

                        #region т.к. надо несколко файлов сохранять с разными именами. То решил что самое простое будет сразу сохранять все.
                        fsPoto = new FileStream(path + message.Type + "\\" + FileName, FileMode.OpenOrCreate);    //Создаем файловый поток.
                        await bot.DownloadFileAsync(file.FilePath, fsPoto);                                    //Скачиваем файл.
                        bot.SendTextMessageAsync(message.Chat.Id, String.Format(@"Файл сохранен по пути: {0}
Тип файла:{1}", fsPoto.Name, message.Type));             //выводим сообщение пользователю.

                        fsPoto.Close();
                        fsPoto.Dispose();
                        Thread.Sleep(100);             //Делаем задержку. На тесте телега лагать начала.
                        #endregion
                    }
                    return;
                case MessageType.Document:                     //Если документ.
                    FileName = message.Document.FileName;     //Формируем имя файла для сохранения.
                    file = await bot.GetFileAsync(message.Document.FileId);            //Получает файл во временную переменную.
                    break;
                case MessageType.Video:                        //Если видео
                    FileName = String.Format("{0}.{1}", DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss"), "mp4");       //Формируем имя файла для сохранения.
                    file = await bot.GetFileAsync(message.Video.FileId);                       //Получает файл во временную переменную.
                    break;
                case MessageType.Audio:                 //Если Аудио.
                    FileName = String.Format("{0}-{2}.{1}", message.Audio.Performer, "mp3",message.Audio.Title);           //Формируем имя файла для сохранения.
                    file = await bot.GetFileAsync(message.Audio.FileId);                                          //Получает файл во временную переменную.
                    break;
                default:                                    //Ну и на крайний случай если не один из выше перечисленных.
                    bot.SendTextMessageAsync(message.Chat.Id, "Тип файла не распознан.");               //Сообщаем что бот не знает о такой типе файла.
                    return;                         //выходим из метода сохранения
            }



            if (FileName == String.Empty)       //На крайний случай. Если имя файла не сформировано то выходим из метода сохранения
                return;


            var fs = new FileStream(path + message.Type+"\\"+FileName, FileMode.OpenOrCreate);    //Создаем файловый поток.

            await bot.DownloadFileAsync(file.FilePath,fs);                        //Скачиваем файлы.

            bot.SendTextMessageAsync(message.Chat.Id, String.Format(@"Файл сохранен по пути: {0}
Тип файла:{1}",fs.Name,message.Type));             //выводим сообщение пользователю.
            fs.Close();                           //Закрываем файловый поток
            fs.Dispose();                         //Помечаем утилизируем файловый поток. Вроде как это не само удаление, а пометка для сборщика + можно финализирующую логику писать для Dispose()
            Thread.Sleep(100);                    //Задержка что бы телега не лагала.
        }

        public static async void GetFile(TelegramBotClient bot, tlgtype.Message message, string FileName = "", FileTypes ftype = FileTypes.All)
        {
            LoadCatalogsAndFiles();
            List<FileInfo> resoult = new List<FileInfo>();


            #region Если указан тип
            if (ftype != FileTypes.All)
            { 
                foreach(var catalog in Catalogs)
                {
                    if (catalog.Key.EndsWith(ftype.ToString()))    //Если тип файлов найден.
                    {
                        resoult.AddRange(catalog.Value);
                    }
                }
            }
            #endregion

            #region Если указано имя файла
            if(FileName != "")
            {
                foreach(var catalog in Catalogs)
                {
                    foreach(var file in catalog.Value)
                    {
                        if(file.Name.Contains(FileName))
                        {
                            resoult.Add(file);
                        }
                    }
                }
            }
            #endregion



            #region Отправка файлов в чат

            foreach (var r in resoult)
            {
                using (FileStream fs = File.OpenRead(r.FullName))
                {
                    InputOnlineFile inputOnlineFile = new InputOnlineFile(fs, r.Name);
                    await bot.SendDocumentAsync(message.Chat.Id, inputOnlineFile);
                }
                Thread.Sleep(200);
            }
            #endregion
        }

        /// <summary>
        /// Вспомогателный метод что бы заполнить справочник каталогов и файлов.
        /// </summary>
        private static void LoadCatalogsAndFiles()
        {
            Catalogs.Clear();  //Очищаем справочник что бы повторно загрузить все.

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            foreach (var Catalog in Directory.GetDirectories(path))   //Получаем все вложенные каталоги и проходимся по каждому
            {
                Catalogs.Add(Catalog, new List<FileInfo>());               //Добовляем новый каталог в справочник
                foreach (var file in Directory.GetFiles(Catalog)) //Проходимся по всем файлам вложенного каталога.
                {
                    Catalogs[Catalog].Add(new FileInfo(file));               //Добовляем найденные файлы.
                }
            }
        }


    }


    


    //Хотел сделать красиво. Не взлетело. Если будет время надо сделать разбивку подсчетов для каждого типа файлов.
    public enum FileTypes
    {
        All,
        Document,
        Video,
        Audio,
        Photo
    }

    /// <summary>
    /// Перечесление едениц измерения.
    /// </summary>
    public enum EMeasure
    {
        Байт = 0,
        Килобайт = 1,
        Мегабайт = 2,
        Гигабайт = 3,
        Терабайт = 4
    }
}
