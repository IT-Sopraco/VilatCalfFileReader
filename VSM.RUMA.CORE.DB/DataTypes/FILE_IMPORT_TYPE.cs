
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
	public class FILE_IMPORT_TYPE : DataObject
	{
		public FILE_IMPORT_TYPE() : base(Database.agrodata)
        {
            UpdateParams = new String[]
              {
                "File_Import_Type_ID"
              };

        }

	
		#region ColumnNames
		public class ColumnNames
		{  
            public const string File_Import_Type_ID = "File_Import_Type_ID";
            public const string FIT_Description = "FIT_Description";
            public const string FIT_Column_JSON = "FIT_Column_JSON";
            public const string TS = "TS";
            public const string Changed_By = "Changed_By";
            public const string SourceID = "SourceID";
            public const string ActionType = "ActionType";
 

		}
        #endregion

        #region Properties

        [System.ComponentModel.DataObjectField(true, true, false)]
        public int File_Import_Type_ID
	    {
			get
	        {
				return base.Getint(ColumnNames.File_Import_Type_ID);
			}
			set
	        {
				base.Setint(ColumnNames.File_Import_Type_ID, value);
			}
		}

        [System.ComponentModel.DataObjectField(false, false, false)]
        public string FIT_Description
	    {
			get
	        {
				return base.Getstring(ColumnNames.FIT_Description);
			}
			set
	        {
				base.Setstring(ColumnNames.FIT_Description, value);
			}
		}

        [System.ComponentModel.DataObjectField(false, false, false)]
        public string FIT_Column_JSON
	    {
			get
	        {
				return base.Getstring(ColumnNames.FIT_Column_JSON);
			}
			set
	        {
				base.Setstring(ColumnNames.FIT_Column_JSON, value);
			}
		}

        [System.ComponentModel.DataObjectField(false, false, false)]
        public DateTime TS
	    {
			get
	        {
				return base.GetDateTime(ColumnNames.TS);
			}
			set
	        {
				base.SetDateTime(ColumnNames.TS, value);
			}
		}

        [System.ComponentModel.DataObjectField(false, false, false)]
        public int ActionType
	    {
			get
	        {
				return base.Getint(ColumnNames.ActionType);
			}
			set
	        {
				base.Setint(ColumnNames.ActionType, value);
			}
		}


		#endregion
		
	}
}
