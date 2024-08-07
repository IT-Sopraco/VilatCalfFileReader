
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
	public class FEED_ADVICE_DETAIL : DataObject
	{
		public FEED_ADVICE_DETAIL() : base(Database.animal)
		{
			UpdateParams = new String[] 
			{
            	"FA_ID",
            	"FAD_AB_Feednr"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesFEED_ADVICE_DETAIL
		{  
            public const string FA_ID = "FA_ID";
            public const string FAD_AB_Feednr = "FAD_AB_Feednr";
            public const string FAD_Calculation_Kind = "FAD_Calculation_Kind";
            public const string FAD_KG_Feed_Advice = "FAD_KG_Feed_Advice";
            public const string FAD_KG_Feed_Target = "FAD_KG_Feed_Target";
		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int FA_ID
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_ADVICE_DETAIL.FA_ID);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_ADVICE_DETAIL.FA_ID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int FAD_AB_Feednr
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_ADVICE_DETAIL.FAD_AB_Feednr);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_ADVICE_DETAIL.FAD_AB_Feednr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int FAD_Calculation_Kind
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_ADVICE_DETAIL.FAD_Calculation_Kind);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_ADVICE_DETAIL.FAD_Calculation_Kind, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double FAD_KG_Feed_Advice
	    {
			get
	        {
				return base.Getdouble(ColumnNamesFEED_ADVICE_DETAIL.FAD_KG_Feed_Advice);
			}
			set
	        {
				base.Setdouble(ColumnNamesFEED_ADVICE_DETAIL.FAD_KG_Feed_Advice, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int FAD_KG_Feed_Target
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_ADVICE_DETAIL.FAD_KG_Feed_Target);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_ADVICE_DETAIL.FAD_KG_Feed_Target, value);
			}
		}


		#endregion
		
	}
}
