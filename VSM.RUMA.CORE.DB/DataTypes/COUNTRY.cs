
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
	public class COUNTRY : DataObject
	{
		public COUNTRY() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"LandId"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesCOUNTRY
		{  
            public const string LandId = "LandId";
            public const string LandNaam = "LandNaam";
            public const string LandAfk2 = "LandAfk2";
            public const string LandAfk3 = "LandAfk3";
            public const string Land_Soort = "Land_Soort";
            public const string Land_Opmerking = "Land_Opmerking";
            public const string Land_Tel_Prefix = "Land_Tel_Prefix";
            public const string LandNummer = "LandNummer";
            public const string Land_EAN13 = "Land_EAN13";
            public const string LandNaam_ENG = "LandNaam_ENG";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int LandId
	    {
			get
	        {
				return base.Getint(ColumnNamesCOUNTRY.LandId);
			}
			set
	        {
				base.Setint(ColumnNamesCOUNTRY.LandId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string LandNaam
	    {
			get
	        {
				return base.Getstring(ColumnNamesCOUNTRY.LandNaam);
			}
			set
	        {
				base.Setstring(ColumnNamesCOUNTRY.LandNaam, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string LandAfk2
	    {
			get
	        {
				return base.Getstring(ColumnNamesCOUNTRY.LandAfk2);
			}
			set
	        {
				base.Setstring(ColumnNamesCOUNTRY.LandAfk2, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string LandAfk3
	    {
			get
	        {
				return base.Getstring(ColumnNamesCOUNTRY.LandAfk3);
			}
			set
	        {
				base.Setstring(ColumnNamesCOUNTRY.LandAfk3, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte Land_Soort
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesCOUNTRY.Land_Soort);
			}
			set
	        {
				base.Setsbyte(ColumnNamesCOUNTRY.Land_Soort, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string Land_Opmerking
	    {
			get
	        {
				return base.Getstring(ColumnNamesCOUNTRY.Land_Opmerking);
			}
			set
	        {
				base.Setstring(ColumnNamesCOUNTRY.Land_Opmerking, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string Land_Tel_Prefix
	    {
			get
	        {
				return base.Getstring(ColumnNamesCOUNTRY.Land_Tel_Prefix);
			}
			set
	        {
				base.Setstring(ColumnNamesCOUNTRY.Land_Tel_Prefix, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int LandNummer
	    {
			get
	        {
				return base.Getint(ColumnNamesCOUNTRY.LandNummer);
			}
			set
	        {
				base.Setint(ColumnNamesCOUNTRY.LandNummer, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Land_EAN13
	    {
			get
	        {
				return base.Getint(ColumnNamesCOUNTRY.Land_EAN13);
			}
			set
	        {
				base.Setint(ColumnNamesCOUNTRY.Land_EAN13, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string LandNaam_ENG
	    {
			get
	        {
				return base.Getstring(ColumnNamesCOUNTRY.LandNaam_ENG);
			}
			set
	        {
				base.Setstring(ColumnNamesCOUNTRY.LandNaam_ENG, value);
			}
		}


		#endregion
		
	}
}
