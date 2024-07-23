using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Runtime.CompilerServices;
using VSM.RUMA.CORE.DB.DataTypes;
using VSM.RUMA.CORE;
using System.Xml;
using System.IO;
using System.Data;
using System.Threading;

namespace VSM.RUMA.CORE.XML2AGROBASE
{
    [Serializable]
    class ReaderPlugin : IReaderPlugin
    {
        private AFSavetoDB DB;
        private DBConnectionToken fToken;
        static readonly object padlck = new object();

        public String GetFilter()
        {
            return "*.XML*";
        }

        public List<String> getExcludeList()
        {
            List<String> Excludes = new List<String>();
            Excludes.Add(".ZIP");
            Excludes.Add(".TMP");
            Excludes.Add(".FXML");
            Excludes.Add(".FAIL");
            Excludes.Add("\\IN");
            string excludedirs = ConfigurationManager.AppSettings["ExcludeDirs"];
            foreach (string dir in excludedirs.Split(';'))
            {
                Excludes.Add($"\\{dir}");
            };

            return Excludes;
        }

        public int LeesFile(int thrId, DBConnectionToken agroFactuurToken, int programId, String agrobaseUser,
                                    String agrobasePassword, int fileLogId, String Bestandsnaam)
        {
            try
            {
                fToken = agroFactuurToken;
                Win32PDA2Agrobase PDA2Agrobase = new Win32PDA2Agrobase();
                unLogger.WriteDebug("XML2AGROBASE.Reader Username : " + agrobaseUser);
                //wachtwoorden niet loggen
                //unLogger.WriteDebug("XML2AGROBASE.Reader Password : " + agrobasePassword);
                unLogger.WriteDebug("XML2AGROBASE.Reader HostName : " + unRechten.getDBHost());
                unLogger.WriteDebug("XML2AGROBASE.Reader LogDir   : " + unLogger.getLogDir());
                String File = System.IO.Path.GetFileNameWithoutExtension(Bestandsnaam);
                String LogFile = unLogger.getLogDir("XML") + "pda2agrobase#" + File + "#" + DateTime.Now.Ticks + ".log";
                FILELOG fl = Facade.GetInstance().getSaveToDB(agroFactuurToken).getFileLogById(fileLogId);
                fl.Logfilepath = LogFile;
                Facade.GetInstance().getSaveToDB(agroFactuurToken).saveFileLog(fl);

                int Result;
                //luc dump bestanden vanuit ruma zijn al in het juiste formaat, deze door de xml.exe halen zorgt alleen voor een extra vertraging dus deze stap voor dump bestanden overslaan.
                if (Bestandsnaam.ToLower().Contains("dump"))
                {
                    Result = PDA2Agrobase.importXMLMySQL(programId,
                           agrobaseUser,
                           agrobasePassword,
                           LogFile, unRechten.getDBHost(), Bestandsnaam, fileLogId);
                }
                else
                {
                    Result = ImportXML2MySQL(programId,
                            agrobaseUser,
                            agrobasePassword,
                            LogFile, unRechten.getDBHost(), Bestandsnaam, fileLogId);
                    //Result = PDA2Agrobase.importXMLMySQLxml(programId,
                    //        agrobaseUser,
                    //        agrobasePassword,
                    //        LogFile, unRechten.getDBHost(), Bestandsnaam, fileLogId);
                }

                unLogger.WriteDebug("XML2AGROBASE.Reader           : " + Bestandsnaam + "result: " + Result.ToString());
                if (Result > 0)
                {
                    unLogger.WriteDebug("XML2AGROBASE.Reader          :" + Bestandsnaam + " ingelezen");
                    return 1;
                }
                else
                {
                    unLogger.WriteDebug("XML2AGROBASE.Reader          :" + Bestandsnaam + " niet ingelezen");
                    return -1;
                }
            }
            catch (Exception ex)
            {
                unLogger.WriteError(ex.Message, ex);
                try
                {
                    FILELOG fl = Facade.GetInstance().getSaveToDB(agroFactuurToken).getFileLogById(fileLogId);
                    fl.Errormemo += ex.Message;
                    fl.Errormemo += ex.StackTrace;
                    Facade.GetInstance().getSaveToDB(agroFactuurToken).saveFileLog(fl);
                }
                catch (Exception flex)
                {
                    unLogger.WriteError("Error saving message to filelog table " + flex.Message, flex);
                    return -52;
                }
                return -2;
            }
        }
        public void setSaveToDB(AFSavetoDB value)
        {
            DB = value;
        }

