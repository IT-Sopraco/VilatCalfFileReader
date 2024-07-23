
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
	public class FEED_ADVICE : DataObject
	{
		public FEED_ADVICE() : base(Database.animal)
		{
			UpdateParams = new String[] 
			{
            	"FA_ID"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesFEED_ADVICE
		{  
            public const string AniID = "AniID";
            public const string FA_ID = "FA_ID";
            public const string FA_DateTime = "FA_DateTime";
		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AniID
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_ADVICE.AniID);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_ADVICE.AniID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int FA_ID
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_ADVICE.FA_ID);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_ADVICE.FA_ID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime FA_DateTime
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesFEED_ADVICE.FA_DateTime);
			}
			set
	        {
				base.SetDateTime(ColumnNamesFEED_ADVICE.FA_DateTime, value);
			}
		}


		#endregion
		
	}
}
