
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
	public class TABLE_INFORMATION : DataObject
	{
		public TABLE_INFORMATION() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"Ti_db",
            	"Ti_table",
            	"Ti_date"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesTABLE_INFORMATION
		{  
            public const string Ti_db = "ti_db";
            public const string Ti_table = "ti_table";
            public const string Ti_date = "ti_date";
            public const string Ti_RecNo = "ti_RecNo";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public string Ti_db
	    {
			get
	        {
				return base.Getstring(ColumnNamesTABLE_INFORMATION.Ti_db);
			}
			set
	        {
				base.Setstring(ColumnNamesTABLE_INFORMATION.Ti_db, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public string Ti_table
	    {
			get
	        {
				return base.Getstring(ColumnNamesTABLE_INFORMATION.Ti_table);
			}
			set
	        {
				base.Setstring(ColumnNamesTABLE_INFORMATION.Ti_table, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public DateTime Ti_date
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesTABLE_INFORMATION.Ti_date);
			}
			set
	        {
				base.SetDateTime(ColumnNamesTABLE_INFORMATION.Ti_date, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Ti_RecNo
	    {
			get
	        {
				return base.Getint(ColumnNamesTABLE_INFORMATION.Ti_RecNo);
			}
			set
	        {
				base.Setint(ColumnNamesTABLE_INFORMATION.Ti_RecNo, value);
			}
		}


		#endregion
		
	}
}