        private int ImportXML2MySQL(int programId, String agrobaseUser, String agrobasePassword, String LogFile, String dbHost, String Bestandsnaam, int fileLogId)
        {
            List<THIRD> relations = new List<THIRD>();
            List<ANIMAL> animals = new List<ANIMAL>();
            List<BUYING> buys = new List<BUYING>(); 
            List<MOVEMENT> movements = new List<MOVEMENT>();
            THIRD bedrijf = null;
            UBN ubn = null;

            int resultCode = 0; 
//            System.Threading.Monitor.Enter(padlck);
            var dierXml = XmlReader.Create(Bestandsnaam);
            while (dierXml.Read())
            {
                if (dierXml.IsStartElement())
                {
                    switch (dierXml.Name.ToString())
                    {
                        case "BEDRIJF":
                            String bedrijfXml = dierXml.ReadOuterXml();
                            bedrijf = ReadBedrijf(bedrijfXml);
                            break;
                        case "UBN": String ubnXml = dierXml.ReadOuterXml();
                            ubn = ReadUbn(ubnXml);
                            break;
                        case "THIRD": String relationXml = dierXml.ReadOuterXml();
                            THIRD relation = ReadRelation(relationXml);
                            relations.Add(relation);
                            break;
                        case "ANIMAL": String animalXml = dierXml.ReadOuterXml();
                            ANIMAL ani = ReadAnimal(animalXml);
                            animals.Add(ani);
                            break;
                        case "MOVEMENT": string movementXml = dierXml.ReadOuterXml();
                            MOVEMENT mov = ReadMovement(movementXml);
                            movements.Add(mov);
                            break;
                        case "BUYING": string buyXml = dierXml.ReadOuterXml();
                            BUYING buy = ReadBuying(buyXml);
                            buys.Add(buy);
                            break;
                    }
                }
            }

            int bedrijfThrId = bedrijf.ThrId;
            int ubnId = DB.getUBNidAndProgIdByUBNNr(ubn.Bedrijfsnummer, out programId);
            Dictionary<int, int> ThrIDs = ConnectABThirdIds(relations, bedrijfThrId);
            ReplaceThrIds(ThrIDs, animals, relations, buys);
            try
            {
                SaveAnimals(animals, movements, ubnId);
                SaveMovements(movements, buys, ubnId);
                SaveBuys(buys);
                resultCode = 1;
            } 
            catch (Exception ex)
            {
                unLogger.WriteError("ImportXML2MySQL: " + ex.Message);
                resultCode = 0; ;  
            } 
            finally
            {
                dierXml.Close();  
//                System.Threading.Monitor.Exit(padlck);
            }
            return resultCode;
        }

        private BUYING ReadBuying(String BuyingNode)
        {
            BUYING buy = new BUYING();
            BUYING existingBuying;

            if (!string.IsNullOrEmpty(BuyingNode))
            {
                XmlReader buyReader = XmlReader.Create(new StringReader(BuyingNode));
                while (buyReader.Read())
                {
                    if (buyReader.IsStartElement())
                    {
                        switch (buyReader.Name.ToString())
                        {
                            case "MovId": buy.MovId = buyReader.ReadElementContentAsInt(); break;
                            case "PurPrice": buy.PurPrice = buyReader.ReadElementContentAsDouble(); break;
                            case "PurWeight": buy.PurWeight = buyReader.ReadElementContentAsDouble(); break;
                            case "PurVersienrPaspoort": buy.PurVersienrPaspoort = buyReader.ReadElementContentAsInt(); break;
                            case "PurHandelaar": buy.PurHandelaar = buyReader.ReadElementContentAsInt(); break;
                        }
                    }
                }
                return buy;
            }
            return null;
        }

        private MOVEMENT ReadMovement(String movementNode) 
        {
            MOVEMENT mov = new MOVEMENT();
            MOVEMENT existingMov;

            if (!string.IsNullOrEmpty(movementNode))
            {
                XmlReader movReader = XmlReader.Create(new StringReader(movementNode));
                while (movReader.Read())
                {
                    if (movReader.IsStartElement())
                    {
                        switch (movReader.Name.ToString()) 
                        {
                            case "AniId": mov.AniId = movReader.ReadElementContentAsInt(); break;
                            case "MovDate": mov.MovDate = movReader.ReadElementContentAsDateTime(); break;
                            case "MovId": mov.MovId = movReader.ReadElementContentAsInt(); break;
                            case "MovKind": mov.MovKind = movReader.ReadElementContentAsInt(); break;
                        }
                    }
                }
                return mov;
            }
            return null;
        }

