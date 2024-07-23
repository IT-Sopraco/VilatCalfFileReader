
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
	public class SAVED_VALUES_DETAIL : DataObject
	{
		public SAVED_VALUES_DETAIL() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"SvdId"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesSAVED_VALUES_DETAIL
		{  
            public const string SvdId = "svdId";
            public const string SvId = "svId";
            public const string SvdKey = "svdKey";
            public const string SvdValue = "svdValue";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int SvdId
	    {
			get
	        {
				return base.Getint(ColumnNamesSAVED_VALUES_DETAIL.SvdId);
			}
			set
	        {
				base.Setint(ColumnNamesSAVED_VALUES_DETAIL.SvdId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int SvId
	    {
			get
	        {
				return base.Getint(ColumnNamesSAVED_VALUES_DETAIL.SvId);
			}
			set
	        {
				base.Setint(ColumnNamesSAVED_VALUES_DETAIL.SvId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string SvdKey
	    {
			get
	        {
				return base.Getstring(ColumnNamesSAVED_VALUES_DETAIL.SvdKey);
			}
			set
	        {
				base.Setstring(ColumnNamesSAVED_VALUES_DETAIL.SvdKey, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string SvdValue
	    {
			get
	        {
				return base.Getstring(ColumnNamesSAVED_VALUES_DETAIL.SvdValue);
			}
			set
	        {
				base.Setstring(ColumnNamesSAVED_VALUES_DETAIL.SvdValue, value);
			}
		}


		#endregion
		
	}
}
