
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
	public class ANIMAL_DISCUSSION : DataObject
	{
		public ANIMAL_DISCUSSION() : base(Database.animal)
		{
			UpdateParams = new String[] 
			{
            	"AD_ID"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesANIMAL_DISCUSSION
		{  
            public const string AD_ID = "AD_ID";
            public const string AD_ThreadID = "AD_ThreadID";
            public const string AD_TypeID = "AD_TypeID";
            public const string AD_Unique_ID = "AD_Unique_ID";
            public const string ThrID = "ThrID";
            public const string AD_Comment = "AD_Comment";
            public const string TS = "TS";
            public const string INS = "INS";
            public const string AD_Public = "AD_Public";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int AD_ID
	    {
			get
	        {
				return base.Getint(ColumnNamesANIMAL_DISCUSSION.AD_ID);
			}
			set
	        {
				base.Setint(ColumnNamesANIMAL_DISCUSSION.AD_ID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AD_ThreadID
	    {
			get
	        {
				return base.Getint(ColumnNamesANIMAL_DISCUSSION.AD_ThreadID);
			}
			set
	        {
				base.Setint(ColumnNamesANIMAL_DISCUSSION.AD_ThreadID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AD_TypeID
	    {
			get
	        {
				return base.Getint(ColumnNamesANIMAL_DISCUSSION.AD_TypeID);
			}
			set
	        {
				base.Setint(ColumnNamesANIMAL_DISCUSSION.AD_TypeID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AD_Unique_ID
	    {
			get
	        {
				return base.Getint(ColumnNamesANIMAL_DISCUSSION.AD_Unique_ID);
			}
			set
	        {
				base.Setint(ColumnNamesANIMAL_DISCUSSION.AD_Unique_ID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int ThrID
	    {
			get
	        {
				return base.Getint(ColumnNamesANIMAL_DISCUSSION.ThrID);
			}
			set
	        {
				base.Setint(ColumnNamesANIMAL_DISCUSSION.ThrID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string AD_Comment
	    {
			get
	        {
				return base.Getstring(ColumnNamesANIMAL_DISCUSSION.AD_Comment);
			}
			set
	        {
				base.Setstring(ColumnNamesANIMAL_DISCUSSION.AD_Comment, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime TS
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesANIMAL_DISCUSSION.TS);
			}
			set
	        {
				base.SetDateTime(ColumnNamesANIMAL_DISCUSSION.TS, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime INS
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesANIMAL_DISCUSSION.INS);
			}
			set
	        {
				base.SetDateTime(ColumnNamesANIMAL_DISCUSSION.INS, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte AD_Public
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesANIMAL_DISCUSSION.AD_Public);
			}
			set
	        {
				base.Setsbyte(ColumnNamesANIMAL_DISCUSSION.AD_Public, value);
			}
		}


		#endregion
		
	}
}
