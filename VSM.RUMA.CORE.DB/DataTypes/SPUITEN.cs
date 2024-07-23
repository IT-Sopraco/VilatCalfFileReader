
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
	public class SPUITEN : DataObject
	{
		public SPUITEN() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"SptPerceelid",
            	"SptActid"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesSPUITEN
		{  
            public const string SptPerceelid = "SptPerceelid";
            public const string SptActid = "SptActid";
            public const string SptMiddelid = "SptMiddelid";
            public const string SptHoeveelheidHA = "SptHoeveelheidHA";
            public const string SptEenheid = "SptEenheid";
            public const string SptLitersHA = "SptLitersHA";
            public const string SptWachtTijd = "SptWachtTijd";
            public const string SptSpuitdop = "SptSpuitdop";
            public const string SptDgnHerhalen = "SptDgnHerhalen";
            public const string SptToelatingsnr = "SptToelatingsnr";
            public const string SptKostPerEenheid = "SptKostPerEenheid";
            public const string SptMiddelAfboeken = "SptMiddelAfboeken";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int SptPerceelid
	    {
			get
	        {
				return base.Getint(ColumnNamesSPUITEN.SptPerceelid);
			}
			set
	        {
				base.Setint(ColumnNamesSPUITEN.SptPerceelid, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int SptActid
	    {
			get
	        {
				return base.Getint(ColumnNamesSPUITEN.SptActid);
			}
			set
	        {
				base.Setint(ColumnNamesSPUITEN.SptActid, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int SptMiddelid
	    {
			get
	        {
				return base.Getint(ColumnNamesSPUITEN.SptMiddelid);
			}
			set
	        {
				base.Setint(ColumnNamesSPUITEN.SptMiddelid, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double SptHoeveelheidHA
	    {
			get
	        {
				return base.Getdouble(ColumnNamesSPUITEN.SptHoeveelheidHA);
			}
			set
	        {
				base.Setdouble(ColumnNamesSPUITEN.SptHoeveelheidHA, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int SptEenheid
	    {
			get
	        {
				return base.Getint(ColumnNamesSPUITEN.SptEenheid);
			}
			set
	        {
				base.Setint(ColumnNamesSPUITEN.SptEenheid, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double SptLitersHA
	    {
			get
	        {
				return base.Getdouble(ColumnNamesSPUITEN.SptLitersHA);
			}
			set
	        {
				base.Setdouble(ColumnNamesSPUITEN.SptLitersHA, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int SptWachtTijd
	    {
			get
	        {
				return base.Getint(ColumnNamesSPUITEN.SptWachtTijd);
			}
			set
	        {
				base.Setint(ColumnNamesSPUITEN.SptWachtTijd, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string SptSpuitdop
	    {
			get
	        {
				return base.Getstring(ColumnNamesSPUITEN.SptSpuitdop);
			}
			set
	        {
				base.Setstring(ColumnNamesSPUITEN.SptSpuitdop, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int SptDgnHerhalen
	    {
			get
	        {
				return base.Getint(ColumnNamesSPUITEN.SptDgnHerhalen);
			}
			set
	        {
				base.Setint(ColumnNamesSPUITEN.SptDgnHerhalen, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string SptToelatingsnr
	    {
			get
	        {
				return base.Getstring(ColumnNamesSPUITEN.SptToelatingsnr);
			}
			set
	        {
				base.Setstring(ColumnNamesSPUITEN.SptToelatingsnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double SptKostPerEenheid
	    {
			get
	        {
				return base.Getdouble(ColumnNamesSPUITEN.SptKostPerEenheid);
			}
			set
	        {
				base.Setdouble(ColumnNamesSPUITEN.SptKostPerEenheid, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int SptMiddelAfboeken
	    {
			get
	        {
				return base.Getint(ColumnNamesSPUITEN.SptMiddelAfboeken);
			}
			set
	        {
				base.Setint(ColumnNamesSPUITEN.SptMiddelAfboeken, value);
			}
		}


		#endregion
		
	}
}
