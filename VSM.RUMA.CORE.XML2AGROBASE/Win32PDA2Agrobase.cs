using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.IO;
using VSM.RUMA.CORE;
using System.Xml;

namespace VSM.RUMA.CORE.XML2AGROBASE
{
    public class Win32PDA2Agrobase : Win32
    {

        public Win32PDA2Agrobase()
            : base(false)
        {
        }

        static readonly object padlck = new object();

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int importPDAXMLMySQL(int pProgramid, String pUserName, String pPassword, String pLogFile,
                                                        String pHostname, String pFilename, int pFilelogID);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int importXMLMySQL(int pProgramid, String pUserName, String pPassword, String pLogFile,
                                                       String pHostname, String pFilename, int pFilelogID)
        {
            int resultcode = -1;

            //LockObject padlck;
            //importPDAXMLMySQL handle = (importPDAXMLMySQL)ExecuteProcedureDLLStack(typeof(importPDAXMLMySQL), "importPDAXMLMySQL", GetLockList(), out padlock);
            String DLL = "PDA2RUMA.DLL";
            importPDAXMLMySQL handle = (importPDAXMLMySQL)ExecuteProcedureDLL(typeof(importPDAXMLMySQL), DLL, "importPDAXMLMySQL");
            try
            {
                System.Threading.Monitor.Enter(padlck);
                resultcode = handle(pProgramid, pUserName, pPassword, pLogFile, pHostname, pFilename, pFilelogID);
                FreeDLL(DLL);
            }
            catch (Exception ex)
            {
                unLogger.WriteError(ex.Message, ex);
                resultcode = -1;
            }
            finally
            {
                System.Threading.Monitor.Exit(padlck);

            }

            return resultcode;
        }

        public int importXMLMySQLxml(int pProgramid, String pUserName, String pPassword, String pLogFile,
                                               String pHostname, String pFilename,int pFilelogID)
        {
            int Result;
            String FormatProcess = "xml.exe";
            String OutputFilename = pFilename;
            if (System.IO.File.Exists(FormatProcess))
            {

                System.Diagnostics.Process p = new System.Diagnostics.Process();
                try
                {
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.FileName = "cmd.exe";//FormatProcess;
                    unLogger.WriteDebug("CurrentDirectory : " + System.IO.Directory.GetCurrentDirectory());
                    unLogger.WriteDebug("BaseDirectory    : " + AppDomain.CurrentDomain.BaseDirectory);
                    unLogger.WriteDebug("ProgramId        : " + pProgramid.ToString());
                    p.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    StringBuilder SBArgs = new StringBuilder();
                    SBArgs.AppendFormat("/C xml.exe fo \"{0}\"", pFilename);
                    
                    if (Path.GetExtension(pFilename).ToUpperInvariant().Equals(".XML"))
                        OutputFilename = Path.ChangeExtension(pFilename, ".FXML");
                    else
                        OutputFilename = pFilename + ".FXML";

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
                    if (p.ExitCode != 0) return -1;
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
                importXMLMySQL(pProgramid, pUserName, pPassword, pLogFile, pHostname, OutputFilename, pFilelogID);
            

            if (Result > 0)
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
