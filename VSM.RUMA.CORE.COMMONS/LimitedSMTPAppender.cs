using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Appender;
using log4net.Core;

namespace VSM.Logger
{
    public class LimitedSMTPAppender : SmtpAppender
    {
        private int limit = 50;           // max at 10 mails ...
        private int cycleSeconds = 3600;  // ... per hour

        public void setLimit(int limit)
        {
            this.limit = limit;
        }

        public void setCycleSeconds(int cycleSeconds)
        {
            this.cycleSeconds = cycleSeconds;
        }

        private int lastVisited;
        private long lastCycle;
        private DateTime lastFlush;
        private StringBuilder Messages;

        public new string Subject { 
            get
            {
                return String.Format("[{0}] {1}",AppDomain.CurrentDomain.GetData("Username"),base.Subject);
            }
            set
            {
                base.Subject = value;
            }
        }

 
        protected override void SendEmail(string messageBody)
        {
            if (Messages == null) Messages = new StringBuilder();
            Messages.Append(messageBody);
            if (CanMail())
                //Messages.Length > (BufferSize * 200) && CanMail())
            {
                base.SendEmail(Messages.ToString());
                Messages = null;
            }

            //base.SendEmail(messageBody);
        }
        



        private bool CanMail()
        {
            if (lastFlush.AddMinutes(1) > DateTime.Now)
                return false;
            lastFlush = DateTime.Now;
            long now = Convert.ToInt64(DateTime.Now.TimeOfDay.TotalMilliseconds);
            long thisCycle = now - (now % (1000L * cycleSeconds));
            if (lastCycle != thisCycle)
            {
                lastCycle = thisCycle;
                lastVisited = 0;
            }
            lastVisited++;
            return lastVisited <= limit;
        }


    }
}
