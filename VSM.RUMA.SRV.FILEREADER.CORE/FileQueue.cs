using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VSM.RUMA.SRV.FILEREADER.CORE
{
    class FileQueue : ConcurrentDictionary<FileReaderQueueItem, FileQueue.FileDetails>
    {

        public class FileDetails
        {

            public FileDetails(FileReaderQueueItem fileitem)
            {
                CheckTime = DateTime.Now;
                Filesize = FileQueue.CurrentFileSize(fileitem.File);
            }

            internal DateTime CheckTime
            {
                get;
                private set;
            }


            internal long? Filesize
            {
                get;
                private set;
            }

            public override bool Equals(object obj)
            {
                if (obj is FileDetails)
                {
                    return (obj as FileDetails).CheckTime == CheckTime && (obj as FileDetails).Filesize == Filesize;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return this.CheckTime.GetHashCode() ^ this.Filesize.GetHashCode();
            }
        }



        private static readonly object uniqueLock = new object();

        public bool TryEnqueue(FileReaderQueueItem item)
        {
            return TryAdd(item, new FileDetails(item));
        }


        public bool TryDequeue(out FileReaderQueueItem result)
        {
            lock (uniqueLock)
            {
                while (this.Count > 0)
                {
                    var enumitem = this.Where(kv => kv.Value.CheckTime < DateTime.Now.AddSeconds(-10)).OrderBy(kv => kv.Value.CheckTime).FirstOrDefault();
                    FileDetails datval;
                    if (enumitem.Equals(new KeyValuePair<FileReaderQueueItem, FileQueue.FileDetails>()))
                    {
                        //no elements in queue that are ready, sleep for 10 seconds
                        Thread.Sleep(10000);
                    }
                    else if (enumitem.Value.Filesize != CurrentFileSize(enumitem.Key.File))
                    {
                        VSM.RUMA.CORE.unLogger.WriteDebug("Item " + enumitem.Key.File + " Size " + enumitem.Value.Filesize);
                        TryUpdate(enumitem.Key, new FileDetails(enumitem.Key), enumitem.Value);
                    }
                    else if (TryRemove(enumitem.Key, out datval))
                    {
                        result = enumitem.Key;
                        return true;
                    }
                }
                result = null;
                return false;
            }

        }

        private static long? CurrentFileSize(String pPath)
        {
            if (File.Exists(pPath))
            {
                FileInfo fi = new FileInfo(pPath);
                return fi.Length;
            }
            return null;
        }
    }
}
