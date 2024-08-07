
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
	public class PROGRAM_PROPERTY : DataObject
	{
		public PROGRAM_PROPERTY() : base(Database.agrolink)
		{
			UpdateParams = new String[] 
			{
            	"ProgId",
            	"ProgKey"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesPROGRAM_PROPERTY
		{  
            public const string ProgId = "ProgId";
            public const string ProgKey = "ProgKey";
            public const string ProgValue = "ProgValue";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int ProgId
	    {
			get
	        {
				return base.Getint(ColumnNamesPROGRAM_PROPERTY.ProgId);
			}
			set
	        {
				base.Setint(ColumnNamesPROGRAM_PROPERTY.ProgId, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public string ProgKey
	    {
			get
	        {
				return base.Getstring(ColumnNamesPROGRAM_PROPERTY.ProgKey);
			}
			set
	        {
				base.Setstring(ColumnNamesPROGRAM_PROPERTY.ProgKey, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string ProgValue
	    {
			get
	        {
				return base.Getstring(ColumnNamesPROGRAM_PROPERTY.ProgValue);
			}
			set
	        {
				base.Setstring(ColumnNamesPROGRAM_PROPERTY.ProgValue, value);
			}
		}


		#endregion
		
	}
}
