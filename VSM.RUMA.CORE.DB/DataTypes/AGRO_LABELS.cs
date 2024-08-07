
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
	public class AGRO_LABELS : DataObject
	{
		public AGRO_LABELS() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"LabKind",
            	"LabProgramID",
            	"LabCountry",
            	"LabProgID",
            	"LabID"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesAGRO_LABELS
		{  
            public const string LabKind = "LabKind";
            public const string LabProgramID = "LabProgramID";
            public const string LabCountry = "LabCountry";
            public const string LabProgID = "LabProgID";
            public const string LabID = "LabID";
            public const string LabLabel = "LabLabel";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int LabKind
	    {
			get
	        {
				return base.Getint(ColumnNamesAGRO_LABELS.LabKind);
			}
			set
	        {
				base.Setint(ColumnNamesAGRO_LABELS.LabKind, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int LabProgramID
	    {
			get
	        {
				return base.Getint(ColumnNamesAGRO_LABELS.LabProgramID);
			}
			set
	        {
				base.Setint(ColumnNamesAGRO_LABELS.LabProgramID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int LabCountry
	    {
			get
	        {
				return base.Getint(ColumnNamesAGRO_LABELS.LabCountry);
			}
			set
	        {
				base.Setint(ColumnNamesAGRO_LABELS.LabCountry, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int LabProgID
	    {
			get
	        {
				return base.Getint(ColumnNamesAGRO_LABELS.LabProgID);
			}
			set
	        {
				base.Setint(ColumnNamesAGRO_LABELS.LabProgID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int LabID
	    {
			get
	        {
				return base.Getint(ColumnNamesAGRO_LABELS.LabID);
			}
			set
	        {
				base.Setint(ColumnNamesAGRO_LABELS.LabID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string LabLabel
	    {
			get
	        {
				return base.Getstring(ColumnNamesAGRO_LABELS.LabLabel);
			}
			set
	        {
				base.Setstring(ColumnNamesAGRO_LABELS.LabLabel, value);
			}
		}


		#endregion
		
	}
}
