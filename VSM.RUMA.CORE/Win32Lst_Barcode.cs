using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32Lst_Barcode: Win32
    {
        [ThreadStatic]
        private static Win32Lst_Barcode singleton;

        public static Win32Lst_Barcode Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Lst_Barcode();
            }
            return singleton;
        }
        
        public int call_lst_Barcode_Afdrukken(int pProgID, int pProgramID, int pSoortBestand,
                                              string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                              pCallback ReadDataProc,
                                              string pRavBestand, int pZiekteID)
        {
            lock (typeof(Win32Lst_Barcode))
            {
                string sFilename = "Lst_Barcode.dll";

                lst_Barcode_Afdrukken handle = (lst_Barcode_Afdrukken)ExecuteProcedureDLL(typeof(lst_Barcode_Afdrukken), sFilename, "lst_Barcode_Afdrukken");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pRavBestand, pZiekteID);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        delegate int lst_Barcode_Afdrukken(int pProgID, int pProgramID, int pSoortBestand,
                                           string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                           pCallback ReadDataProc,
                                           string pRavBestand, int pZiekteID);                
        
        public delegate void pCallback(int PercDone, string Msg);    
    }
}
