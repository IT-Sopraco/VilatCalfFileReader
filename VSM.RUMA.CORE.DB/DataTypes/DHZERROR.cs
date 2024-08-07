
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
	public class DHZERROR : DataObject
	{
		public DHZERROR() : base(Database.animal)
		{
			UpdateParams = new String[] 
			{
            	"FarmNumber",
            	"CountryCode",
            	"LifeNumber",
            	"InsemDate"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesDHZERROR
		{  
            public const string FarmNumber = "FarmNumber";
            public const string CountryCode = "CountryCode";
            public const string LifeNumber = "LifeNumber";
            public const string InsemDate = "InsemDate";
            public const string CountryCodeBull = "CountryCodeBull";
            public const string LifeNumberBull = "LifeNumberBull";
            public const string AInumberBull = "AInumberBull";
            public const string Chargenr = "Chargenr";
            public const string Imported = "Imported";
            public const string InsKind = "InsKind";
            public const string ReportStatus = "ReportStatus";
            public const string Third = "Third";
            public const string Remark = "Remark";
            public const string ReportTo = "ReportTo";
            public const string Error = "Error";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public string FarmNumber
	    {
			get
	        {
				return base.Getstring(ColumnNamesDHZERROR.FarmNumber);
			}
			set
	        {
				base.Setstring(ColumnNamesDHZERROR.FarmNumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public string CountryCode
	    {
			get
	        {
				return base.Getstring(ColumnNamesDHZERROR.CountryCode);
			}
			set
	        {
				base.Setstring(ColumnNamesDHZERROR.CountryCode, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public string LifeNumber
	    {
			get
	        {
				return base.Getstring(ColumnNamesDHZERROR.LifeNumber);
			}
			set
	        {
				base.Setstring(ColumnNamesDHZERROR.LifeNumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public DateTime InsemDate
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesDHZERROR.InsemDate);
			}
			set
	        {
				base.SetDateTime(ColumnNamesDHZERROR.InsemDate, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string CountryCodeBull
	    {
			get
	        {
				return base.Getstring(ColumnNamesDHZERROR.CountryCodeBull);
			}
			set
	        {
				base.Setstring(ColumnNamesDHZERROR.CountryCodeBull, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string LifeNumberBull
	    {
			get
	        {
				return base.Getstring(ColumnNamesDHZERROR.LifeNumberBull);
			}
			set
	        {
				base.Setstring(ColumnNamesDHZERROR.LifeNumberBull, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string AInumberBull
	    {
			get
	        {
				return base.Getstring(ColumnNamesDHZERROR.AInumberBull);
			}
			set
	        {
				base.Setstring(ColumnNamesDHZERROR.AInumberBull, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string Chargenr
	    {
			get
	        {
				return base.Getstring(ColumnNamesDHZERROR.Chargenr);
			}
			set
	        {
				base.Setstring(ColumnNamesDHZERROR.Chargenr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Imported
	    {
			get
	        {
				return base.Getint(ColumnNamesDHZERROR.Imported);
			}
			set
	        {
				base.Setint(ColumnNamesDHZERROR.Imported, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int InsKind
	    {
			get
	        {
				return base.Getint(ColumnNamesDHZERROR.InsKind);
			}
			set
	        {
				base.Setint(ColumnNamesDHZERROR.InsKind, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int ReportStatus
	    {
			get
	        {
				return base.Getint(ColumnNamesDHZERROR.ReportStatus);
			}
			set
	        {
				base.Setint(ColumnNamesDHZERROR.ReportStatus, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Third
	    {
			get
	        {
				return base.Getint(ColumnNamesDHZERROR.Third);
			}
			set
	        {
				base.Setint(ColumnNamesDHZERROR.Third, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Remark
	    {
			get
	        {
				return base.Getint(ColumnNamesDHZERROR.Remark);
			}
			set
	        {
				base.Setint(ColumnNamesDHZERROR.Remark, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int ReportTo
	    {
			get
	        {
				return base.Getint(ColumnNamesDHZERROR.ReportTo);
			}
			set
	        {
				base.Setint(ColumnNamesDHZERROR.ReportTo, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string Error
	    {
			get
	        {
				return base.Getstring(ColumnNamesDHZERROR.Error);
			}
			set
	        {
				base.Setstring(ColumnNamesDHZERROR.Error, value);
			}
		}


		#endregion
		
	}
}
