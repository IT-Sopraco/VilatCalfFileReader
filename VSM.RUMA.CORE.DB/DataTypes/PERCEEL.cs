
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
	public class PERCEEL : DataObject
	{
		public PERCEEL() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"Perceelid",
            	"FarmId"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesPERCEEL
		{  
            public const string Perceelid = "Perceelid";
            public const string FarmId = "FarmId";
            public const string Perceelnr = "Perceelnr";
            public const string Perceelnaam = "Perceelnaam";
            public const string Oppervlak = "Oppervlak";
            public const string Grondsoort = "Grondsoort";
            public const string Drainage = "Drainage";
            public const string Nummerint = "Nummerint";
            public const string Nummerstr = "Nummerstr";
            public const string Droogtegevoelig = "Droogtegevoelig";
            public const string PercStatus = "PercStatus";
            public const string PercGebruiksrecht = "PercGebruiksrecht";
            public const string PercOrgPerceelid = "PercOrgPerceelid";
            public const string GBperceelid = "GBperceelid";
            public const string Ruma_PerceelID = "Ruma_PerceelID";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int Perceelid
	    {
			get
	        {
				return base.Getint(ColumnNamesPERCEEL.Perceelid);
			}
			set
	        {
				base.Setint(ColumnNamesPERCEEL.Perceelid, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int FarmId
	    {
			get
	        {
				return base.Getint(ColumnNamesPERCEEL.FarmId);
			}
			set
	        {
				base.Setint(ColumnNamesPERCEEL.FarmId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string Perceelnr
	    {
			get
	        {
				return base.Getstring(ColumnNamesPERCEEL.Perceelnr);
			}
			set
	        {
				base.Setstring(ColumnNamesPERCEEL.Perceelnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string Perceelnaam
	    {
			get
	        {
				return base.Getstring(ColumnNamesPERCEEL.Perceelnaam);
			}
			set
	        {
				base.Setstring(ColumnNamesPERCEEL.Perceelnaam, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double Oppervlak
	    {
			get
	        {
				return base.Getdouble(ColumnNamesPERCEEL.Oppervlak);
			}
			set
	        {
				base.Setdouble(ColumnNamesPERCEEL.Oppervlak, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Grondsoort
	    {
			get
	        {
				return base.Getint(ColumnNamesPERCEEL.Grondsoort);
			}
			set
	        {
				base.Setint(ColumnNamesPERCEEL.Grondsoort, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte Drainage
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesPERCEEL.Drainage);
			}
			set
	        {
				base.Setsbyte(ColumnNamesPERCEEL.Drainage, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Nummerint
	    {
			get
	        {
				return base.Getint(ColumnNamesPERCEEL.Nummerint);
			}
			set
	        {
				base.Setint(ColumnNamesPERCEEL.Nummerint, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string Nummerstr
	    {
			get
	        {
				return base.Getstring(ColumnNamesPERCEEL.Nummerstr);
			}
			set
	        {
				base.Setstring(ColumnNamesPERCEEL.Nummerstr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Droogtegevoelig
	    {
			get
	        {
				return base.Getint(ColumnNamesPERCEEL.Droogtegevoelig);
			}
			set
	        {
				base.Setint(ColumnNamesPERCEEL.Droogtegevoelig, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int PercStatus
	    {
			get
	        {
				return base.Getint(ColumnNamesPERCEEL.PercStatus);
			}
			set
	        {
				base.Setint(ColumnNamesPERCEEL.PercStatus, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int PercGebruiksrecht
	    {
			get
	        {
				return base.Getint(ColumnNamesPERCEEL.PercGebruiksrecht);
			}
			set
	        {
				base.Setint(ColumnNamesPERCEEL.PercGebruiksrecht, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int PercOrgPerceelid
	    {
			get
	        {
				return base.Getint(ColumnNamesPERCEEL.PercOrgPerceelid);
			}
			set
	        {
				base.Setint(ColumnNamesPERCEEL.PercOrgPerceelid, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int GBperceelid
	    {
			get
	        {
				return base.Getint(ColumnNamesPERCEEL.GBperceelid);
			}
			set
	        {
				base.Setint(ColumnNamesPERCEEL.GBperceelid, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Ruma_PerceelID
	    {
			get
	        {
				return base.Getint(ColumnNamesPERCEEL.Ruma_PerceelID);
			}
			set
	        {
				base.Setint(ColumnNamesPERCEEL.Ruma_PerceelID, value);
			}
		}


		#endregion
		
	}
}
