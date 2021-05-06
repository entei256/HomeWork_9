using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace HomeWork_9.Bots
{
    static class FileHelper
    {
        private static string path = @".\";


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

    }

    enum FileTypes
    {
        All,
        Document,
        Video,
        Audio
    }
}
