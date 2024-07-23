using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.ComponentModel;
using VSM.RUMA.CORE.COMMONS;

namespace VSM.RUMA.CORE
{
    public abstract class Win32 : IDisposable
    {
        private const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;
        private const uint DONT_RESOLVE_DLL_REFERENCES = 0x00000001;
        private const uint LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008;
        private const uint LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010;
        public const uint ATTACH_PARENT_PROCESS = 0x0ffffffff;
        static readonly object padlock = new object();


        private Dictionary<String, IntPtr> mDLLPointers = new Dictionary<String, IntPtr>();

        private bool disposed = false;
        private bool mUseShareMem = false;

        public Win32()
        {
            SetDllDirectory(GetBaseDir() + "lib\\");
        }

        public Win32(bool UseShareMem)
        {
            SetDllDirectory(GetBaseDir() + "lib\\");
            if (UseShareMem) LoadLibraryRelative("lib\\borlndmm.dll");
            mUseShareMem = UseShareMem;
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr LoadLibrary(string fileName);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern IntPtr LoadLibraryEx(string fileName, IntPtr hFile, uint dwFlags);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern Boolean FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern bool SetDllDirectory(string lpPathName);


        protected int LoadLibraryRelative(String pFile)
        {
            int Result = 0;
            lock (padlock)
            {
                unLogger.WriteDebug("WIN32API", String.Format("[{0}] Win32: Current Directory : {1}", pFile, System.IO.Directory.GetCurrentDirectory()));
                if (!mDLLPointers.ContainsKey(pFile))
                {

                    unLogger.WriteDebug("WIN32API", String.Format("[{0}] LoadLibrary: Load from file: {1}", pFile, GetBaseDir() + pFile));
                    if (!File.Exists(GetBaseDir() + pFile))
                        unLogger.WriteError("WIN32API", String.Format("[{0}] LoadLibrary: File Does not Exist! : {1} ", pFile, GetBaseDir() + pFile));
                    IntPtr Libptr = IntPtr.Zero;
                    for (int recusivecounter = 0; recusivecounter < 10 && Libptr == IntPtr.Zero; recusivecounter++)
                    {
                        Libptr = LoadLibraryEx(GetBaseDir() + pFile, IntPtr.Zero, LOAD_WITH_ALTERED_SEARCH_PATH);

                        if (Libptr == IntPtr.Zero)
                        {
                            Result = Marshal.GetLastWin32Error();
                            Libptr = LoadLibrary(GetBaseDir() + pFile);
                            if (Libptr == IntPtr.Zero) Result = Marshal.GetLastWin32Error();
                        }
                    }
                    if (Libptr == IntPtr.Zero)
                    {
                        
                        unLogger.WriteError("WIN32API", String.Format("[{0}] LoadLibrary: Libptr is Zero!", pFile));
                        unLogger.WriteError("WIN32API", String.Format("[{0}] LoadLibrary: LastError #{1}", pFile, Result));
                        return Result;
                    }

                    mDLLPointers.Add(pFile, Libptr);
                }
                else unLogger.WriteWarn("WIN32API", String.Format("[{0}] LoadLibrary: File is already loaded.", pFile));
            }
            return 0;
        }
        protected bool FreeLibraryRelative(String pFile)
        {
            lock (padlock)
            {
                unLogger.WriteInfo("WIN32API", String.Format("[{0}] FreeLibraryRelative", pFile));
                if (mDLLPointers.ContainsKey(pFile))
                {
                    IntPtr Libptr = mDLLPointers[pFile];
                    for (int recusivecounter = 0; recusivecounter < 85000; recusivecounter++)
                    {
                        if (FreeLibrary(Libptr))
                            break;
                        Thread.Sleep(1000);
                    }
                    mDLLPointers.Remove(pFile);
                }
            }
            return true;
        }

        private Delegate ExecuteProcedureDLLRelative(Type t, String pFile, String pProcName)
        {
            Delegate Result = null;
            lock (padlock)
            {
                if (!mDLLPointers.ContainsKey(pFile))
                {
                    throw new InvalidOperationException("Library not Loaded");
                }
                IntPtr Proc = IntPtr.Zero;
                int i = 0;
                int j = 0;
                while (Proc == IntPtr.Zero && i < 100)
                {
                    try
                    {
                        unLogger.WriteDebug("WIN32API", String.Format("[{0}] GetProcAddress {1} ", pFile, pProcName));
                        Proc = GetProcAddress(mDLLPointers[pFile], pProcName);
                    }

                    catch (Exception ex)
                    {
                        unLogger.WriteError("WIN32API", String.Format("[{0}] GetProcAddress {1}  Win32 Error: {2}", pFile, pProcName, Marshal.GetLastWin32Error().ToString()));
                        unLogger.WriteError("WIN32API", ex.Message, ex);
                    }
                    finally
                    {
                        i++;
                    }
                }
                if (i >= 100)
                {
                    unLogger.WriteDebug("WIN32API", String.Format("[{0}] GetProcAddress {1} niet gevonden", pFile, pProcName));
                    throw new SystemException(pFile + " " + pProcName + " niet gevonden");
                }
                while (Result == null && j < 100)
                {
                    try
                    {
                        unLogger.WriteDebug("WIN32API", String.Format("[{0}] GetDelegateForFunctionPointer {1} ", pFile, pProcName));
                        Result = Marshal.GetDelegateForFunctionPointer(Proc, t);
                    }
                    catch (Exception ex)
                    {
                        unLogger.WriteError("WIN32API", String.Format("[{0}] GetDelegateForFunctionPointer {1} Fout: {2} ", pFile, pProcName, ex.Message), ex);
                    }
                    finally
                    {
                        j++;
                    }
                }
                if (j >= 100)
                {
                    unLogger.WriteDebug("WIN32API", String.Format("[{0}] Onjuiste DLL versie voor aanroep van proceduire {1} ", pFile, pProcName));
                    throw new NullReferenceException(pFile + " " + pProcName + " Onjuiste DLL versie");
                }
            }
            return Result;
        }

        protected Delegate ExecuteProcedureDLL(Type t, String pFilename, String pProcName)
        {

            lock (padlock)
            {
                int Result = LoadLibraryRelative("lib\\" + pFilename);
                if (Result == 0)
                {
                    Delegate Proc = ExecuteProcedureDLLRelative(t, "lib\\" + pFilename, pProcName);
                    return Proc;
                }
                else throw new SystemException(String.Format("Library {0} not Loaded", pFilename), new Win32Exception(Result));
                //else throw new SystemException(String.Format("Library {0} not Loaded",pFilename));

            }
        }


        protected Delegate ExecuteProcedureDLLStack(Type t, string pProcName, LockObject[] locklist, out LockObject currentlock)
        {
            for (int recusivecounter = 0; recusivecounter < 85000; recusivecounter++)
            {
                foreach (LockObject trylock in locklist)
                {
                    if (System.Threading.Monitor.TryEnter(trylock))
                    {
                        currentlock = trylock;
                        //unLogger.WriteInfo("WIN32API", String.Format("{1} Using File {0}", trylock.DLLname, pProcName));
                        Delegate result = ExecuteProcedureDLL(t, trylock.DLLname, pProcName);
                        return result;
                    }
                }
                // minimaal 1 dll aanwezig
                if (locklist.Count() > 0)
                    unLogger.WriteInfo("WIN32API", String.Format("[{0}] ExecuteProcedureDLLStack All Threads Busy! {1} ", locklist[0].DLLname, pProcName));
                else
                    throw new FileNotFoundException(" DLL niet gevonden \r\n procedure " + pProcName);
                Thread.Sleep(1000);
                // alle dlls zijn bezet... wat nu? nog een keer proberen???
            }
            throw new TimeoutException("ExecuteProcedureDLLStack could not open function " + pProcName + " " + locklist[0].DLLname);
        }

        protected bool FreeDLL(String pFilename)
        {
            unLogger.WriteInfo("WIN32API",  String.Format("[{0}] FreeDLL mUseShareMem = {1}",pFilename,mUseShareMem.ToString()));
            bool result;
            //if (mUseShareMem)
            //    result = FreeLibraryRelative("lib\\borlndmm.dll");
            result = FreeLibraryRelative("lib\\" + pFilename);
            //
            return result;
        }

        public static String GetBaseDir()
        {
            return System.AppDomain.CurrentDomain.BaseDirectory;
        }

        protected string Decodeer_PW(string Tekst)
        {
            PasswordDecryptor Decryptor = new PasswordDecryptor();
            return Decryptor.Decodeer_PW(Tekst);
        }


        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~Win32()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                FreeHandles(disposing);

                // Note disposing has been done.
                disposed = true;

            }
        }
        public void FreeHandles()
        {
            FreeHandles(true);
        }

