using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using Ionic.Zip;

namespace VSM.RUMA.CORE.XML2AGROBASE
{
    [Serializable]
    class ZipReader : IReaderPlugin
    {
        public String GetFilter()
        {
            return "*.ZIP*";
        }

        public List<String> getExcludeList()
        {
            List<String> Excludes = new List<String>();
            Excludes.Add(".TMP");
            return Excludes;
        }

        public int LeesFile(int thrId, DBConnectionToken agroFactuurToken, int programId, String agrobaseUser,
                                    String agrobasePassword, int fileLogId, String Bestandsnaam)
        {
            try
            {
                String archiefnaam = Bestandsnaam;
                String ext = "";
                if (!Bestandsnaam.ToUpper().EndsWith(GetFilter().Replace("*", "").ToUpper()))
                    ext = Path.GetExtension(Bestandsnaam);

                while (archiefnaam.Contains('.'))
                {
                    archiefnaam = Path.GetFileNameWithoutExtension(archiefnaam);
                }
                ZipFile ZFile = new ZipFile(Bestandsnaam);
                ZFile.ExtractProgress += ZFile_ExtractProgress;
                ZFile.ExtractExistingFile = ExtractExistingFileAction.InvokeExtractProgressEvent;
                ZFile.ExtractAll(Path.GetDirectoryName(Bestandsnaam), ExtractExistingFileAction.InvokeExtractProgressEvent);
                if (ZFile.Any()) return 1;
                return -1;
            }
            catch (Exception ex)
            {
                unLogger.WriteError("ZipReader Error: " + ex.Message, ex);
                return -2;
            }
        }

        void ZFile_ExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            if (e.EventType == ZipProgressEventType.Extracting_ExtractEntryWouldOverwrite)
            {
                if (!e.CurrentEntry.IsDirectory)
                {
                    unLogger.WriteDebugFormat("UNZIP", "ArchiveName {0} FileName {1} already Exists ", Path.GetFileName(e.ArchiveName), Path.GetFileName(e.CurrentEntry.FileName));
                    String Filename = e.ExtractLocation + Path.DirectorySeparatorChar + e.CurrentEntry.FileName;
                    int i = 0;
                    while (File.Exists(Filename))
                    {
                        i++;
                        Filename = String.Format("{0}-{1}{2}", e.ExtractLocation + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(e.CurrentEntry.FileName), i.ToString().PadLeft(3, '0'), Path.GetExtension(e.CurrentEntry.FileName));
                    }

                    unLogger.WriteDebugFormat("UNZIP", "ArchiveName {0} FileName {1} Extract as {2}", Path.GetFileName(e.ArchiveName), e.CurrentEntry.FileName,Filename);
                    using (StreamWriter s = new StreamWriter(Filename, false))
                    {
                        e.CurrentEntry.Extract(s.BaseStream);
                    }
                    e.CurrentEntry.ExtractExistingFile = ExtractExistingFileAction.DoNotOverwrite;
                }
                else unLogger.WriteDebugFormat("UNZIP", "ArchiveName {0} Directory {1} already Exists ", e.ArchiveName, e.CurrentEntry.FileName);

            }
        }

        public void setSaveToDB(AFSavetoDB value)
        {
        }
    }
}
