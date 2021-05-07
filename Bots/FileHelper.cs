using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using tlgtype =Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace HomeWork_9.Bots
{
    static class FileHelper
    {
        private static string path = Directory.GetCurrentDirectory() +"\\Downloads\\";


        public static string[] SearchByName(string FileName)
        {
            string[] resoult;


            return new string[1];
        }

        public static string[] SearchByType(FileTypes type)
        {
            string[] resoult;


            return new string[1];
        }
        
        public static int CountFiles(FileTypes type = FileTypes.All)
        {
            int resoult = 0;


            return resoult;
        }

        public static string FilesSize(FileTypes type = FileTypes.All)
        {



            return "";
        }

        internal static async void SaveToFile(tlgtype.Message message,TelegramBotClient bot)
        {
            string FileName = String.Empty;
            if (!System.IO.Directory.Exists(path + message.Type))
                Directory.CreateDirectory(path + message.Type);

            tlgtype.File file = null;

            switch (message.Type)
            {
                case MessageType.Photo:
                    FileName = String.Format("{0}.{1}", DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss"), message.Type);
                    file = await bot.GetFileAsync(message.Photo[message.Photo.Length-1].FileId);
                    break;
            }

            
            
            
            
            
            if (FileName == String.Empty)
                return;

            var fs = new FileStream(path + message.Type+"\\"+FileName, FileMode.Create);

            await bot.DownloadFileAsync(file.FilePath,fs);

            bot.SendTextMessageAsync(message.Chat.Id, String.Format(@"Файл сохранен по пути: {0}
Тип файла:{1}",fs.Name,message.Type));
            fs.Close();
            fs.Dispose();
        }
    }

    enum FileTypes
    {
        All,
        Document,
        Video,
        Audio
    }
}
