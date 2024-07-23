
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
	public class FILELOG_IDFIELD : DataObject
	{
		public FILELOG_IDFIELD() : base(Database.agrologs)
		{
			UpdateParams = new String[] 
			{
            	"Filelog_idfield_id"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesFILELOG_IDFIELD
		{  
            public const string Filelog_idfield_id = "filelog_idfield_id";
            public const string Dbname = "dbname";
            public const string Tablename = "tablename";
            public const string Fieldname_id = "fieldname_id";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int Filelog_idfield_id
	    {
			get
	        {
				return base.Getint(ColumnNamesFILELOG_IDFIELD.Filelog_idfield_id);
			}
			set
	        {
				base.Setint(ColumnNamesFILELOG_IDFIELD.Filelog_idfield_id, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string Dbname
	    {
			get
	        {
				return base.Getstring(ColumnNamesFILELOG_IDFIELD.Dbname);
			}
			set
	        {
				base.Setstring(ColumnNamesFILELOG_IDFIELD.Dbname, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string Tablename
	    {
			get
	        {
				return base.Getstring(ColumnNamesFILELOG_IDFIELD.Tablename);
			}
			set
	        {
				base.Setstring(ColumnNamesFILELOG_IDFIELD.Tablename, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string Fieldname_id
	    {
			get
	        {
				return base.Getstring(ColumnNamesFILELOG_IDFIELD.Fieldname_id);
			}
			set
	        {
				base.Setstring(ColumnNamesFILELOG_IDFIELD.Fieldname_id, value);
			}
		}


		#endregion
		
	}
}
