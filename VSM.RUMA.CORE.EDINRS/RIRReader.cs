using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.DB.DataTypes;
using VSM.RUMA.CORE;
using System.Runtime.CompilerServices;

namespace VSM.RUMA.CORE.EDINRS
{
    [Serializable]
    class RIRReader : IFacade 
                  //: IReaderPlugin, IFacade
    {
        public event DomainEventHandler Application_Loading;
        private AFSavetoDB DB;

        public String GetFilter()
        {
            return "*.RIR";
        }

        public List<String> getExcludeList()
        {
            return new List<String>();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int LeesFile(int pThrId, DBConnectionToken pAgroFactuurToken, int pProgramID, String pAgrobaseUser,
                                    String pAgrobasePassword, int FileLogId, String Bestandsnaam)
        {
            try
            {
                int ProgId;
                switch (pProgramID)
                {
                    case 1: ProgId = 1;         // progid 1,4
                        break;
                    case 2: ProgId = 1;         // progid 1,4
                        break;
                    default: ProgId = 0;
                        break;
                }
                RUMAcomEDINRS NRS = new RUMAcomEDINRS(this, pAgroFactuurToken);
                if (NRS.IRretour_Terugmelding(pThrId, Bestandsnaam) > 0)
                {
                    unLogger.WriteDebug("IRretour.Reader :" + Bestandsnaam + " ingelezen");
                    return 1;
                }
                return -1;
            }
            catch (Exception ex)
            {
                unLogger.WriteError(ex.Message, ex);
                return -2;
            }
        }

        public void setDatacomPath(string Path)
        {

        }

        public void setSaveToDB(AFSavetoDB value)
        {
            DB = value;
        }

        public void setSaveToDB(AFSavetoDB value, DBConnectionToken pToken)
        {
            DB = value;
        }

        public AFSavetoDB getSaveToDB(DBConnectionToken pToken)
        {
            DB.setToken(pToken);
            return DB;
        }


        public void UpdateProgress(int progress, string message)
        {
            unLogger.WriteDebug("EDINRS.Reader :" + progress.ToString() + "%  " + message);
        }

        public AFSavetoDB getSaveToDB()
        {
            return DB;
        }

        public string Version()
        {
            System.Reflection.Assembly AssemblyInfo = this.GetType().Assembly;
            String LoaderTitle = "Versie: " + AssemblyInfo.GetName().Version.ToString();
            return LoaderTitle;
        }

        public void LoadApplication(DBConnectionToken pToken, int FarmId)
        {

        }

        public ISendReceive getSendReceive()
        {
            throw new NotImplementedException();
        }
    }
}
