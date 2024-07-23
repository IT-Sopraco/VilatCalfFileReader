using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
	public class Win32lstvkialg: Win32
	{
        [ThreadStatic]
        private static Win32lstvkialg singleton;

        public static Win32lstvkialg Init()
        {
            if (singleton == null)
            {
                singleton = new Win32lstvkialg();
            }
            return singleton;
        }

        public Win32lstvkialg()
            : base(false)
        {
        }
        /*
         function lst_AB_VKIalgemeen(pProgID, pProgramID, pSoortBestand: Integer;
                            pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                            pCallBack: Pointer;
                            vkiId: Integer; pHoknr: PAnsiChar): Integer; stdcall; 
         */
        public int call_lst_AB_VKIalgemeen(int pProgID, int pProgramID, int pSoortBestand,
										   string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
										   pCallback ReadDataProc,
                                           int vkiId, string pHoknr)
		{
            lock (typeof(Win32lstvkialg))
            {
                lst_AB_VKIalgemeen handle = (lst_AB_VKIalgemeen)ExecuteProcedureDLL(typeof(lst_AB_VKIalgemeen), "lstvkialg.dll", "lst_AB_VKIalgemeen");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc, vkiId, pHoknr);

                FreeDLL("lstvkialg.dll");
                return tmp;
            }
		}


		public delegate void pCallback(int PercDone, string Msg);

		delegate int lst_AB_VKIalgemeen(int pProgID, int pProgramID, int pSoortBestand,
										 string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
										 pCallback ReadDataProc,
                                         int vkiId, string pHoknr);
	}
}
