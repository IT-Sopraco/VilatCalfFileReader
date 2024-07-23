using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using VSM.RUMA.CORE.DB.DataTypes;
using VSM.RUMA.CORE;
using System.Runtime.CompilerServices;

namespace VSM.RUMA.CORE.EDINRS
{
    [Serializable]
    class ReaderPlugin : IReaderPlugin, IFacade
    {
        public event DomainEventHandler Application_Loading;
        private AFSavetoDB DB;

        public String GetFilter()
        {
            return "*.VEE*";
        }

        public List<String> getExcludeList()
        {
            return new List<String>();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int LeesFile(int thrId, DBConnectionToken agroFactuurToken, int programId, String agrobaseUser,
                                    String agrobasePassword, int fileLogId, String Bestandsnaam)
        {
            try
            {
                int ProgId;
                switch (programId)
                {
                    case 1:
                    case 2:
                    case 53:
                    case 3500:
                    case 3599:
                    case 3600:
                    case 3699:
                    case 3700:
                    case 3799:
                    case 3800:
                    case 3899:
                    case 3900:
                    case 3999:
                    case 4000:
                    case 4099:
                    case 4100:
                    case 4199:
                    case 13000:
                        ProgId = 1;         // progid 1,4
                        break;

                    default: ProgId = 0;
                        break;
                }
                String Berichttype, Versienr, Specificatie;
                DateTime BestandsDatum, BestandsTijd;

                RUMAcomEDINRS NRS = new RUMAcomEDINRS(this, agroFactuurToken, fileLogId);
                NRS.LeesHeader(Bestandsnaam, out Berichttype, out Versienr, out  Specificatie,
                          out  BestandsDatum, out  BestandsTijd);
                bool IsEVOBestand = Berichttype == "503";                
                String UBNnr = NRS.LeesUBN(Bestandsnaam);
                UBN lbestandsUBN = DB.getUBNByBedrijfsnummer(UBNnr);
                BEDRIJF lFarm = DB.GetBedrijfByUbnIdProgIdProgramid(lbestandsUBN.UBNid, ProgId, programId);
                if (lFarm.FarmId <= 0)
                {
                    var farms = DB.getBedrijvenByUBNId(lbestandsUBN.UBNid).Where(be => be.ProgId == ProgId);
                    if (farms.Count() > 0)
                    {
                        lFarm = farms.First();
                    }
                }
                IsEVOBestand = false;//uitgezet de xmlimporter zou de data beter uitlezen VSMDATAKOP-210
                if (IsEVOBestand)
                {

                    if (NRS.LeesEVOBestand(thrId, Bestandsnaam, lFarm.FarmId))
                    {
                        unLogger.WriteDebug("EDINRS.Reader .EVO :" + Bestandsnaam + " ingelezen");
                        return 1;
                    }
                    else return -1;

                }
                else
                {
                    String File = System.IO.Path.GetFileNameWithoutExtension(Bestandsnaam);
                    String LogFile = unLogger.getLogDir("XML") + "EDINRS#" + File + "#" + DateTime.Now.Ticks + ".log";
                    Win32PDA2Agrobase PDA2Agrobase = new Win32PDA2Agrobase();
                    bool Result = PDA2Agrobase.importXMLMySQL(programId,
                            agrobaseUser,
                            agrobasePassword,
                            LogFile, DB.GetDataBase().getDBHost(), Bestandsnaam,fileLogId);

                    unLogger.WriteDebug("EDINRS2XMLReader.Reader    .VEE       : " + Bestandsnaam + " ingelezen: result: " + Result.ToString());
                    if (Result)
                        return 1;
                    else return -1;
                }
                return -1;
            }
            catch (Exception ex)
            {
                unLogger.WriteError("FOUT bij het inlezen van bestand " + Bestandsnaam);
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
