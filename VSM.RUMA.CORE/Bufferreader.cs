using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE;
using VSM.RUMA.CORE.DB.DataTypes;
using VSM.RUMA.CORE.DB.MYSQL;
namespace VSM.RUMA.CORE
{
    public class Bufferreader
    {
        private const int ChangedBy = 1515;
        private AFSavetoDB AFSavetoDBObj;
        private DBSelectQueries DBSelectQueriesObj;
        private char[] splitters = { ';' };
        private char[] splittersfilename = { '_' };
        public enum FILE_IMPORT_STATUS
        {
            NogNietBeoordeeld = 1,
            Bevestigd = 2,
            Afgekeurd = 3
        }

        public enum FILE_IMPORT_Filekind
        {
            Voer = 1,
            Medicijnen = 2,
            Bloedonderzoek = 3,
            Dier = 6
        }

        /*
            Data opslaan in de Buffertabel alvorens op te slaan in de normale tabellen na acceptatie van de Mester 
            Zie G:\agrobase\Projecten\Sopraco\ImportbestandenSopraco.doc
         */

        public Bufferreader(UserRightsToken pUserRichtsToken)
        {
            AFSavetoDBObj = (AFSavetoDB)Facade.GetInstance().getMaster(pUserRichtsToken);
            DBSelectQueriesObj = Facade.GetInstance().getSlave(pUserRichtsToken);
        }

        public void leveringenImport(string pImportFile, FILE_IMPORT_Filekind pFilekind, int pThrID_User, int pAgrolinkProgram, string pImportFileXtra2, string pImportFileXtra3)
        {
            /*
                Zie G:\agrobase\Projecten\Sopraco\ImportbestandenSopraco.doc
             */
             
            SOAPLOG sl = new SOAPLOG();
            sl.SourceID = pThrID_User;
            sl.Changed_By = ChangedBy;
            sl.Date = DateTime.Now.Date;
            sl.Time = DateTime.Now;
            string soort = "Voerlevering";
            unLogger.WriteInfo(" leveringenImport " + soort + " " + pImportFile);
            switch (pFilekind)
            {
                case FILE_IMPORT_Filekind.Voer:
                    soort = "Voerlevering";
                    break;
                case FILE_IMPORT_Filekind.Bloedonderzoek:
                    //soort = "Bloedonderzoek";
                    throw new NotImplementedException();
                case FILE_IMPORT_Filekind.Medicijnen:
                    soort = "Medicijnlevering";
                    break;
            }
            sl.Code = "leveringenImport " + soort;
            string[] Filestoread = { pImportFile, pImportFileXtra2, pImportFileXtra3 };
            
            for(int i=0;i<Filestoread.Count();i++)//  string Filetoread in Filestoread)
            {
                if (!String.IsNullOrEmpty(Filestoread[i]) && File.Exists(Filestoread[i]))
                {
                    sl.Status = "G";
                    sl.Omschrijving = "VoerleveringenSopraco  in buffertabel:" + Filestoread[i];
                    try
                    {

                        string json = "Datum:DateTime,Artikelcode:string,Hoeveelheid:double,Opslag:string,Inrichtingsnr:string,Onbekend1:string,Onbekend2:string,Fokkersnr:string";
                        UBN uDestination = new UBN();
                        THIRD tDestination = new THIRD();
                        FILE_IMPORT_TYPE fit = new FILE_IMPORT_TYPE();
                        using (StreamReader reader = new StreamReader(Filestoread[i]))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                string[] importline = line.Split(splitters);
                                string Fokkersnummer = "";
                                string Inrichtingsnr = "";
                                
                                switch (pFilekind)
                                {
                                    case FILE_IMPORT_Filekind.Voer:
                                        if (importline.Length >= 8)
                                        {
                                            Fokkersnummer = importline[7];
                                            Inrichtingsnr = importline[4];
                                        }

                                        break;
                                    case FILE_IMPORT_Filekind.Bloedonderzoek:
                                        // 3 files to read
                                        throw new NotImplementedException();
                                        break;
                                    case FILE_IMPORT_Filekind.Medicijnen:
                                        json = "volgnr:int,leverdatum:DateTime,bruto bedrag excl btw:double,Hoeveelheid:double,eenheid:int,RegNL:string,brotoprijs:double,omschrijving:string,btw perc:double,btw bedrag:double,batchnr:string,Inrichtingsnr:string,Fokkersnr:string,Onbekend:string";
                                        if (importline.Length >= 12)
                                        {
                                            Fokkersnummer = importline[12];
                                            Inrichtingsnr = importline[11];
                                        }
                                        break;
                                }
                                if (tDestination.ThrId == 0 || tDestination.ThrStamboeknr != Fokkersnummer)
                                {
                                    tDestination = new THIRD();// AFSavetoDBObj.GetThirdByStamboeknr(Fokkersnummer);
                                }
                                if (tDestination.ThrId > 0)
                                {
                                    if (uDestination.ThrID == 0 || tDestination.ThrId != uDestination.ThrID)
                                    {
                                        List<UBN> ubns = AFSavetoDBObj.getUBNsByThirdID(tDestination.ThrId);
                                        if (ubns.Count() > 0)
                                        {
                                            uDestination = ubns.ElementAt(0);
                                        }
                                    }
                                    if (uDestination.UBNid > 0)
                                    {

                                        FILE_IMPORT fi = AFSavetoDBObj.getFile_ImportByData(line);
                                        if (fi.FILE_IMPORT_ID == 0)
                                        {
                                            if (fit.File_Import_Type_ID <= 0)
                                            {
                                                fit.Changed_By = ChangedBy;
                                                fit.FIT_Column_JSON = json;
                                                fit.FIT_Description = soort;
                                                fit.SourceID = pThrID_User;
                                                int File_Import_Type_ID = AFSavetoDBObj.saveFile_Import_Type(fit);
                                                fit.File_Import_Type_ID = File_Import_Type_ID;
                                            }
                                            if (fit.File_Import_Type_ID > 0)
                                            {
                                                fi.File_Import_Type_ID = fit.File_Import_Type_ID;
                                                fi.Changed_By = ChangedBy;
                                                fi.SourceID = pThrID_User;
                                                fi.FI_Data_Row = line;
                                                fi.FI_Filename = System.IO.Path.GetFileName(Filestoread[i]);
                                                fi.FI_State = (int)FILE_IMPORT_STATUS.NogNietBeoordeeld;
                                                fi.ProgID_Origin = pAgrolinkProgram;
                                                fi.ThrID_Destination = tDestination.ThrId;
                                                fi.ThrID_Origin = pThrID_User;
                                                fi.UbnID_Destination = uDestination.UBNid;
                                                AFSavetoDBObj.saveFile_Import(fi);
                                            }
                                        }
                                        
                                    }
                                    else
                                    {
                                        unLogger.WriteError(soort + " Find UBN by ThrStamboeknr " + line);
                                        sl.Status = "F";
                                        sl.Omschrijving = soort + " Find UBN by ThrStamboeknr " + line;
                                    }
                                }
                                else
                                {
                                    unLogger.WriteError(soort + " Find THIRD by ThrStamboeknr " + line);
                                    sl.Status = "F";
                                    sl.Omschrijving = soort + " Find THIRD by ThrStamboeknr " + line;
                                }
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        unLogger.WriteError(exc.ToString());
                        sl.Status = "F";
                        sl.Omschrijving = soort + "  " + exc.Message;
                    }
                }
                else
                {
                    unLogger.WriteError(soort + " File Not found:" + Filestoread[i]);
                    sl.Status = "F";
                    sl.Omschrijving = soort + " File Not found:" + Filestoread[i];
                }
            }
           
            AFSavetoDBObj.WriteSoapError(sl);

        }

