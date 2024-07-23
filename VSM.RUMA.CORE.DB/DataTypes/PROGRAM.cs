
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
	public class PROGRAM : DataObject
	{
		public PROGRAM() : base(Database.agrolink)
		{
			UpdateParams = new String[] 
			{
            	"ProgId"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesPROGRAM
		{  
            public const string ProgId = "ProgId";
            public const string ProgName = "ProgName";
            public const string ProgDescription = "ProgDescription";
            public const string ProgDatabase = "ProgDatabase";
            public const string TS = "TS";
            public const string INS = "INS";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int ProgId
	    {
			get
	        {
				return base.Getint(ColumnNamesPROGRAM.ProgId);
			}
			set
	        {
				base.Setint(ColumnNamesPROGRAM.ProgId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string ProgName
	    {
			get
	        {
				return base.Getstring(ColumnNamesPROGRAM.ProgName);
			}
			set
	        {
				base.Setstring(ColumnNamesPROGRAM.ProgName, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string ProgDescription
	    {
			get
	        {
				return base.Getstring(ColumnNamesPROGRAM.ProgDescription);
			}
			set
	        {
				base.Setstring(ColumnNamesPROGRAM.ProgDescription, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string ProgDatabase
	    {
			get
	        {
				return base.Getstring(ColumnNamesPROGRAM.ProgDatabase);
			}
			set
	        {
				base.Setstring(ColumnNamesPROGRAM.ProgDatabase, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime TS
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesPROGRAM.TS);
			}
			set
	        {
				base.SetDateTime(ColumnNamesPROGRAM.TS, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime INS
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesPROGRAM.INS);
			}
			set
	        {
				base.SetDateTime(ColumnNamesPROGRAM.INS, value);
			}
		}


		#endregion
		
	}
}
