
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
	public class FEED_ADVICE_ROVECOM : DataObject
	{
		public FEED_ADVICE_ROVECOM() : base(Database.animal)
		{
			UpdateParams = new String[] 
			{
            	"UbnID",
            	"AniID",
            	"FAR_Rovecom_Feednr"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesFEED_ADVICE_ROVECOM
		{  
            public const string UbnID = "UbnID";
            public const string AniID = "AniID";
            public const string FAR_Rovecom_Feednr = "FAR_Rovecom_Feednr";
            public const string FAR_Kgfeed = "FAR_Kgfeed";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int UbnID
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_ADVICE_ROVECOM.UbnID);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_ADVICE_ROVECOM.UbnID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int AniID
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_ADVICE_ROVECOM.AniID);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_ADVICE_ROVECOM.AniID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int FAR_Rovecom_Feednr
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_ADVICE_ROVECOM.FAR_Rovecom_Feednr);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_ADVICE_ROVECOM.FAR_Rovecom_Feednr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double FAR_Kgfeed
	    {
			get
	        {
				return base.Getdouble(ColumnNamesFEED_ADVICE_ROVECOM.FAR_Kgfeed);
			}
			set
	        {
				base.Setdouble(ColumnNamesFEED_ADVICE_ROVECOM.FAR_Kgfeed, value);
			}
		}


		#endregion
		
	}
}