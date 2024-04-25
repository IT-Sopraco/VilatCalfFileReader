using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using VSM.RUMA.CORE;
using VSM.RUMA.CORE.COMMONS;
using VSM.RUMA.CORE.DB;
using VSM.RUMA.CORE.DB.DataTypes;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace VSM.RUMA.SRV.FILEREADER.SOPRACO
{
    [Serializable]
    public class SopracoTXTPlugin : IReaderPlugin
    {
        readonly int _ChangedBy = 2946;

        readonly string SOPRACO_CSV_DIR_VOER = "Voer";
        readonly string SOPRACO_CSV_DIR_MEDICIJN = "Medicijnen";
        readonly string SOPRACO_CSV_DIR_BLOED = "Bloedonderzoeken";
        readonly string SOPRACO_CSV_DIR_DIER = "Dier";
        readonly string SOPRACO_CSV_DIR_SLACHTDATA = "Slachtdata";
        readonly char[] SPLIT = { ';' };
        //private Agrofactuurlogin auth;
        //private FilebufferSync fbs;
        //private IAPIConfigurationSettings _settings;
        //private ILogManager _log;
        //private ISaveTaskLog _tasklog;

        public int LeesFile(int pThrId, DBConnectionToken pAgroFactuurToken, int pProgramID, string pAgrobaseUser, string pAgrobasePassword, int FileLogId, string Bestandsnaam)
        {
            //Het gebruik van de ageobase.api versie word nog niet geaccepteerd op de inleesserver. 
            //ivm de System.Web.Mvc versie (e.a.) conflicten.
            //daarom is het inlezen in deze  Module zelf gezet. via de klasse FilebufferSync
            //TODO indien versies later wel overeenkomen, de code van FilebufferSync naar de agrobase.api overzetten.
            
           
            char[] bloodkommasplitter = { ',' };
            //if (!Bestandsnaam.StartsWith(SOPRACO_CSV_BASE))
            //{
            //    unLogger.WriteDebug($"{nameof(SopracoCSVPlugin)}.{nameof(LeesFile)} Bestand '{Bestandsnaam}' overgeslagen. (Ongeldige base dir)");
            //    return (int)LABELSConst.IReaderPluginResult.InvalidFileTypeForPlugin;
            //}

            unLogger.WriteInfo($"{nameof(SopracoCSVPlugin)}.{nameof(LeesFile)} '{Bestandsnaam}' begonnen.");

            //SessionToken token;
            //if (!Login(out token))
            //{
            //    unLogger.WriteError($"{nameof(SopracoCSVPlugin)}.{nameof(LeesFile)} Error tijdens inloggen API.");
            //    return -2;
            //}
            UserRightsToken URT = Facade.GetInstance().getRechten().Inloggen("00000", Facade.GetInstance().getRechten().Codeer_base64("kcD0QJC3"));

       
            string dirName = new FileInfo(Bestandsnaam).Directory.FullName;

            FilebufferSync.FILE_IMPORT_Filekind fileKind;
            if (dirName.ToUpperInvariant().EndsWith(SOPRACO_CSV_DIR_VOER.ToUpperInvariant()))
                fileKind = FilebufferSync.FILE_IMPORT_Filekind.Voer;
            else if (dirName.ToUpperInvariant().EndsWith(SOPRACO_CSV_DIR_BLOED.ToUpperInvariant()))
                fileKind = FilebufferSync.FILE_IMPORT_Filekind.Bloedonderzoek1;
            else if (dirName.ToUpperInvariant().EndsWith(SOPRACO_CSV_DIR_DIER.ToUpperInvariant()))
                fileKind = FilebufferSync.FILE_IMPORT_Filekind.Dieraanvoer;
            else if (dirName.ToUpperInvariant().EndsWith(SOPRACO_CSV_DIR_MEDICIJN.ToUpperInvariant()))
                fileKind = FilebufferSync.FILE_IMPORT_Filekind.Medicijnen;
            else if (dirName.ToUpperInvariant().EndsWith(SOPRACO_CSV_DIR_SLACHTDATA.ToUpperInvariant()))
                fileKind = FilebufferSync.FILE_IMPORT_Filekind.SlachtdataSopraco;
            else
            {
                unLogger.WriteError($"{nameof(SopracoCSVPlugin)}.{nameof(LeesFile)} Kan bestandsType niet achterhalen voor bestand: '{Bestandsnaam}'.");
                return -2;
            }
            try
            {

                if (fileKind == FilebufferSync.FILE_IMPORT_Filekind.Bloedonderzoek1)
                {
                    if (Bestandsnaam.ToLower().EndsWith(".csv"))
                    {
                        fileKind = FilebufferSync.FILE_IMPORT_Filekind.Bloedonderzoek2;

                        if (Bestandsnaam.ToUpper().Contains("BROK"))
                        {
                            fileKind = FilebufferSync.FILE_IMPORT_Filekind.Bloedonderzoek3;
                        }
                    }
                    else
                    {
                        if (Bestandsnaam.ToUpper().Contains("BROK"))
                        {
                            fileKind = FilebufferSync.FILE_IMPORT_Filekind.Bloedonderzoek3;
                        }
                    }
                    //controle naam bloedwaardes
                    if (fileKind == FilebufferSync.FILE_IMPORT_Filekind.Bloedonderzoek1 || fileKind == FilebufferSync.FILE_IMPORT_Filekind.Bloedonderzoek2)
                    {
                        int ret = FilebufferSync.GetInstance(URT, _ChangedBy).Controlebloodfilename(fileKind, Bestandsnaam);
                        if (ret < 0)
                        {
                            return ret;
                        }
                    }
                    
                }
                if (fileKind == FilebufferSync.FILE_IMPORT_Filekind.SlachtdataSopraco)
                {
                    //Welke slachtdata ?
                    using (StreamReader reader = new StreamReader(Bestandsnaam))
                    {
                        string line;

                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] regelarr = line.Split(SPLIT);
                            if (regelarr.Length > 4)
                            {
                                DateTime? datum = FilebufferSync.GetInstance(URT, _ChangedBy).GetDateFormat(regelarr[0], regelarr[1]);
                                if (!datum.HasValue)
                                {

                                    datum = utils.getDateSlacht(regelarr[1]);
                                    if (datum.HasValue)
                                    {
                                        fileKind = FilebufferSync.FILE_IMPORT_Filekind.SlachtdataSarreguemines;
                                    }
                                }
                                else
                                {
                                    fileKind = FilebufferSync.FILE_IMPORT_Filekind.SlachtdataSopraco;
                                }
                            }
                        }
                    }
                }
                List<FILE_IMPORT> result = new List<FILE_IMPORT>();
                unLogger.WriteInfo($@"Save txt FILE_IMPORT's Bestandsnaam:{Bestandsnaam} ");
                using (StreamReader reader = new StreamReader(Bestandsnaam))
                {
                    string line;
                    int Regelnr = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Regelnr += 1;
                        FILE_IMPORT f = FilebufferSync.GetInstance(URT, _ChangedBy).LeveringenImport(fileKind, Bestandsnaam, line, FilebufferSync.FILE_IMPORT_STATUS.NogNietBeoordeeld, Regelnr);
                        if (f.FILE_IMPORT_ID > 0)
                        {
                            result.Add(f);
                        }
                    }
                }
                
                
                if (fileKind == FilebufferSync.FILE_IMPORT_Filekind.Bloedonderzoek1 || fileKind == FilebufferSync.FILE_IMPORT_Filekind.Bloedonderzoek2 || fileKind == FilebufferSync.FILE_IMPORT_Filekind.Bloedonderzoek3)
                {
                    //alleen de .txt
                    unLogger.WriteInfo($@"Save txt Broklocaties Bestandsnaam:{Bestandsnaam} ");
                    FilebufferSync.GetInstance(URT, _ChangedBy).Broklocaties(fileKind, Bestandsnaam);

                    unLogger.WriteInfo($@"Save txt Brokvergelijk Bestandsnaam:{Bestandsnaam} ");
                    FilebufferSync.GetInstance(URT, _ChangedBy).Brokvergelijk(fileKind, Bestandsnaam);

                    unLogger.WriteInfo($@"Save txt Savebloodindatabase Bestandsnaam:{Bestandsnaam} ");
                    FilebufferSync.GetInstance(URT, _ChangedBy).Savebloodindatabase(fileKind, Bestandsnaam, 82);

                }
                if (fileKind == FilebufferSync.FILE_IMPORT_Filekind.SlachtdataSopraco || fileKind == FilebufferSync.FILE_IMPORT_Filekind.SlachtdataSarreguemines || fileKind == FilebufferSync.FILE_IMPORT_Filekind.SlachtdataAnder)
                {

                    //ThreadStart starter = delegate { fbs.saveslachtdata(fileKind, Bestandsnaam, 82); };
                    //Thread newThread = new Thread(starter);
                    //newThread.Start();
                    unLogger.WriteInfo($@"Save txt Saveslachtdata Bestandsnaam:{Bestandsnaam} ");
                    FilebufferSync.GetInstance(URT, _ChangedBy).Saveslachtdata(fileKind, Bestandsnaam, result, 82);
                }

                FilebufferSync.GetInstance(URT, _ChangedBy).controleervoeren(fileKind, Bestandsnaam, result);
             
                unLogger.WriteInfo($"{nameof(SopracoCSVPlugin)}.{nameof(LeesFile)} '{Bestandsnaam}' verwerkt?.");
            }
            catch (Exception ex)
            {
                unLogger.WriteError($"{nameof(SopracoCSVPlugin)}.{nameof(LeesFile)} '{Bestandsnaam}' SopracoTXTImport ex: {ex.ToString()}", ex);
                return -2;
            }

            return 1;
        }

  

        #region IReaderPlugin

        public void setSaveToDB(CORE.AFSavetoDB value)
        {

        }

        public List<string> getExcludeList()
        {
            return new List<string>();
        }

        public string GetFilter()
        {
            return "*.TXT*";
        }

        #endregion
    }
 
}
