using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VSM.RUMA.CORE.DB.DataTypes;
using VSM.RUMA.CORE.EDINRS2AGROBASE;

//TODO - class renamen naar slachtbestandImport

namespace VSM.RUMA.CORE
{
    public class Edinrs
    {
        public bool LeesDirectory(int pProgramID, String Directory, UserRightsToken pToken)
        {
            try
            {
                string logDir = unLogger.getLogDir();
                string logfilepath = logDir + DateTime.Now.ToString("yyyyMMdd_HHmmss_") + "slachtbestandImport.txt";

                Win32slachtbestandImport.pCallback pCallback = new Win32slachtbestandImport.pCallback(pCallbackmethod);

                Win32slachtbestandImport.Init().call_lees_AB_SlachtDirectory(pProgramID,
                                                          pToken.Host, pToken.User, pToken.Password,
                                                          Directory, logfilepath,
                                                          pCallback, null, 
                                                          0, 0);

                unLogger.WriteDebug("EDINRS2AGROBASE.Reader :" + Directory + " ingelezen");
                return true;
            }
            catch (Exception ex)
            {
                unLogger.WriteError(ex.Message, ex);
                return false;
            }
        }

        public bool LeesBBS(int pProgramID, int pProgId, String pUbn, UserRightsToken pToken)
        {
            try
            {
                bbs_leesSlachtBestanden(pUbn, pProgId, pProgramID, pToken);

                return true;
            }
            catch (Exception ex) { unLogger.WriteError(ex.Message); }

            return false;
        }

        public bool LeesBBSByProgId(int pProgramID, int pProgId, UserRightsToken pToken)
        {
            try
            {
                List<UBN> ubnList = Facade.GetInstance().getSaveToDB(pToken).getUBNsByProgId(pProgId);

                foreach (UBN ubn in ubnList)
                {
                    string sUbn = ubn.Bedrijfsnummer;
                    LeesBBS(pProgramID, pProgId, sUbn, pToken);
                }

                return true;
            }
            catch (Exception ex) { unLogger.WriteError(ex.Message); }

            return false;
        }

        private void bbs_leesSlachtBestanden(String pUbn, int pProgId, int pProgramId, UserRightsToken pToken)
        {
            int afterTransferLocal = 1;  //delete lokale files
            int afterTransferRemote = 4; //verplaats ftp files naar opslag map

            string localfilepath = Facade.GetInstance().getDatacomPath() + "datacom\\in\\";
            
            string logDir = unLogger.getLogDir();
            string logBasis = logDir + DateTime.Now.ToString("yyyyMMdd_HHmmss_") + "edinrs_";

            //verkrijg postbusnummer                
            string bbsCode = Facade.GetInstance().getRechten().soap_geefPostbusnummerVanUBN(pUbn);

            if (bbsCode == string.Empty) 
            {
                return;
            }
            
            string remotefilePath = "/" + bbsCode + "/UIT/";
            string remotefilePathTo = "/" + bbsCode + "/OPSLAG/";
            
            FTPINFO ftpInf = Facade.GetInstance().getRechten().getFTPInfoBBS(pProgId);
            ftpInf.DirectoryTo = localfilepath;
            ftpInf.DirectoryFrom = remotefilePath;

            //kopieer bestanden van bbs-postbus naar datacom\in en verwerk ze
            if (ftpInf.FtpHostName != string.Empty)
            {           
                FTP ftpcon = new FTP(ftpInf.FtpHostName, ftpInf.UserName, ftpInf.Password);

                string[] fileList = ftpcon.GetFileList(remotefilePath);
                for (int i = 0; i < fileList.Count(); i++)
                {
                    try
                    {
                        string fileName = fileList[i];
                        string localFile = localfilepath + fileName;
                        string remoteFile = remotefilePath + fileName;
                        string logFile = logBasis + fileName + ".txt";

                        ftpcon.Download(localfilepath, remotefilePath, fileName, 0);
                        if (File.Exists(localFile) == true)
                        {
                            int res = Win32slachtbestandImport.Init().call_lees_AB_SlachtBestand(
                                    pProgramId, pToken.Host, pToken.User, pToken.Password,
                                    localFile, logFile, pCallbackmethod, 0, 0, 0);

                            if (res == 1)
                            {
                                //indien dll succesvol uitgevoerd; aftertransfer actie op de ftp
                                ftpcon.AfterTransferAction_FTP(afterTransferRemote, fileName, remotefilePath, remotefilePathTo);
                            }
                            
                            //aftertransfer actie lokaal
                            FileInfo fileInfLocal = new FileInfo(localFile);
                            ftpcon.AfterTransferAction(afterTransferLocal, fileInfLocal);                                                               
                        }
                    }
                    catch (Exception ex)
                    {
                        unLogger.WriteError(ex.Message);
                    }
                }
            }    
        }

        private void pCallbackmethod(int PercDone, string Msg)
        {
            //unLogger.WriteInfo("callback edi_AB_SlachtBestand: " + PercDone.ToString() + " :" + Msg);
        }

    }
}
