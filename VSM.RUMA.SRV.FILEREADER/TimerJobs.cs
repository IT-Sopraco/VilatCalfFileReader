using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using VSM.RUMA.CORE.COMMONS;
using VSM.RUMA.CORE;

namespace VSM.RUMA.SRV.FILEREADER
{
    class TimerJobs
    {
        private DBConnectionToken mToken;
        private List<TimerJob> mTimerJobs = new List<TimerJob>();

        public TimerJobs(DBConnectionToken pAgroFactuurToken)
        {
            mToken = pAgroFactuurToken;
        }

        internal void ScheduleTask(ITimerTask pTask)
        {
            try
            {
                TimerJob Job = new TimerJob(pTask, mToken);
                mTimerJobs.Add(Job);
            }
            catch (Exception ex)
            {
                unLogger.WriteError("Error Scheduling Task ", ex);
            }
        }
        internal void StopTasks()
        {
            try
            {
                foreach (TimerJob Job in mTimerJobs)
                {
                    try
                    {
                        Job.StopJob();
                    }
                    catch (Exception ex)
                    {
                        unLogger.WriteError("Error Stopping Task ", ex);
                    }
                }
                mTimerJobs.Clear();
            }
            catch (Exception ex)
            {
                unLogger.WriteError("Error Stopping Tasks ", ex);
            }

        }

    }
}
