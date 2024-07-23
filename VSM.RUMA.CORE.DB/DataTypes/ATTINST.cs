
/*
'===============================================================================
'  Generated From - CSharp_VSM_BusinessEntity.vbgen
' 
'  ** IMPORTANT  ** 
'  How to Generate your stored procedures:
' 
'  SQL        = SQL_StoredProcs.vbgen
'  ACCESS     = Access_StoredProcs.vbgen
'  ORACLE     = Oracle_StoredProcs.vbgen
'  FIREBIRD   = FirebirdStoredProcs.vbgen
'  POSTGRESQL = PostgreSQL_StoredProcs.vbgen
'
'  The supporting base class  is in the Architecture directory in "VSM".
'  
'  This object is 'abstract' which means you need to inherit from it to be able
'  to instantiate it.  This is very easilly done. You can override properties and
'  methods in your derived class, this allows you to regenerate this class at any
'  time and not worry about overwriting custom code. 
'
'  NEVER EDIT THIS FILE.
'
'  public class YourObject :  _YourObject
'  {
'
'  }
'
'===============================================================================
*/

// Generated by MyGeneration Version # (1.3.0.3)
using System;
using System.Data;

using System.Collections;
using System.Collections.Specialized;


namespace VSM.RUMA.CORE.DB.DataTypes
{
    [Serializable()]
	public class ATTINST : DataObject
	{
		public ATTINST() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{

			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesATTINST
		{  
            public const string Farmid = "farmid";
            public const string Internalnr = "Internalnr";
            public const string Omschrijving = "Omschrijving";
            public const string Sortering = "Sortering";
            public const string Soortlijst = "Soortlijst";
            public const string InstTocht = "InstTocht";
            public const string DgnTocht1 = "DgnTocht1";
            public const string DgnTocht2 = "DgnTocht2";
            public const string RegelsTocht = "RegelsTocht";
            public const string Inst21Dagen = "Inst21Dagen";
            public const string Dgn21Dagen1 = "Dgn21Dagen1";
            public const string Dgn21Dagen2 = "Dgn21Dagen2";
            public const string Regels21Dagen = "Regels21Dagen";
            public const string InstDracht = "InstDracht";
            public const string DgnDracht1 = "DgnDracht1";
            public const string DgnDracht2 = "DgnDracht2";
            public const string RegelsDracht = "RegelsDracht";
            public const string InstDroog = "InstDroog";
            public const string DgnDroog1 = "DgnDroog1";
            public const string RegelsDroog = "RegelsDroog";
            public const string InstAfkalven = "InstAfkalven";
            public const string DgnAfkalven1 = "DgnAfkalven1";
            public const string RegelsAfkalven = "RegelsAfkalven";
            public const string InstBehandeling = "InstBehandeling";
            public const string DgnBehandeling1 = "DgnBehandeling1";
            public const string DgnBehandeling2 = "DgnBehandeling2";
            public const string RegelsBehandeling = "RegelsBehandeling";
            public const string InvulregelPerDier = "InvulregelPerDier";
            public const string ExtraRegels = "ExtraRegels";
            public const string DgnWachttijd1 = "DgnWachttijd1";
            public const string DgnWachttijd2 = "DgnWachttijd2";
            public const string InstWachttijd = "InstWachttijd";
            public const string RegelsWachttijd = "RegelsWachttijd";
            public const string DgnBekappen1 = "DgnBekappen1";
            public const string DgnBekappen1eKeer = "DgnBekappen1eKeer";
            public const string DgnBekappen2 = "DgnBekappen2";
            public const string DgnBekappen2eKeer = "DgnBekappen2eKeer";
            public const string DgnBekappen3ekeer = "DgnBekappen3ekeer";
            public const string DgnNuka1 = "DgnNuka1";
            public const string DgnNuka2 = "DgnNuka2";
            public const string InstBekappen = "InstBekappen";
            public const string InstNuka = "InstNuka";
            public const string RegelsBekappen = "RegelsBekappen";
            public const string RegelsNuka = "RegelsNuka";
            public const string DgnSpenen1 = "DgnSpenen1";
            public const string DgnSpenen2 = "DgnSpenen2";
            public const string InstSpenen = "InstSpenen";
            public const string RegelsSpenen = "RegelsSpenen";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Farmid
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.Farmid);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.Farmid, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Internalnr
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.Internalnr);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.Internalnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string Omschrijving
	    {
			get
	        {
				return base.Getstring(ColumnNamesATTINST.Omschrijving);
			}
			set
	        {
				base.Setstring(ColumnNamesATTINST.Omschrijving, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Sortering
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.Sortering);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.Sortering, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Soortlijst
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.Soortlijst);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.Soortlijst, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte InstTocht
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesATTINST.InstTocht);
			}
			set
	        {
				base.Setsbyte(ColumnNamesATTINST.InstTocht, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int DgnTocht1
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.DgnTocht1);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.DgnTocht1, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int DgnTocht2
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.DgnTocht2);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.DgnTocht2, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int RegelsTocht
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.RegelsTocht);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.RegelsTocht, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte Inst21Dagen
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesATTINST.Inst21Dagen);
			}
			set
	        {
				base.Setsbyte(ColumnNamesATTINST.Inst21Dagen, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Dgn21Dagen1
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.Dgn21Dagen1);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.Dgn21Dagen1, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Dgn21Dagen2
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.Dgn21Dagen2);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.Dgn21Dagen2, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Regels21Dagen
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.Regels21Dagen);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.Regels21Dagen, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte InstDracht
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesATTINST.InstDracht);
			}
			set
	        {
				base.Setsbyte(ColumnNamesATTINST.InstDracht, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int DgnDracht1
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.DgnDracht1);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.DgnDracht1, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int DgnDracht2
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.DgnDracht2);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.DgnDracht2, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int RegelsDracht
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.RegelsDracht);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.RegelsDracht, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte InstDroog
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesATTINST.InstDroog);
			}
			set
	        {
				base.Setsbyte(ColumnNamesATTINST.InstDroog, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int DgnDroog1
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.DgnDroog1);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.DgnDroog1, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int RegelsDroog
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.RegelsDroog);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.RegelsDroog, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte InstAfkalven
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesATTINST.InstAfkalven);
			}
			set
	        {
				base.Setsbyte(ColumnNamesATTINST.InstAfkalven, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int DgnAfkalven1
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.DgnAfkalven1);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.DgnAfkalven1, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int RegelsAfkalven
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.RegelsAfkalven);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.RegelsAfkalven, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte InstBehandeling
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesATTINST.InstBehandeling);
			}
			set
	        {
				base.Setsbyte(ColumnNamesATTINST.InstBehandeling, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int DgnBehandeling1
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.DgnBehandeling1);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.DgnBehandeling1, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int DgnBehandeling2
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.DgnBehandeling2);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.DgnBehandeling2, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int RegelsBehandeling
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.RegelsBehandeling);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.RegelsBehandeling, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte InvulregelPerDier
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesATTINST.InvulregelPerDier);
			}
			set
	        {
				base.Setsbyte(ColumnNamesATTINST.InvulregelPerDier, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte ExtraRegels
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesATTINST.ExtraRegels);
			}
			set
	        {
				base.Setsbyte(ColumnNamesATTINST.ExtraRegels, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int DgnWachttijd1
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.DgnWachttijd1);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.DgnWachttijd1, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int DgnWachttijd2
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.DgnWachttijd2);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.DgnWachttijd2, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte InstWachttijd
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesATTINST.InstWachttijd);
			}
			set
	        {
				base.Setsbyte(ColumnNamesATTINST.InstWachttijd, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int RegelsWachttijd
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.RegelsWachttijd);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.RegelsWachttijd, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int DgnBekappen1
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.DgnBekappen1);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.DgnBekappen1, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int DgnBekappen1eKeer
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.DgnBekappen1eKeer);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.DgnBekappen1eKeer, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int DgnBekappen2
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.DgnBekappen2);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.DgnBekappen2, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int DgnBekappen2eKeer
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.DgnBekappen2eKeer);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.DgnBekappen2eKeer, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int DgnBekappen3ekeer
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.DgnBekappen3ekeer);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.DgnBekappen3ekeer, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int DgnNuka1
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.DgnNuka1);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.DgnNuka1, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int DgnNuka2
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.DgnNuka2);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.DgnNuka2, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte InstBekappen
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesATTINST.InstBekappen);
			}
			set
	        {
				base.Setsbyte(ColumnNamesATTINST.InstBekappen, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte InstNuka
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesATTINST.InstNuka);
			}
			set
	        {
				base.Setsbyte(ColumnNamesATTINST.InstNuka, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int RegelsBekappen
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.RegelsBekappen);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.RegelsBekappen, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int RegelsNuka
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.RegelsNuka);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.RegelsNuka, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int DgnSpenen1
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.DgnSpenen1);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.DgnSpenen1, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int DgnSpenen2
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.DgnSpenen2);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.DgnSpenen2, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte InstSpenen
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesATTINST.InstSpenen);
			}
			set
	        {
				base.Setsbyte(ColumnNamesATTINST.InstSpenen, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int RegelsSpenen
	    {
			get
	        {
				return base.Getint(ColumnNamesATTINST.RegelsSpenen);
			}
			set
	        {
				base.Setint(ColumnNamesATTINST.RegelsSpenen, value);
			}
		}


		#endregion
		
	}
}