        //[HandleProcessCorruptedStateExceptions]
        private void FreeHandles(bool disposing)
        {
            lock (padlock)
            {
                try
                {
                    foreach (KeyValuePair<String, IntPtr> handlePair in mDLLPointers)
                    {
                        IntPtr handle = handlePair.Value;
                        if (disposing)
                            unLogger.WriteDebug("WIN32API", String.Format("[{0}] FreeHandles: Call FreeLibrary {1}", handlePair.Key, unLogger.StackInfo(2, false)));
                        else
                            unLogger.WriteDebug("WIN32API", String.Format("[{0}] FreeHandles: Call FreeLibrary GC Thread", handlePair.Key));
                        for (int recusivecounter = 0; recusivecounter < 85000; recusivecounter++)
                        {
                            if (FreeLibrary(handle))
                                break;
                            Thread.Sleep(1000);
                        }
                    }
                }
                catch (AccessViolationException exc)
                {
                    unLogger.WriteErrorFormat("WIN32API", "AccessViolation {0}", exc.Message);
                    unLogger.WriteDebug("WIN32API", exc.ToString());
                }
                catch (Exception exc)
                {
                    unLogger.WriteDebug("WIN32API", exc.ToString());
                }
                mDLLPointers.Clear();
            }

        }

    }


    public class LockObject
    {
        private String name;
        public LockObject(String DllName)
        {
            name = DllName;
        }

        public String DLLname
        {
            get
            {
                return name;
            }

        }

    }
}
