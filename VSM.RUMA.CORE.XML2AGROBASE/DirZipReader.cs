using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ionic.Zip;

namespace VSM.RUMA.CORE.XML2AGROBASE
{
    [Serializable]
    class DirZipReader : IReaderPlugin
    {
        public String GetFilter()
        {
            return "*.DIRZIP*";
        }

        public List<String> getExcludeList()
        {
            List<String> Excludes = new List<String>();
            Excludes.Add(".TMP");
            Excludes.Add(".ZIP");
            Excludes.Add(".ZIP");
            return Excludes;
        }

        public int LeesFile(int thrId, DBConnectionToken agroFactuurToken, int programId, String agrobaseUser,
                                    String agrobasePassword, int fileLogId, String Bestandsnaam)
        {
            try
            {
                unLogger.WriteDebug("XML2AGROBASE.DirZipReader Username : " + agrobaseUser);
                //unLogger.WriteDebug("XML2AGROBASE.DirZipReader Password : " + agrobasePassword);
                unLogger.WriteDebug("DirZipReader HostName : " + unRechten.getDBHost());

                String archiefnaam = Bestandsnaam;
                String ext = "";
                if (!Bestandsnaam.ToUpper().EndsWith(GetFilter().Replace("*", "").ToUpper()))
                    ext = Path.GetExtension(Bestandsnaam);

                unLogger.WriteDebug("DirZipReader openen bestand : " + Path.GetFileName(Bestandsnaam));

                ZipFile dzip = new ZipFile(Bestandsnaam);
                var FilesinZip = dzip.EntryFileNames;
                unLogger.WriteInfo(String.Format("DirZipReader zipbestand bevat {0} bestanden ", FilesinZip.Count));
                bool IsUniformZip = false;
                foreach (String ZippedFile in FilesinZip)
                {
                    unLogger.WriteDebug(String.Format("DirZipReader bestand in zip : {1} zipbestand {0}", Path.GetFileName(Bestandsnaam), ZippedFile));
                    if (ZippedFile.ToUpper().Contains(".FDB"))
                    {
                        unLogger.WriteDebug("DirZipReader Bestand is een firebird database");
                        IsUniformZip = true;
                    }
                }

                String Dest;
                if (FilesinZip.Count == 1 || IsUniformZip)
                {
                    unLogger.WriteInfo("DirZipReader bestand uitpakken [UNIFORM BACKUP]");
                    Dest = Path.GetDirectoryName(Bestandsnaam) + Path.DirectorySeparatorChar;
                    dzip.ExtractProgress += dzip_ExtractProgress;
                    dzip.ExtractExistingFile = ExtractExistingFileAction.InvokeExtractProgressEvent;


                    String DestFilename;
                    foreach (ZipEntry e in dzip)
                    {
                        DestFilename = String.Format("{0}{1}{2}", Dest, Path.GetFileNameWithoutExtension(Bestandsnaam), e.FileName);
                        if (DestFilename.Length >= 260 || File.Exists(DestFilename))
                            e.Extract(Path.GetDirectoryName(Bestandsnaam), ExtractExistingFileAction.InvokeExtractProgressEvent);
                        else
                        {
                            using (StreamWriter sr = new StreamWriter(DestFilename))
                            {
                                e.Extract(sr.BaseStream);
                            }
                        }
                    }
                    return 1;
                }
                else
                {
                    unLogger.WriteInfo("DirZipReader bestand inlezen met XML Import [AGROVISION BACKUP]");
                    String File = System.IO.Path.GetFileNameWithoutExtension(Bestandsnaam);
                    String LogFile = unLogger.getLogDir("XML") + "DIRZIP#" + File + "#" + DateTime.Now.Ticks + ".log";
                    Win32PDA2Agrobase PDA2Agrobase = new Win32PDA2Agrobase();
                    int Result = PDA2Agrobase.importXMLMySQL(programId,
                            agrobaseUser,
                            agrobasePassword,
                            LogFile, unRechten.getDBHost(), Bestandsnaam, fileLogId);

                    unLogger.WriteDebug("XML2AGROBASE.DirZipReader           : " + Bestandsnaam + "result: " + Result.ToString());
                    if (Result > 0)
                    {
                        unLogger.WriteDebug("XML2AGROBASE.DirZipReader         :" + Bestandsnaam + " ingelezen");
                        return 1;
                    }
                    else
                    {
                        unLogger.WriteDebug("XML2AGROBASE.DirZipReader         :" + Bestandsnaam + " niet ingelezen");
                        return -1;
                    }
                }

            }
            catch (Exception ex)
            {
                unLogger.WriteInfo("DirZipReader onbekende fout bij het verwerken bestand");
                unLogger.WriteError(ex.Message, ex);
                return -2;
            }
        }

        void dzip_ExtractProgress(object sender, ExtractProgressEventArgs e)
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

                    unLogger.WriteDebugFormat("UNZIP", "ArchiveName {0} FileName {1} Extract as {2}", Path.GetFileName(e.ArchiveName), e.CurrentEntry.FileName, Filename);
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