        private SOAPLOG MedicijnleveringenSopraco(string pMedicijnleveringBestand, int pThrID_User)
        {

            /*
                Zie G:\agrobase\Projecten\Sopraco\ImportbestandenSopraco.doc
             */
            SOAPLOG sl = new SOAPLOG();
            sl.SourceID = pThrID_User;
            sl.Changed_By = ChangedBy;
            sl.Date = DateTime.Now.Date;
            sl.Time = DateTime.Now;
            if (File.Exists(pMedicijnleveringBestand))
            {
                sl.Status = "G";
                sl.Code = "";
                sl.Omschrijving = "MedicijnleveringenSopraco  in buffertabel:" + pMedicijnleveringBestand;
                try
                {


                }
                catch (Exception exc)
                {
                    unLogger.WriteError(exc.ToString());
                    sl.Status = "F";
                    sl.Code = "";
                    sl.Omschrijving = " MedicijnleveringenSopraco " + exc.Message;
                }
            }
            else
            {
                sl.Status = "F";
                sl.Code = "";
                sl.Omschrijving = "File Not found:" + pMedicijnleveringBestand;
            }
            AFSavetoDBObj.WriteSoapError(sl);
            return sl;
        }

        private SOAPLOG BloedonderzoekSopraco(string pBloedonderzoekBestand, int pThrID_User)
        {
            /*
                Zie G:\agrobase\Projecten\Sopraco\ImportbestandenSopraco.doc
             */
            SOAPLOG sl = new SOAPLOG();
            sl.SourceID = pThrID_User;
            sl.Changed_By = ChangedBy;
            sl.Date = DateTime.Now.Date;
            sl.Time = DateTime.Now;
            if (File.Exists(pBloedonderzoekBestand))
            {


                sl.Status = "G";
                sl.Code = "";
                sl.Omschrijving = "BloedonderzoekSopraco  in buffertabel:" + pBloedonderzoekBestand;
                try
                {


                }
                catch (Exception exc)
                {
                    unLogger.WriteError(exc.ToString());
                    sl.Status = "F";
                    sl.Code = "";
                    sl.Omschrijving = " BloedonderzoekSopraco " + exc.Message;
                }
            }
            else
            {
                sl.Status = "F";
                sl.Code = "";
                sl.Omschrijving = "File Not found:" + pBloedonderzoekBestand;
            }
            AFSavetoDBObj.WriteSoapError(sl);
            return sl;
        }
    }
}
