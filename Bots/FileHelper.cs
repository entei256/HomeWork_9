using System;
using System.Collections.Generic;
using System.Text;

namespace HomeWork_9.Bots
{
    static class FileHelper
    {



        /*public static string[] SearchByName(string FileName)
        {

        }

        public static string[] SearchByType(FileTypes type)
        {

        }
        */
        public static int CountFiles(FileTypes type = FileTypes.All)
        {
            int resoult = 0;


            return resoult;
        }

        public static string FilesSize(FileTypes type = FileTypes.All)
        {
            StringBuilder resoult = new StringBuilder();

            return resoult.ToString();
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
