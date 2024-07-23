
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
	public class ADMIN : DataObject
	{
		public ADMIN() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"AdmisId"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesADMIN
		{  
            public const string AdmisId = "AdmisId";
            public const string AdmisNumber = "AdmisNumber";
            public const string AdmisName = "AdmisName";
            public const string AdmisPassword = "AdmisPassword";
            public const string AdmisBtwPlichtig = "AdmisBtwPlichtig";
            public const string AdmisSoortboekhouding = "AdmisSoortboekhouding";
            public const string AdminBatch = "AdminBatch";
            public const string AdmisBedrijfId = "AdmisBedrijfId";
            public const string AdmisFactMemo = "AdmisFactMemo";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int AdmisId
	    {
			get
	        {
				return base.Getint(ColumnNamesADMIN.AdmisId);
			}
			set
	        {
				base.Setint(ColumnNamesADMIN.AdmisId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AdmisNumber
	    {
			get
	        {
				return base.Getint(ColumnNamesADMIN.AdmisNumber);
			}
			set
	        {
				base.Setint(ColumnNamesADMIN.AdmisNumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string AdmisName
	    {
			get
	        {
				return base.Getstring(ColumnNamesADMIN.AdmisName);
			}
			set
	        {
				base.Setstring(ColumnNamesADMIN.AdmisName, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string AdmisPassword
	    {
			get
	        {
				return base.Getstring(ColumnNamesADMIN.AdmisPassword);
			}
			set
	        {
				base.Setstring(ColumnNamesADMIN.AdmisPassword, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte AdmisBtwPlichtig
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesADMIN.AdmisBtwPlichtig);
			}
			set
	        {
				base.Setsbyte(ColumnNamesADMIN.AdmisBtwPlichtig, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AdmisSoortboekhouding
	    {
			get
	        {
				return base.Getint(ColumnNamesADMIN.AdmisSoortboekhouding);
			}
			set
	        {
				base.Setint(ColumnNamesADMIN.AdmisSoortboekhouding, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte AdminBatch
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesADMIN.AdminBatch);
			}
			set
	        {
				base.Setsbyte(ColumnNamesADMIN.AdminBatch, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AdmisBedrijfId
	    {
			get
	        {
				return base.Getint(ColumnNamesADMIN.AdmisBedrijfId);
			}
			set
	        {
				base.Setint(ColumnNamesADMIN.AdmisBedrijfId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string AdmisFactMemo
	    {
			get
	        {
				return base.Getstring(ColumnNamesADMIN.AdmisFactMemo);
			}
			set
	        {
				base.Setstring(ColumnNamesADMIN.AdmisFactMemo, value);
			}
		}


		#endregion
		
	}
}
