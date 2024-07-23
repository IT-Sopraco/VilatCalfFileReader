
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
	public class RIGHTS_LISTS : DataObject
	{
		public RIGHTS_LISTS() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"ListName"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesRIGHTS_LISTS
		{  
            public const string ListName = "ListName";
            public const string ListText = "ListText";
            public const string LabId = "LabId";
            public const string ListProgramIds = "ListProgramIds";
            public const string ListFarmIds = "ListFarmIds";
            public const string ListDescription = "ListDescription";
            public const string ListUrl = "ListUrl";
            public const string ListLocation = "ListLocation";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public string ListName
	    {
			get
	        {
				return base.Getstring(ColumnNamesRIGHTS_LISTS.ListName);
			}
			set
	        {
				base.Setstring(ColumnNamesRIGHTS_LISTS.ListName, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string ListText
	    {
			get
	        {
				return base.Getstring(ColumnNamesRIGHTS_LISTS.ListText);
			}
			set
	        {
				base.Setstring(ColumnNamesRIGHTS_LISTS.ListText, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int LabId
	    {
			get
	        {
				return base.Getint(ColumnNamesRIGHTS_LISTS.LabId);
			}
			set
	        {
				base.Setint(ColumnNamesRIGHTS_LISTS.LabId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string ListProgramIds
	    {
			get
	        {
				return base.Getstring(ColumnNamesRIGHTS_LISTS.ListProgramIds);
			}
			set
	        {
				base.Setstring(ColumnNamesRIGHTS_LISTS.ListProgramIds, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string ListFarmIds
	    {
			get
	        {
				return base.Getstring(ColumnNamesRIGHTS_LISTS.ListFarmIds);
			}
			set
	        {
				base.Setstring(ColumnNamesRIGHTS_LISTS.ListFarmIds, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string ListDescription
	    {
			get
	        {
				return base.Getstring(ColumnNamesRIGHTS_LISTS.ListDescription);
			}
			set
	        {
				base.Setstring(ColumnNamesRIGHTS_LISTS.ListDescription, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string ListUrl
	    {
			get
	        {
				return base.Getstring(ColumnNamesRIGHTS_LISTS.ListUrl);
			}
			set
	        {
				base.Setstring(ColumnNamesRIGHTS_LISTS.ListUrl, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string ListLocation
	    {
			get
	        {
				return base.Getstring(ColumnNamesRIGHTS_LISTS.ListLocation);
			}
			set
	        {
				base.Setstring(ColumnNamesRIGHTS_LISTS.ListLocation, value);
			}
		}


		#endregion
		
	}
}