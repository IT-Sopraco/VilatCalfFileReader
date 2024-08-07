
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
	public class AUTH_GROUPS_RIGHTS : DataObject
	{
		public AUTH_GROUPS_RIGHTS() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{

			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesAUTH_GROUPS_RIGHTS
		{  
            public const string ModuleID = "ModuleID";
            public const string ObjectID = "ObjectID";
            public const string Object_Type = "Object_Type";
            public const string GroupID = "GroupID";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int ModuleID
	    {
			get
	        {
				return base.Getint(ColumnNamesAUTH_GROUPS_RIGHTS.ModuleID);
			}
			set
	        {
				base.Setint(ColumnNamesAUTH_GROUPS_RIGHTS.ModuleID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string ObjectID
	    {
			get
	        {
				return base.Getstring(ColumnNamesAUTH_GROUPS_RIGHTS.ObjectID);
			}
			set
	        {
				base.Setstring(ColumnNamesAUTH_GROUPS_RIGHTS.ObjectID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte Object_Type
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesAUTH_GROUPS_RIGHTS.Object_Type);
			}
			set
	        {
				base.Setsbyte(ColumnNamesAUTH_GROUPS_RIGHTS.Object_Type, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int GroupID
	    {
			get
	        {
				return base.Getint(ColumnNamesAUTH_GROUPS_RIGHTS.GroupID);
			}
			set
	        {
				base.Setint(ColumnNamesAUTH_GROUPS_RIGHTS.GroupID, value);
			}
		}


		#endregion
		
	}
}
