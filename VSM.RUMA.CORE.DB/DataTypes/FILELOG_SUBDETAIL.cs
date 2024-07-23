
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
	public class FILELOG_SUBDETAIL : DataObject
	{
		public FILELOG_SUBDETAIL() : base(Database.agrologs)
		{
			UpdateParams = new String[] 
			{
            	"FlsdId"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesFILELOG_SUBDETAIL
		{  
            public const string FlsdId = "flsdId";
            public const string FldId = "fldId";
            public const string FlsdKey = "flsdKey";
            public const string FlsdValue = "flsdValue";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int FlsdId
	    {
			get
	        {
				return base.Getint(ColumnNamesFILELOG_SUBDETAIL.FlsdId);
			}
			set
	        {
				base.Setint(ColumnNamesFILELOG_SUBDETAIL.FlsdId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int FldId
	    {
			get
	        {
				return base.Getint(ColumnNamesFILELOG_SUBDETAIL.FldId);
			}
			set
	        {
				base.Setint(ColumnNamesFILELOG_SUBDETAIL.FldId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string FlsdKey
	    {
			get
	        {
				return base.Getstring(ColumnNamesFILELOG_SUBDETAIL.FlsdKey);
			}
			set
	        {
				base.Setstring(ColumnNamesFILELOG_SUBDETAIL.FlsdKey, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string FlsdValue
	    {
			get
	        {
				return base.Getstring(ColumnNamesFILELOG_SUBDETAIL.FlsdValue);
			}
			set
	        {
				base.Setstring(ColumnNamesFILELOG_SUBDETAIL.FlsdValue, value);
			}
		}


		#endregion
		
	}
}
