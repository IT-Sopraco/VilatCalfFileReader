using System;
using System.Data;
using System.Text;
using System.Configuration;
using System.Web;
using System.Net;
using System.IO;

namespace VSM.RUMA.CORE
{
    /// <summary>
    /// Summary description for ftpclass
    /// </summary>
    public class FTP
    {
        private string ftpServerIP;
        private string ftpUserID;
        private string ftpPassword;
        public string Errorstring;
        private bool ftpPassive = true;

        public FTP(string pServerIP, string pUserID, string pPassword)
        {
            ftpServerIP = pServerIP;
            ftpUserID = pUserID;
            ftpPassword = pPassword;
            Errorstring = "";
        }

        public FTP(string pServerIP, string pUserID, string pPassword, bool pPassive)
            : this(pServerIP, pUserID, pPassword)
        {
            ftpPassive = pPassive;
        }

        public void UploadThrowsExceptions(string localfilePat, string remotefilePath, string filename, int AfterTransfer, bool overwrite)
        {
            FileInfo fileInfLoc = new FileInfo(filename);
            FileInfo fileInfRem = new FileInfo(filename);
            //   string uri = "ftp://" +
            //ftpServerIP + "/" + fileInf.Name;
            int add = 1;
            while (FileExists(remotefilePath.Replace('\\', '/') + "/" + fileInfRem.Name) && !overwrite)
            {
                fileInfRem = new FileInfo(String.Format(filename + ".{0}", add));
                add++;
            }

            FtpWebRequest reqFTP;


            unLogger.WriteDebug("URL : ftp://" + ftpServerIP + remotefilePath.Replace('\\', '/') + "/" + fileInfRem.Name);
            // Create FtpWebRequest object from the Uri provided
            reqFTP =
         (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + remotefilePath.Replace('\\', '/') + "/" + Path.ChangeExtension(fileInfRem.Name, "TMP")));

            // Provide the WebPermission Credintials
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);

            // By default KeepAlive is true, where the control connection is not closed
            // after a command is executed.
            reqFTP.KeepAlive = false;

            // Specify the command to be executed.
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

            // Specify the data transfer type.
            reqFTP.UseBinary = true;
            reqFTP.UsePassive = ftpPassive;

            // Notify the server about the size of the uploaded file
            reqFTP.ContentLength = fileInfLoc.Length;

            // The buffer size is set to 2kb
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;

            // Opens a file stream (System.IO.FileStream) to read the file to be uploaded
            FileStream fs = fileInfLoc.OpenRead();


            // Stream to which the file to be upload is written
            Stream strm = reqFTP.GetRequestStream();

            // Read from the file stream 2kb at a time
            contentLen = fs.Read(buff, 0, buffLength);

            // Till Stream content ends
            while (contentLen != 0)
            {
                // Write Content from the file stream to the FTP Upload Stream
                strm.Write(buff, 0, contentLen);
                contentLen = fs.Read(buff, 0, buffLength);
            }

