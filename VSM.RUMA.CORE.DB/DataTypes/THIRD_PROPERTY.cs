
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
	public class THIRD_PROPERTY : DataObject
	{
		public THIRD_PROPERTY() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"ThrID",
            	"TP_Key"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesTHIRD_PROPERTY
		{  
            public const string ThrID = "ThrID";
            public const string TP_Key = "TP_Key";
            public const string TP_Value = "TP_Value";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int ThrID
	    {
			get
	        {
				return base.Getint(ColumnNamesTHIRD_PROPERTY.ThrID);
			}
			set
	        {
				base.Setint(ColumnNamesTHIRD_PROPERTY.ThrID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public string TP_Key
	    {
			get
	        {
				return base.Getstring(ColumnNamesTHIRD_PROPERTY.TP_Key);
			}
			set
	        {
				base.Setstring(ColumnNamesTHIRD_PROPERTY.TP_Key, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string TP_Value
	    {
			get
	        {
				return base.Getstring(ColumnNamesTHIRD_PROPERTY.TP_Value);
			}
			set
	        {
				base.Setstring(ColumnNamesTHIRD_PROPERTY.TP_Value, value);
			}
		}


		#endregion
		
	}
}
