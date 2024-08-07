
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
	public class FEED_REST_DETAIL : DataObject
	{
		public FEED_REST_DETAIL() : base(Database.agrodata)
		{
			UpdateParams = new String[] 
			{
            	"FR_ID",
            	"FRD_AB_Feednr"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesFEED_REST_DETAIL
		{  
            public const string FR_ID = "FR_ID";
            public const string FRD_AB_Feednr = "FRD_AB_Feednr";
            public const string FRD_KG_Advice = "FRD_KG_Advice";
            public const string FRD_KG_Rest = "FRD_KG_Rest";
            public const string FRD_KG_Total = "FRD_KG_Total";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int FR_ID
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_REST_DETAIL.FR_ID);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_REST_DETAIL.FR_ID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int FRD_AB_Feednr
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_REST_DETAIL.FRD_AB_Feednr);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_REST_DETAIL.FRD_AB_Feednr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double FRD_KG_Advice
	    {
			get
	        {
				return base.Getdouble(ColumnNamesFEED_REST_DETAIL.FRD_KG_Advice);
			}
			set
	        {
				base.Setdouble(ColumnNamesFEED_REST_DETAIL.FRD_KG_Advice, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double FRD_KG_Rest
	    {
			get
	        {
				return base.Getdouble(ColumnNamesFEED_REST_DETAIL.FRD_KG_Rest);
			}
			set
	        {
				base.Setdouble(ColumnNamesFEED_REST_DETAIL.FRD_KG_Rest, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double FRD_KG_Total
	    {
			get
	        {
				return base.Getdouble(ColumnNamesFEED_REST_DETAIL.FRD_KG_Total);
			}
			set
	        {
				base.Setdouble(ColumnNamesFEED_REST_DETAIL.FRD_KG_Total, value);
			}
		}


		#endregion
		
	}
}
