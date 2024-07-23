using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SopracoFileReader
{

    class FileReaderQueueItemComparer : IEqualityComparer<FileReaderQueueItem>
    {
        public bool Equals(FileReaderQueueItem x, FileReaderQueueItem y)
        {
            return x.File == y.File && x.FileWatcherInfo.Filter == y.FileWatcherInfo.Filter && x.FileWatcherInfo.Folder == y.FileWatcherInfo.Folder;
        }

        public int GetHashCode(FileReaderQueueItem obj)
        {
            return obj.File.GetHashCode() ^ obj.FileWatcherInfo.Filter.GetHashCode() ^ obj.FileWatcherInfo.Folder.GetHashCode();
        }
    }


    class FileReaderQueueItem
    {
        private string mFilename;
        private FileWatchData mFWD;
        private int programid;

        public int ProgramId
        {
            get
            {
                return programid;
            }
        }

        public string File
        {
            get
            {
                return mFilename;
            }
        }


        public FileWatchData FileWatcherInfo
        {
            get
            {
                return mFWD;
            }
        }

        public FileReaderQueueItem(String pFilename, String[] PathList, FileWatchData pFWD)
        {
            mFilename = pFilename;
            mFWD = pFWD;
            // probeer een programid te bepalen
            switch (PathList[0].ToLower())
            {
                case "agrobase":
                    switch (PathList[1].ToLower())
                    {
                        case "agrobase_t4c3":
                            programid = 2;         // lely robot
                            break;
                        case "agrobase_vc5":
                            programid = 53;         // progid 1,4
                            break;
                        case "processserver":
                            programid = 1;

                            if (pFilename.ToLower().Contains("lely"))
                            {
                                programid = 53;
                            }
                            else if (pFilename.ToLower().Contains("veecode"))
                            {
                                programid = 53;
                            }
                            else if (pFilename.ToLower().Contains("id2000"))
                            {
                                programid = 4200;
                            }
                            else if (pFilename.ToLower().Contains("fullwood"))
                            {
                                programid = 3500;
                            }
                            else if (pFilename.ToLower().Contains("crystal"))
                            {
                                programid = 3500;
                            }
                            else if (pFilename.ToLower().Contains("gea"))
                            {
                                programid = 3600;
                            }
                            else if (pFilename.ToLower().Contains("dairyplan"))
                            {
                                programid = 3600;
                            }
                            else if (pFilename.ToLower().Contains("delaval"))
                            {
                                programid = 3700;
                            }
                            else if (pFilename.ToLower().Contains("delpro"))
                            {
                                programid = 3700;
                            }
                            else if (pFilename.ToLower().Contains("alpro"))
                            {
                                programid = 3700;
                            }
                            else if (pFilename.ToLower().Contains("velos"))
                            {
                                programid = 3800;
                            }
                            else if (pFilename.ToLower().Contains("sac"))
                            {
                                programid = 3900;
                            }
                            else if (pFilename.ToLower().Contains("dairymaster"))
                            {
                                programid = 4000;
                            }
                            else if (pFilename.ToLower().Contains("boumatic"))
                            {
                                programid = 4100;
                            }
                            else if (pFilename.ToLower().Contains("bmi"))
                            {
                                programid = 4100;
                            }

                            // progid 1,4
                            break;
                        case "hetdeenssysteem":
                            programid = 13000;         // progid 1,4
                            break;
                        default:
                            programid = 1;         // progid 1,4
                            break;
                    }
                    break;
                case "agrobase_calf":
                    switch (PathList[1].ToLower())
                    {

                        case "sopraco":
                            programid = 8;   // progid 2,6,7 
                            break;
                        case "forfarmers":
                            programid = 9;   // progid 2,6,7 
                            break;
                        default:
                            programid = 5;   // progid 2,6,7 
                            break;
                    }
                    break;
                case "agrobase_sheep":
                    switch (PathList[1].ToLower())
                    {
                        case "nsfo_schotland":
                            programid = 49;   // progid 2,6,7 
                            break;
                        case "nsfo_lelystad_cvi":
                            programid = 49;   // progid 2,6,7 
                            break;
                        default:
                            programid = 6;  // progid 3
                            break;
                    }
                    break;
                case "agrobase_goat":
                    switch (PathList[1].ToLower())
                    {
                        default:
                            programid = 7;  // progid 5

                            break;
                    }
                    break;
                default:
                    programid = 0;
                    break;
            }
        }


        public override bool Equals(object obj)
        {
            if (obj is FileReaderQueueItem)
            {
                return (obj as FileReaderQueueItem).mFilename == mFilename && (obj as FileReaderQueueItem).mFWD.Filter == mFWD.Filter && (obj as FileReaderQueueItem).mFWD.Folder == mFWD.Folder;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return this.File.GetHashCode() ^ this.FileWatcherInfo.Filter.GetHashCode() ^ this.FileWatcherInfo.Folder.GetHashCode();
        }
    }
}

