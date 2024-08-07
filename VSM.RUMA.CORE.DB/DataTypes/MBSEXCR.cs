
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
	public class MBSEXCR : DataObject
	{
		public MBSEXCR() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"Mestnummer",
            	"Jaar"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesMBSEXCR
		{  
            public const string Mestnummer = "Mestnummer";
            public const string Jaar = "Jaar";
            public const string KgNexclCF = "KgNexclCF";
            public const string KgNinclCF = "KgNinclCF";
            public const string KgP2O5exclCF = "KgP2O5exclCF";
            public const string KgP2O5inclCF = "KgP2O5inclCF";
            public const string KgNforfaitair = "KgNforfaitair";
            public const string KgP2O5forfaitair = "KgP2O5forfaitair";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public string Mestnummer
	    {
			get
	        {
				return base.Getstring(ColumnNamesMBSEXCR.Mestnummer);
			}
			set
	        {
				base.Setstring(ColumnNamesMBSEXCR.Mestnummer, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int Jaar
	    {
			get
	        {
				return base.Getint(ColumnNamesMBSEXCR.Jaar);
			}
			set
	        {
				base.Setint(ColumnNamesMBSEXCR.Jaar, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double KgNexclCF
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMBSEXCR.KgNexclCF);
			}
			set
	        {
				base.Setdouble(ColumnNamesMBSEXCR.KgNexclCF, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double KgNinclCF
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMBSEXCR.KgNinclCF);
			}
			set
	        {
				base.Setdouble(ColumnNamesMBSEXCR.KgNinclCF, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double KgP2O5exclCF
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMBSEXCR.KgP2O5exclCF);
			}
			set
	        {
				base.Setdouble(ColumnNamesMBSEXCR.KgP2O5exclCF, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double KgP2O5inclCF
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMBSEXCR.KgP2O5inclCF);
			}
			set
	        {
				base.Setdouble(ColumnNamesMBSEXCR.KgP2O5inclCF, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double KgNforfaitair
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMBSEXCR.KgNforfaitair);
			}
			set
	        {
				base.Setdouble(ColumnNamesMBSEXCR.KgNforfaitair, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double KgP2O5forfaitair
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMBSEXCR.KgP2O5forfaitair);
			}
			set
	        {
				base.Setdouble(ColumnNamesMBSEXCR.KgP2O5forfaitair, value);
			}
		}


		#endregion
		
	}
}
