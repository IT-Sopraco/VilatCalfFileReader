using System;
using System.Runtime.InteropServices;

namespace VSM.RUMA.CORE
{
    [Guid("C5C88D51-1032-43a6-849B-2DC9203B4C46"),
   ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId("COMwrapper")] 
    public class COMwrapper
    {

        private IDelphiEvents EventCaller;
        private String Progressmessage;
        public COMwrapper()
        {
            Facade.GetInstance().Application_Update += new ProgressUpdateEvent(COMwrapper_Application_Update);
            Progressmessage = string.Empty;
        }

        void COMwrapper_Application_Update(Facade sender, int progress, string message)
        {
            Progressmessage = progress + "% " + message;
            unLogger.WriteInfo(Progressmessage);
            EventCaller.Application_Update(progress, message);
        }

        public IDelphiEvents Events
        {
            get
            {
                return EventCaller;
            }
            set
            {
               EventCaller = value;
            }
        }


        public Facade GetFacade()
        {
            try
            {

                return Facade.GetInstance();
            }
            catch (Exception ex)
            {
                EventCaller.Application_Update(0, ex.Message);
                return null;
            }
        }
        /*
        public AddUpdate( pFunction)
        {
            Facade.GetInstance().Application_Update += new ProgressUpdateEvent(pFunction);
        }*/
        
        public String GetLastProgressstring()
        {
            return Progressmessage;
        }
    }
}