            // Close the file stream and the Request Stream
            strm.Close();
            fs.Close();
            Rename(Path.ChangeExtension(remotefilePath.Replace('\\', '/') + "/" + fileInfRem.Name, "TMP"), remotefilePath.Replace('\\', '/') + "/" + fileInfRem.Name);
            AfterTransferAction(AfterTransfer, fileInfLoc);

        }

        public void UploadThrowsExceptionsWithNewRemoteName(string localfilePathInclName, string remotefilePath, string remoteFileName, 
            int AfterTransfer, bool overwrite) 
        {
            string origRemoteFileName = remoteFileName;
            FileInfo fileInfLoc = new FileInfo(localfilePathInclName);

            int add = 1;
            while (FileExists(remotefilePath.Replace('\\', '/') + "/" + remoteFileName) && !overwrite)
            {
                remoteFileName = String.Format(origRemoteFileName + ".{0}", add);
                add++;
            }

            FtpWebRequest reqFTP;

            unLogger.WriteDebug("URL : ftp://" + ftpServerIP + remotefilePath.Replace('\\', '/') + "/" + remoteFileName);
            // Create FtpWebRequest object from the Uri provided
            reqFTP =
         (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + remotefilePath.Replace('\\', '/') + "/" + Path.ChangeExtension(remoteFileName, "TMP")));

            // Provide the WebPermission Credintials
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);

            // By default KeepAlive is true, where the control connection is not closed
            // after a command is executed.
            reqFTP.KeepAlive = false;

            // Specify the command to be executed.
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

            // Specify the data transfer type.
            reqFTP.UseBinary = true;
            reqFTP.UsePassive = ftpPassive;

            // Notify the server about the size of the uploaded file
            reqFTP.ContentLength = fileInfLoc.Length;

            // The buffer size is set to 2kb
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;

            // Opens a file stream (System.IO.FileStream) to read the file to be uploaded
            FileStream fs = fileInfLoc.OpenRead();

            // Stream to which the file to be upload is written
            Stream strm = reqFTP.GetRequestStream();

            // Read from the file stream 2kb at a time
            contentLen = fs.Read(buff, 0, buffLength);

            // Till Stream content ends
            while (contentLen != 0)
            {
                // Write Content from the file stream to the FTP Upload Stream
                strm.Write(buff, 0, contentLen);
                contentLen = fs.Read(buff, 0, buffLength);
            }

            // Close the file stream and the Request Stream
            strm.Close();
            fs.Close();
            Rename(Path.ChangeExtension(remotefilePath.Replace('\\', '/') + "/" + remoteFileName, "TMP"), remotefilePath.Replace('\\', '/') + "/" + remoteFileName);
            AfterTransferAction(AfterTransfer, fileInfLoc);
        }

        public bool Upload(string localfilePat, string remotefilePath, string filename, int AfterTransfer, bool overwrite)
        {
            try
            {
                UploadThrowsExceptions(localfilePat, remotefilePath, filename, AfterTransfer, overwrite);
                return true;
            }
            catch (Exception ex)
            {
                Errorstring = "Upload Error:" + ex.Message;
                unLogger.WriteInfo(Errorstring);
                return false;
            }
        }

        public void AfterTransferAction_FTP(int AfterTransfer, string fileName, string remotefilePath, string remotefilePathTo)
        {
            switch (AfterTransfer)
            {
                case 1:
                    //verwijder bestand
                    Delete(remotefilePath, fileName);
                    break;
                case 4:
                    //verplaats bestand naar opslag map
                    Rename(fileName, remotefilePath, remotefilePathTo);
                    break;
            }
        }

        public void AfterTransferAction(int AfterTransfer, FileInfo fileInf)
        {
            try
            {
                switch (AfterTransfer)
                {
                    case 1:
                        fileInf.Delete();
                        break;
                    case 4:
                        if (!Directory.Exists(unDatacomReader.DatacomPath + "Datacom\\Opslag\\"))
                            Directory.CreateDirectory(unDatacomReader.DatacomPath + "Datacom\\Opslag\\");

                        int add = 1;
                        String Filename = fileInf.Name;
                        while (File.Exists(unDatacomReader.DatacomPath + "Datacom\\Opslag\\" + Filename))
                        {
                            Filename = String.Format(fileInf.Name + ".{0}", add);
                            add++;
                        }
                        fileInf.MoveTo(unDatacomReader.DatacomPath + "Datacom\\Opslag\\" + Filename);
                        break;
                }
            }
            catch (Exception ex)
            {
                unLogger.WriteError(ex.Message);
            }
        }

        public bool Download(string localfilePath, string remotefilePath, string fileName, int AfterTransfer)
        {

            try
            {
                DownloadThrowsExceptions(localfilePath, remotefilePath, fileName, AfterTransfer);
                return true;
            }
            catch (Exception ex)
            {
                Errorstring = "Download Error:" + ex.Message;
                unLogger.WriteInfo(Errorstring);
                return false;
            }
        }

        public void DownloadThrowsExceptions(string localfilePath, string remotefilePath, string fileName, int AfterTransfer)
        {
            FtpWebRequest reqFTP;
            //filePath = <<The full path where the file is to be created. the>>, 
            //fileName = <<Name of the file to be createdNeed not name on FTP server. name name()>>
            FileStream outputStream = new FileStream(localfilePath + "\\" + fileName, FileMode.Create);

            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" +
                                                         remotefilePath.Replace('\\', '/') + "/" + fileName));

            reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
            reqFTP.Timeout = -1;
            reqFTP.UseBinary = true;
            reqFTP.UsePassive = ftpPassive;
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
            Stream ftpStream = response.GetResponseStream();
            long cl = response.ContentLength;
            int bufferSize = 2048;
            int readCount;
            byte[] buffer = new byte[bufferSize];

            readCount = ftpStream.Read(buffer, 0, bufferSize);
            while (readCount > 0)
            {
                outputStream.Write(buffer, 0, readCount);
                readCount = ftpStream.Read(buffer, 0, bufferSize);
            }

            ftpStream.Close();
            outputStream.Close();
            response.Close();

            if (!configHelper.UseCRDTestserver)
            {
                FileInfo fileInf = new FileInfo(remotefilePath);
                AfterTransferAction_FTP(AfterTransfer, fileName, remotefilePath, remotefilePath);
            }
        }

        public void DownloadFromDir(string localfilePath, string remotefilePath, int AfterTransfer)
        {
            string[] Files = GetFileList(remotefilePath);
            if (Files != null)
            {
                foreach (string Filename in Files)
                {
                    unLogger.WriteDebug(String.Format("DownloadFromDir: Downloaden bestand {0}", Filename));
                    try
                    {
                        Download(localfilePath, remotefilePath, Filename, AfterTransfer);
                    }
                    catch (Exception ex)
                    {
                        unLogger.WriteError(String.Format("Download of {0} Failed... ", Filename), ex);
                    }
                }
            }
        }

        public void UploadFilestoDir(string localfilePath, string remotefilePath, string Filter, int AfterTransfer)
        {
            string[] Files = Directory.GetFiles(localfilePath, Filter);
            foreach (string Filename in Files)
            {
                try
                {

                    Upload(localfilePath, remotefilePath, Filename, AfterTransfer, false);
                }
                catch (Exception ex)
                {
                    unLogger.WriteError(String.Format("Upload of {0} Failed... ", Filename), ex);
                }
            }
        }

        public void UploadFilestoDirThrowsExceptions(string localfilePath, string remotefilePath, string Filter, int AfterTransfer)
        {
            string[] Files = Directory.GetFiles(localfilePath, Filter);
            foreach (string Filename in Files)
            {
                Upload(localfilePath, remotefilePath, Filename, AfterTransfer, false);
            }
        }

        public void UploadtoDir(string localfilePath, string remotefilePath, int AfterTransfer)
        {
            string[] Files = Directory.GetFiles(localfilePath, "*.*");
            foreach (string Filename in Files)
            {
                try
                {

                    Upload(localfilePath, remotefilePath, Filename, AfterTransfer, false);
                }
                catch (Exception ex)
                {
                    unLogger.WriteError(String.Format("Upload of {0} Failed... ", Filename), ex);
                }
            }
        }

        public void Delete(string remotefilePath, string fileName)
        {
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new
                Uri("ftp://" + ftpServerIP + "/" + remotefilePath.Replace('\\', '/') + "/" + fileName));
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = ftpPassive;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                //response.
                response.Close();
            }
            catch (Exception ex)
            {
                Errorstring = "Download Error:" + ex.Message;
                unLogger.WriteInfo(Errorstring);
            }

        }

        public void Rename(string fileName, string remotefilePathFrom, string remotefilePathTo)
        {
            string from = remotefilePathFrom + fileName;
            string to = remotefilePathTo + fileName;
            Rename(from, to);
        }

        public void Rename(string FullfileNameFrom, string FullfileNameTo)
        {


            try
            {
                string FileUri = "ftp://" + ftpServerIP + FullfileNameFrom;

                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)WebRequest.Create(new Uri(FileUri));
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.Rename;
                reqFTP.RenameTo = FullfileNameTo;
                reqFTP.KeepAlive = false;
                reqFTP.UsePassive = ftpPassive;

                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();

                unLogger.WriteDebug(("Rename response status:" + response.StatusDescription));
                response.Close();
            }
            catch (Exception ex)
            {
                Errorstring = "Rename Error: from " + FullfileNameFrom + " to " + FullfileNameTo + " - ERROR: " + ex.Message;
                unLogger.WriteError(Errorstring);
            }
        }

        public bool FileExists(string FullfileName)
        {
            try
            {
                string FileUri = "ftp://" + ftpServerIP + FullfileName;

                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)WebRequest.Create(new Uri(FileUri));
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;
                reqFTP.KeepAlive = false;
                reqFTP.UsePassive = ftpPassive;

                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();

                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    unLogger.WriteDebug($"return FALSE - Exists response status: {response.StatusDescription}");
                    response.Close();
                    return false;
                }
                else
                {

                    unLogger.WriteDebug($"return TRUE - Exists response status: {response.StatusDescription}");
                    response.Close();
                    return true;
                }
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    unLogger.WriteDebug($"return FALSE - Exists response status: {response.StatusDescription} Ex: {ex.Message}", ex);
                    response.Close();
                    return false;
                }
                else
                {
                    unLogger.WriteDebug($"return TRUE - Exists response status: {response.StatusDescription} Ex: {ex.Message}", ex);
                    response.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Errorstring = "Rename Error: from " + FullfileName + " to " + FullfileName + " - ERROR: " + ex.Message;
                unLogger.WriteError(Errorstring, ex);
                return false;

            }
        }

        public string[] GetFileList()
        {
            return GetFileList(string.Empty);
        }

        public string[] GetFileList(string remotefilePath)
        {
            string[] downloadFiles;
            StringBuilder result = new StringBuilder();
            try
            {
                string ftpfilepath = remotefilePath.Replace('\\', '/');
                if (ftpfilepath[0] != '/' && !ftpServerIP.EndsWith("/"))
                {
                    ftpfilepath = "/" + ftpfilepath;
                }

                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + ftpfilepath + "/"));
                reqFTP.Timeout = -1;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = ftpPassive;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;

                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());

                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                // to remove the trailing '\n'
                int idx = result.ToString().LastIndexOf('\n');
                if (idx >= 0)
                    result.Remove(idx, 1);

                reader.Close();
                response.Close();
                return result.ToString().Split('\n');
            }
            catch (Exception exc)
            {
                Errorstring = "GetFileList Error:" + exc.Message;
                unLogger.WriteInfo(Errorstring);
                downloadFiles = null;
                return downloadFiles;
            }

        }
    }
}
