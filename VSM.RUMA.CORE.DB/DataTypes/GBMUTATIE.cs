
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
	public class GBMUTATIE : DataObject
	{
		public GBMUTATIE() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"Internalnr"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesGBMUTATIE
		{  
            public const string Internalnr = "Internalnr";
            public const string Farmid = "Farmid";
            public const string GBperceelid = "GBperceelid";
            public const string Begindatum = "Begindatum";
            public const string Gewascode = "Gewascode";
            public const string Gebruikscode = "Gebruikscode";
            public const string Oppervlak = "Oppervlak";
            public const string VorigBRSnummer = "VorigBRSnummer";
            public const string RedenPIPOoverschr = "RedenPIPOoverschr";
            public const string RedenGeenBRSnr = "RedenGeenBRSnr";
            public const string Regelnr = "Regelnr";
            public const string Archive = "Archive";
            public const string WachtrijNummer = "WachtrijNummer";
            public const string ReportDate = "ReportDate";
            public const string ReportTime = "ReportTime";
            public const string TeeltPerceel = "TeeltPerceel";
            public const string GBperceelnr = "GBperceelnr";
            public const string VerwerkStatus = "VerwerkStatus";
            public const string Communicatienr = "Communicatienr";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int Internalnr
	    {
			get
	        {
				return base.Getint(ColumnNamesGBMUTATIE.Internalnr);
			}
			set
	        {
				base.Setint(ColumnNamesGBMUTATIE.Internalnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Farmid
	    {
			get
	        {
				return base.Getint(ColumnNamesGBMUTATIE.Farmid);
			}
			set
	        {
				base.Setint(ColumnNamesGBMUTATIE.Farmid, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int GBperceelid
	    {
			get
	        {
				return base.Getint(ColumnNamesGBMUTATIE.GBperceelid);
			}
			set
	        {
				base.Setint(ColumnNamesGBMUTATIE.GBperceelid, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime Begindatum
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesGBMUTATIE.Begindatum);
			}
			set
	        {
				base.SetDateTime(ColumnNamesGBMUTATIE.Begindatum, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Gewascode
	    {
			get
	        {
				return base.Getint(ColumnNamesGBMUTATIE.Gewascode);
			}
			set
	        {
				base.Setint(ColumnNamesGBMUTATIE.Gewascode, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Gebruikscode
	    {
			get
	        {
				return base.Getint(ColumnNamesGBMUTATIE.Gebruikscode);
			}
			set
	        {
				base.Setint(ColumnNamesGBMUTATIE.Gebruikscode, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double Oppervlak
	    {
			get
	        {
				return base.Getdouble(ColumnNamesGBMUTATIE.Oppervlak);
			}
			set
	        {
				base.Setdouble(ColumnNamesGBMUTATIE.Oppervlak, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string VorigBRSnummer
	    {
			get
	        {
				return base.Getstring(ColumnNamesGBMUTATIE.VorigBRSnummer);
			}
			set
	        {
				base.Setstring(ColumnNamesGBMUTATIE.VorigBRSnummer, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string RedenPIPOoverschr
	    {
			get
	        {
				return base.Getstring(ColumnNamesGBMUTATIE.RedenPIPOoverschr);
			}
			set
	        {
				base.Setstring(ColumnNamesGBMUTATIE.RedenPIPOoverschr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string RedenGeenBRSnr
	    {
			get
	        {
				return base.Getstring(ColumnNamesGBMUTATIE.RedenGeenBRSnr);
			}
			set
	        {
				base.Setstring(ColumnNamesGBMUTATIE.RedenGeenBRSnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Regelnr
	    {
			get
	        {
				return base.Getint(ColumnNamesGBMUTATIE.Regelnr);
			}
			set
	        {
				base.Setint(ColumnNamesGBMUTATIE.Regelnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Archive
	    {
			get
	        {
				return base.Getint(ColumnNamesGBMUTATIE.Archive);
			}
			set
	        {
				base.Setint(ColumnNamesGBMUTATIE.Archive, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int WachtrijNummer
	    {
			get
	        {
				return base.Getint(ColumnNamesGBMUTATIE.WachtrijNummer);
			}
			set
	        {
				base.Setint(ColumnNamesGBMUTATIE.WachtrijNummer, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime ReportDate
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesGBMUTATIE.ReportDate);
			}
			set
	        {
				base.SetDateTime(ColumnNamesGBMUTATIE.ReportDate, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime ReportTime
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesGBMUTATIE.ReportTime);
			}
			set
	        {
				base.SetDateTime(ColumnNamesGBMUTATIE.ReportTime, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string TeeltPerceel
	    {
			get
	        {
				return base.Getstring(ColumnNamesGBMUTATIE.TeeltPerceel);
			}
			set
	        {
				base.Setstring(ColumnNamesGBMUTATIE.TeeltPerceel, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string GBperceelnr
	    {
			get
	        {
				return base.Getstring(ColumnNamesGBMUTATIE.GBperceelnr);
			}
			set
	        {
				base.Setstring(ColumnNamesGBMUTATIE.GBperceelnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int VerwerkStatus
	    {
			get
	        {
				return base.Getint(ColumnNamesGBMUTATIE.VerwerkStatus);
			}
			set
	        {
				base.Setint(ColumnNamesGBMUTATIE.VerwerkStatus, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string Communicatienr
	    {
			get
	        {
				return base.Getstring(ColumnNamesGBMUTATIE.Communicatienr);
			}
			set
	        {
				base.Setstring(ColumnNamesGBMUTATIE.Communicatienr, value);
			}
		}


		#endregion
		
	}
}
