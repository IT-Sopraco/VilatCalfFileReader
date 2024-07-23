using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class unDatacomReader
    {
        private static string lDatacomPath = "C:\\";

        public static string DatacomPath
        {
            get
            {
                return lDatacomPath;
            }
            set
            {
                lDatacomPath = value;
                lDatacomInDir = lDatacomPath + "Datacom\\In\\";
                lDatacomUitDir = lDatacomPath + "Datacom\\Uit\\";
            }
        }

        private static string lDatacomInDir = lDatacomPath + "Datacom\\In\\";

        public static string DatacomInDir
        {
            get
            {
                return lDatacomInDir;
            }
            set
            {
                lDatacomInDir = value;
            }
        }

        private static string lDatacomUitDir = lDatacomPath +  "Datacom\\Uit\\";

        public static string DatacomUitDir
        {
            get
            {
                return lDatacomUitDir;
            }
            set
            {
                lDatacomUitDir = value;
            }
        }


        public static List<String> GetFilesDatacomIn(String Extension)
        {
            List<String> Result = new List<String>();
            Result.AddRange(System.IO.Directory.GetFiles(lDatacomInDir, "*." + Extension));
            return Result;
        }

        public static List<String> GetFilesDatacomInDatabseMapnaam(String Extension,string pDatabaseName)
        {
            List<String> Result = new List<String>();
            Result.AddRange(System.IO.Directory.GetFiles(lDatacomInDir + pDatabaseName, "*." + Extension));
            return Result;
        }

        //public static bool RenameFileDatacomIn(String Filename)
        //{
        //    int nr = 1;
        //    while (File.Exists(lDatacomPath + lDatacomInDir + "edinrs.!0" + nr)) nr++;
        //    System.IO.File.Move(Filename, lDatacomPath + lDatacomInDir + "edinrs.!0" + nr);
        //    return true;
        //}

        public static bool RenameFileDatacomIn(String Filename,String Newname)
        {
            int nr = 1;
            while (File.Exists(lDatacomInDir +  Newname + ".!0" + nr)) nr++;
            System.IO.File.Move(Filename, lDatacomInDir +  Newname + ".!0" + nr);
            return true;
        }



        public static bool MoveFiletoDatacomUit(String FilePath)
        {
            String lFilename = Path.GetFileName(FilePath);
            unLogger.WriteDebug("Datacom uit : " + lDatacomUitDir);
            if (!File.Exists(lDatacomUitDir + lFilename))
                System.IO.File.Move(FilePath, lDatacomUitDir + lFilename);
            else return false; 
            return true;
        }
    }
}