        private ANIMAL ReadAnimal(String animalNode)
        {
            ANIMAL ani = new ANIMAL();
            ANIMAL existingAnimal;

            if (!string.IsNullOrEmpty(animalNode))
            {
                XmlReader animalReader = XmlReader.Create(new StringReader(animalNode));
                while (animalReader.Read())
                {
                    if (animalReader.IsStartElement())
                    {
                        switch (animalReader.Name.ToString()) 
                        {
                            case "AniId": ani.AniId = animalReader.ReadElementContentAsInt(); break;
                            case "AniCountryCode": ani.AniLifeNumber = animalReader.ReadElementContentAsString(); break;
                            case "AniLifeNumber": ani.AniLifeNumber = ani.AniLifeNumber + " " + animalReader.ReadElementContentAsString(); break;
                            case "AniWorkNumber": ani.AniWorkNumber = animalReader.ReadElementContentAsString(); break;
                            case "AniBirthDate": ani.AniBirthDate = animalReader.ReadElementContentAsDateTime(); break;
                            case "AniSex": ani.AniSex = animalReader.ReadElementContentAsInt(); break;
                            case "AniQuality": ani.AniQuality = animalReader.ReadElementContentAsInt(); break;
                            case "ThrId": ani.ThrId = animalReader.ReadElementContentAsInt(); break;
                            case "HcoId": ani.Anihaircolor = animalReader.ReadElementContentAsInt(); break;
                        }
                    }
                }
                return ani;
            }
            return null;
        }

        private THIRD ReadRelation(String relationNode)
        {
            THIRD relation = new THIRD();
            THIRD ExistingRelation;

            if (!string.IsNullOrEmpty(relationNode))
            {
                XmlReader relationReader = XmlReader.Create(new StringReader(relationNode));
                while (relationReader.Read())
                {
                    if (relationReader.IsStartElement()) 
                    {
                        switch (relationReader.Name.ToString())
                        {
                            case "ThrId": relation.ThrId = relationReader.ReadElementContentAsInt(); break;
                            case "ThrSecondName": relation.ThrSecondName = relationReader.ReadElementContentAsString(); break;
                            case "ThrStreet1": relation.ThrStreet1 = relationReader.ReadElementContentAsString(); break;
                            case "ThrZipCode": relation.ThrZipCode = relationReader.ReadElementContentAsString(); break;
                            case "ThrCity": relation.ThrCity = relationReader.ReadElementContentAsString(); break;
                            case "ThrJuridischNr": relation.ThrVATNumber = relationReader.ReadElementContentAsString(); break;
                        }
                    }
                }
                return relation;
            }
            return null;
        }

        private UBN ReadUbn(String ubnNode)
        {
            UBN ubn = new UBN();
            UBN existingUbn;

            if (!string.IsNullOrEmpty(ubnNode)) 
            { 
                XmlReader ubnReader = XmlReader.Create(new StringReader(ubnNode));
                while (ubnReader.Read())
                {
                    if (ubnReader.IsStartElement()) 
                    {
                        switch (ubnReader.Name.ToString())
                        {
                            case "Bedrijfsnummer": ubn.Bedrijfsnummer = ubnReader.ReadElementContentAsString(); break;
                            case "Bedrijfsnaam": ubn.Bedrijfsnaam = ubnReader.ReadElementContentAsString(); break;
                            case "UBNlong": ubn.UBNlong = ubnReader.ReadElementContentAsString(); break;
                        }
                    }
                }
                return ubn;
            }
            return null;
        }
        private THIRD ReadBedrijf(String bedrijfsNode)
        {
            THIRD bedrijf = new THIRD();
            THIRD existingBedrijf;

            if (!string.IsNullOrEmpty(bedrijfsNode))
            {
                XmlReader bedrijfsReader = XmlReader.Create(new StringReader(bedrijfsNode));
                while (bedrijfsReader.Read())
                {
                    if (bedrijfsReader.IsStartElement())
                    { 
                        switch (bedrijfsReader.Name.ToString())
                        {
                            case "ThrStamboeknr": bedrijf.ThrStamboeknr = bedrijfsReader.ReadElementContentAsString(); break;
                            case "ThrSecondName": bedrijf.ThrSecondName = bedrijfsReader.ReadElementContentAsString(); break;
                            case "ThrStreet1": bedrijf.ThrStreet1 = bedrijfsReader.ReadElementContentAsString(); break;
                            case "ThrZipCode": bedrijf.ThrZipCode = bedrijfsReader.ReadElementContentAsString(); break;
                            case "ThrCity": bedrijf.ThrCity = bedrijfsReader.ReadElementContentAsString(); break;
                            case "ThrPhoneNumber": bedrijf.ThrPhoneNumber = bedrijfsReader.ReadElementContentAsString(); break;
                            case "ThrFarmNumber": bedrijf.ThrFarmNumber = bedrijfsReader.ReadElementContentAsString(); break;
                            case "ThrClientNumber": bedrijf.ThrClientNumber = bedrijfsReader.ReadElementContentAsString(); break;
                            case "ThrCompanyName": bedrijf.ThrCompanyName = bedrijfsReader.ReadElementContentAsString(); break;
                            case "ThrBeslagnr": bedrijf.ThrBeslagnr = bedrijfsReader.ReadElementContentAsString(); break;
                            case "ThrJuridischNr": bedrijf.ThrVATNumber = bedrijfsReader.ReadElementContentAsString(); break;
                         }
                    }
                }
                existingBedrijf = DB.GetThirdByStamboeknr(bedrijf.ThrStamboeknr);
                if (existingBedrijf.ThrId > 0) { return existingBedrijf; }
                else { return bedrijf; }
            }
            return null;
        }

