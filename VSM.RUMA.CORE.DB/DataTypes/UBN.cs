
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
using MySql.Data.Types;


namespace VSM.RUMA.CORE.DB.DataTypes
{
	[Serializable()]
	public class UBN : DataObject
	{
		public UBN() : base(Database.agrofactuur)
		{
			UpdateParams = new String[]
			{
				"UBNid"
			};

		}


		#region ColumnNames
		public class ColumnNamesUBN
		{
			public const string UBNid = "UBNid";
			public const string Bedrijfsnummer = "Bedrijfsnummer";
			public const string Bedrijfsnaam = "Bedrijfsnaam";
			public const string UBNlong = "UBNlong";
			public const string BRSnummer = "BRSnummer";
			public const string ThrID = "ThrID";
			public const string Extranummer1 = "Extranummer1";
			public const string ins = "ins";

		}
		#endregion




		#region Properties

		[System.ComponentModel.DataObjectField(true, true, false)]
		public int UBNid
		{
			get
			{
				return base.Getint(ColumnNamesUBN.UBNid);
			}
			set
			{
				base.Setint(ColumnNamesUBN.UBNid, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)]
		public string Bedrijfsnummer
		{
			get
			{
				return base.Getstring(ColumnNamesUBN.Bedrijfsnummer);
			}
			set
			{
				base.Setstring(ColumnNamesUBN.Bedrijfsnummer, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)]
		public string Bedrijfsnaam
		{
			get
			{
				return base.Getstring(ColumnNamesUBN.Bedrijfsnaam);
			}
			set
			{
				base.Setstring(ColumnNamesUBN.Bedrijfsnaam, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)]
		public string UBNlong
		{
			get
			{
				return base.Getstring(ColumnNamesUBN.UBNlong);
			}
			set
			{
				base.Setstring(ColumnNamesUBN.UBNlong, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)]
		public string BRSnummer
		{
			get
			{
				return base.Getstring(ColumnNamesUBN.BRSnummer);
			}
			set
			{
				base.Setstring(ColumnNamesUBN.BRSnummer, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)]
		public int ThrID
		{
			get
			{
				return base.Getint(ColumnNamesUBN.ThrID);
			}
			set
			{
				base.Setint(ColumnNamesUBN.ThrID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)]
		public string Extranummer1
		{
			get
			{
				return base.Getstring(ColumnNamesUBN.Extranummer1);
			}
			set
			{
				base.Setstring(ColumnNamesUBN.Extranummer1, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)]
		public DateTime ins
		{
			get 
			{ 
				return base.GetDateTime(ColumnNamesUBN.ins); 
			}
			set 
			{ 
			//	base.SetDateTime(ColumnNamesUBN.ins, value);
			}
		}
        

		#endregion
		
	}
}
