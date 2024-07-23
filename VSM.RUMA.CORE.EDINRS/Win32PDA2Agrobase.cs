using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.IO;
using VSM.RUMA.CORE;

namespace VSM.RUMA.CORE.EDINRS
{
    public class Win32PDA2Agrobase : Win32
    {

        public Win32PDA2Agrobase()
            : base(false)
        {
        }

        static readonly object padlck = new object();

        //private static LockObject[] LockList;


        //public static LockObject[] GetLockList()
        //{

        //    lock (padlck)
        //    {
        //        if (LockList == null)
        //        {
        //            List<LockObject> locklist = new List<LockObject>();
        //            FileInfo fileInf = new FileInfo(GetBaseDir() + "lib\\PDA2RUMA.DLL");
        //            int add = 1;
        //            while (fileInf.Exists || add == 1)
        //            {
        //                if (fileInf.Exists)
        //                    locklist.Add(new LockObject(fileInf.Name));
        //                fileInf = new FileInfo(String.Format(GetBaseDir() + "lib\\PDA2RUMA{0}.DLL", add));
        //                add++;
        //            }
        //            LockList = locklist.ToArray();
        //        }
        //        return LockList;
        //    }
        //}


        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int importPDAXMLMySQL(int pProgramid, String pUserName, String pPassword, String pLogFile,
                                                        String pHostname, String pFilename, int pFilelogID);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool importXMLMySQL(int pProgramid, String pUserName, String pPassword, String pLogFile,
                                                       String pHostname, String pFilename, int pFilelogID)
        {
            int dllcode = -1;
            //LockObject padlock;
            //importPDAXMLMySQL handle = (importPDAXMLMySQL)ExecuteProcedureDLLStack(typeof(importPDAXMLMySQL), "importPDAXMLMySQL", GetLockList(), out padlock);
            String DLL = "PDA2RUMA.DLL";
            importPDAXMLMySQL handle = (importPDAXMLMySQL)ExecuteProcedureDLL(typeof(importPDAXMLMySQL), DLL, "importPDAXMLMySQL");
            try
            {
                System.Threading.Monitor.Enter(padlck);
                dllcode = handle(pProgramid, pUserName, pPassword, pLogFile, pHostname, pFilename, pFilelogID);
                FreeDLL(DLL);
            }
            catch (Exception ex)
            {
                unLogger.WriteError(ex.Message, ex);
                dllcode = -1;
            }
            finally
            {
                System.Threading.Monitor.Exit(padlck);

            }

            return dllcode > 0;
        }

        //public bool importXMLMySQLexec(int pProgramid, String pUserName, String pPassword, String pLogFile,
        //                                       String pHostname, String pFilename)
        //{

        //    bool Result = false;
        //    String ChildProcess = "FilereaderChild.exe";
        //    if (System.IO.File.Exists(ChildProcess))
        //    {

        //        System.Diagnostics.Process p = new System.Diagnostics.Process();
        //        try
        //        {

        //            p.StartInfo.UseShellExecute = false;
        //            p.StartInfo.CreateNoWindow = true;
        //            p.StartInfo.FileName = ChildProcess;
        //            unLogger.WriteDebug("CurrentDirectory :" + System.IO.Directory.GetCurrentDirectory());
        //            unLogger.WriteDebug("BaseDirectory    : " + AppDomain.CurrentDomain.BaseDirectory);
        //            p.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
        //            StringBuilder SBArgs = new StringBuilder();
        //            SBArgs.AppendFormat(" -Programid \"{0}\" ", pProgramid);
        //            SBArgs.AppendFormat(" -UserName \"{0}\" ", pUserName);
        //            SBArgs.AppendFormat(" -Password \"{0}\" ", pPassword);
        //            SBArgs.AppendFormat(" -LogFile \"{0}\" ", pLogFile);
        //            SBArgs.AppendFormat(" -Hostname \"{0}\" ", pHostname);
        //            SBArgs.AppendFormat(" -Filename \"{0}\" ", pFilename);
        //            p.StartInfo.Arguments = SBArgs.ToString();
        //            p.Start();
        //            p.WaitForExit();
        //            if (p.ExitCode == 0) Result = true;
        //        }
        //        catch (Exception ex)
        //        {
        //            unLogger.WriteDebug(ex.Message, ex);

        //        }
        //        finally
        //        {
        //            if (!p.HasExited)
        //                p.Kill();
        //        }
        //    }
        //    else
        //    {
        //        unLogger.WriteInfo("FilereaderChild.exe not found!");
        //        //applicatie staat niet in de map, xml inlezen op de oude manier
        //        Result = importXMLMySQL(pProgramid, pUserName, pPassword, pLogFile, pHostname, pFilename);

        //    }
        //    return Result;
        //}


        public bool importXMLMySQLxml(int pProgramid, String pUserName, String pPassword, String pLogFile,
                                               String pHostname, String pFilename,int FileLogId)
        {

            bool Result = false;
            String FormatProcess = "xml.exe";
            String OutputFilename = pFilename;
            if (System.IO.File.Exists(FormatProcess))
            {

                System.Diagnostics.Process p = new System.Diagnostics.Process();
                try
                {
                    if (!utils.checkxmlfile(pFilename)) { return false; }
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.FileName = "cmd.exe";//FormatProcess;
                    unLogger.WriteDebug("CurrentDirectory :" + System.IO.Directory.GetCurrentDirectory());
                    unLogger.WriteDebug("BaseDirectory    : " + AppDomain.CurrentDomain.BaseDirectory);
                    p.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    StringBuilder SBArgs = new StringBuilder();
                    SBArgs.AppendFormat("/C xml.exe fo \"{0}\"", pFilename);
                    OutputFilename = Path.ChangeExtension(pFilename, ".FXML");
                    SBArgs.AppendFormat(" > \"{0}\"", OutputFilename);
                    //p.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(p_ErrorDataReceived);
                    //p.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(p_OutputDataReceived);
                    p.StartInfo.Arguments = SBArgs.ToString();
                    //p.StartInfo.RedirectStandardError = true;                    
                    //p.StartInfo.RedirectStandardOutput = true;
                    p.Start();
                    p.WaitForExit();
                    //unLogger.WriteInfo(p.StandardOutput.ReadToEnd());
                    //unLogger.WriteInfo(p.StandardError.ReadToEnd());                   
                    if (p.ExitCode == 0) Result = true;
                    else return false;
                }
                catch (Exception ex)
                {
                    unLogger.WriteDebug(ex.Message, ex);
                }
                finally
                {
                    if (!p.HasExited)
                        p.Kill();
                }
            }
            Result = //importXMLMySQLexec(pProgramid, pUserName, pPassword, pLogFile, pHostname, OutputFilename);
                importXMLMySQL(pProgramid, pUserName, pPassword, pLogFile, pHostname, OutputFilename, FileLogId);

            if (Result)
                DeleteTempFiles(OutputFilename);
            return Result;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void DeleteTempFiles(String OutputFilename)
        {
            try
            {
                File.Delete(OutputFilename);
            }
            catch (Exception ex)
            {
                unLogger.WriteDebug(ex.Message, ex);
            }
        }

        void p_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            unLogger.WriteInfo("Output From FileChild : " + e.Data);
        }

        void p_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            unLogger.WriteError("Error in FileChild : " + e.Data);
        }
    }
}