        private Dictionary<int, int> ConnectABThirdIds(List<THIRD> relations, int ownThirdId)
        {
            Dictionary<int, int> thirdIdsPairs = new Dictionary<int, int>();
            foreach (THIRD r in relations)
            {
                if (!thirdIdsPairs.ContainsKey(r.ThrId))
                {
                    THIRD temp = FindThird(r);
                    if (temp != null)
                    {
                        thirdIdsPairs.Add(r.ThrId, temp.ThrId);
                    }
                    else
                    {
                        thirdIdsPairs.Add(r.ThrId, ownThirdId);
                    }
                }
            }
            return thirdIdsPairs;
        }

        private void ReplaceThrIds(Dictionary<int, int> idPairs, List<ANIMAL> animals, List<THIRD> relations, List<BUYING> buys)
        {
            foreach (int k in idPairs.Keys)
            {
                foreach (THIRD r in relations)
                {
                    if (r.ThrId == k) { r.ThrId = idPairs[k]; }
                }

                foreach (ANIMAL a in animals)
                {  
                    if (a.ThrId == k) { a.ThrId = idPairs[k];} 
                }

                foreach (BUYING b in buys)
                {
                    if (b.PurHandelaar == k) { b.PurHandelaar = idPairs[k]; }
                }
            }
        }

        private void SaveAnimals(List<ANIMAL> animals, List<MOVEMENT> movements, int ubnId)
        {
            foreach (ANIMAL a in animals)
            {
                ANIMALCATEGORY aniCategory = new ANIMALCATEGORY();
                
                int oldAniId = a.AniId;
                a.AniId = 0;
                a.AniAlternateNumber = a.AniLifeNumber;
                int newAniId = DB.SaveAnimal(a, 0, 0);


                if (newAniId > 0)
                {
                    aniCategory.AniId = newAniId;
                    aniCategory.UbnId = ubnId;
                    aniCategory.Anicategory = 1;
                    aniCategory.AniWorknumber = a.AniWorkNumber;
                    DB.SaveAnimalCategory(aniCategory);

                    foreach (MOVEMENT movement in movements)
                    {
                        if (movement.AniId == oldAniId) { movement.AniId = newAniId; }
                    }
                }
                else
                {
                    unLogger.WriteInfo($"Animal not saved. ID: {oldAniId}, Lifenumber: {a.AniLifeNumber}");
                }
            }
        }

        private void SaveMovements(List<MOVEMENT> movements, List<BUYING> buys, int ubnId)
        {
            foreach (MOVEMENT m in movements)
            {
                int oldId = m.MovId;
                m.MovId = 0;
                m.UbnId = ubnId;
                int newId = DB.SaveObject(m, fToken);

                if (newId > 0)
                {
                    foreach (BUYING b in buys)
                    {
                        if (b.MovId == oldId) { b.MovId = newId; }
                    }
                }
                else
                {
                    unLogger.WriteInfo($"Movement not saved. ID: {oldId}, AniId: {m.AniId}");
                }
            }
        }

        private void SaveBuys(List<BUYING> buys)
        {
            foreach (BUYING b in buys)
            {
                DB.SaveBuying(b);
            }
        }

        private THIRD FindThird(THIRD tempThird)
        {
            THIRD temp = null;
            temp = DB.GetThirdByAddressZIPCity(tempThird.ThrStreet1, tempThird.ThrZipCode, tempThird.ThrCity);
            if (temp == null || temp.ThrId == 0 ) 
            {
                temp = DB.GetThirdByVatNo(tempThird.ThrVATNumber);
            }
            if (temp != null) { return temp; }
            else { return tempThird; }
        }

    }
}
