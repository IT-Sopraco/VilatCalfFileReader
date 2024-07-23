using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
	public class Win32VKIlijst: Win32
	{
        [ThreadStatic]
		private static Win32VKIlijst singleton;

		public static Win32VKIlijst Init()
		{
			if (singleton == null)
			{
				singleton = new Win32VKIlijst();
			}
			return singleton;
		}

		//public void call_lst_AB_CBDvkiLijst(string UBN, string Naam, string Adres, string Postcode, string Woonplaats, string csvKoppelnrs,
		//                                    string V1, string V2, string V3a, string V3b, string csvV3bKoppOornr, string csvV3Reden,
		//                                    string V3c, string csvV3cKoppOornr, string csvV3cRegNLmiddel, string csvV3cNaamMiddel,
		//                                    string csvV3cDatumBegin, string csvV3cDatumEind, string csvV3cDgnWacht,
		//                                    string csvV3cEindeWacht, string V4, string V5, string V6, string V7, string V8,
		//                                    string DAPnaam, string DAPadres, string DAPpostcode, string DAPWoonplaats, string DAPtelnr,
		//                                    string Aanvulling,
		//                                    string ExportBestand, int Soortbestand)
		//{
		//  lst_AB_CBDvkiLijst handle = (lst_AB_CBDvkiLijst)ExecuteProcedureDLL(typeof(lst_AB_CBDvkiLijst), "lstvkialg.dll", "CBDvkiLijst");

		//  handle(UBN, Naam, Adres, Postcode, Woonplaats, csvKoppelnrs,
		//         V1, V2, V3a, V3b, csvV3bKoppOornr, csvV3Reden,
		//         V3c, csvV3cKoppOornr, csvV3cRegNLmiddel, csvV3cNaamMiddel,
		//         csvV3cDatumBegin, csvV3cDatumEind, csvV3cDgnWacht,
		//         csvV3cEindeWacht, V4, V5, V6, V7, V8,
		//         DAPnaam, DAPadres, DAPpostcode, DAPWoonplaats, DAPtelnr,
		//         Aanvulling,
		//         ExportBestand, Soortbestand);


		//  FreeDLL("lstvkialg.dll");
		//}

		//public delegate void pCallback(int PercDone, string Msg);

		//delegate void lst_AB_CBDvkiLijst(string UBN, string Naam, string Adres, string Postcode, string Woonplaats, string csvKoppelnrs,
		//                                 string V1, string V2, string V3a, string V3b, string csvV3bKoppOornr, string csvV3Reden,
		//                                 string V3c, string csvV3cKoppOornr, string csvV3cRegNLmiddel, string csvV3cNaamMiddel,
		//                                 string csvV3cDatumBegin, string csvV3cDatumEind, string csvV3cDgnWacht,
		//                                 string csvV3cEindeWacht, string V4, string V5, string V6, string V7, string V8,
		//                                 string DAPnaam, string DAPadres, string DAPpostcode, string DAPWoonplaats, string DAPtelnr,
		//                                 string Aanvulling, 
		//                                 string ExportBestand, int Soortbestand);

	}
}
