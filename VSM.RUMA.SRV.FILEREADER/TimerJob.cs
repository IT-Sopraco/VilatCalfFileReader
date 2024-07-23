using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using VSM.RUMA.CORE.COMMONS;
using VSM.RUMA.CORE;

namespace VSM.RUMA.SRV.FILEREADER
{
    public class TimerJob
    {
        private Timer timer;
        private TimerCallback timerDelegate;
        private DBConnectionToken mToken;

        public TimerJob(ITimerTask pTask, DBConnectionToken pAgroFactuurToken)
        {
            try
            {
                timerDelegate = new TimerCallback(timer_Elapsed);
                TimeSpan Delay = pTask.GetDelay();
                if (Delay.Ticks <= 0) Delay = Delay.Add(new TimeSpan(1, 0, 0, 0));
                timer = new Timer(timerDelegate, pTask, Delay, pTask.GetInterval());
                unLogger.WriteInfo(String.Format("Add Task {0} : starting {1} ", pTask.GetType().Name, DateTime.Now.Add(pTask.GetDelay())));
                mToken = (DBConnectionToken) pAgroFactuurToken.Clone();
                //if (pTask.GetDelay().Ticks <= 0) new Timer(timerDelegate, pTask, new TimeSpan(0, 0, Delay.Hours, Delay.Minutes), new TimeSpan(0, 0, 0, 0, -1));

            }
            catch (Exception ex)
            {
                unLogger.WriteError("Error Scheduling Task ", ex);
            }

        }

        void timer_Elapsed(object pTask)
        {
            try
            {
                int pogingen = 0;
                bool klaar = false;
                while (!klaar && pogingen < 10)
                {
                    try
                    {
                        ITimerTask lTask = (ITimerTask)pTask;
                        unLogger.WriteInfo(String.Format("Running Task {0} : scheduled on {1} ", lTask.GetType().Name, DateTime.Now.Subtract(lTask.GetDelay())));
                        #if DEBUG
                        klaar = lTask.DoTask(mToken);
                        #else
                        klaar = DoTask(lTask);
                        #endif
                        unLogger.WriteInfo(String.Format("Finished Task {0} : next start {1} ", lTask.GetType().Name, DateTime.Now.Add(lTask.GetInterval())));
                        klaar = true;
                    }
                    catch (Exception ex)
                    {
                        unLogger.WriteError("Error Running Task ", ex);
                    }

                    pogingen++;
                }
            }
            catch (Exception ex)
            {
                unLogger.WriteError("Error timer_Elapsed Task ", ex);
            }
        }


        public bool DoTask(ITimerTask pTask)
        {

            int Result = -1;
            String FilereaderTask = "FilereaderTask.exe";
            if (System.IO.File.Exists(FilereaderTask))
            {

                System.Diagnostics.Process p = new System.Diagnostics.Process();
                try
                {
                    unLogger.WriteInfo("Spawning FilereaderTask for file : " + pTask.GetType().Name);
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.FileName = FilereaderTask;
                    unLogger.WriteDebug("CurrentDirectory :" + System.IO.Directory.GetCurrentDirectory());
                    unLogger.WriteDebug("BaseDirectory    : " + AppDomain.CurrentDomain.BaseDirectory);
                    p.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    StringBuilder SBArgs = new StringBuilder();
                    SBArgs.AppendFormat(" -Library  \"{0}\" ", pTask.GetType().Module.Name);
                    SBArgs.AppendFormat(" -Task  \"{0}\" ", pTask.GetType().Name);
                    p.StartInfo.Arguments = SBArgs.ToString();
                    p.Start();
                    p.WaitForExit();
                    Result = p.ExitCode;

                }
                catch (Exception ex)
                {
                    unLogger.WriteDebug(ex.Message, ex);
                    Result = -2;
                }
                finally
                {
                    if (!p.HasExited)
                    {
                        Result = -3;
                        p.Kill();
                    }
                    unLogger.WriteInfo("FilereaderTask Finished : " + pTask.GetType().Name);
                }
            }
            else
            {
                unLogger.WriteInfo(FilereaderTask + " not found!");
            }

            return Result == 0;
        }

        public void StopJob()
        {
            lock (timer)
            {
                timerDelegate = null;
                timer.Dispose();
            }
        }
    }
}
