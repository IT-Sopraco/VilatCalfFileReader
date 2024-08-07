
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
	public class SOAP_MELDING_SOORTEN : DataObject
	{
		public SOAP_MELDING_SOORTEN() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"SMS_ID"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesSOAP_MELDING_SOORTEN
		{  
            public const string SMS_ID = "SMS_ID";
            public const string SMS_Kind = "SMS_Kind";
            public const string SMS_Sub_Kind = "SMS_Sub_Kind";
            public const string SMS_Status = "SMS_Status";
            public const string SMS_Code = "SMS_Code";
            public const string SMS_Omschrijving = "SMS_Omschrijving";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int SMS_ID
	    {
			get
	        {
				return base.Getint(ColumnNamesSOAP_MELDING_SOORTEN.SMS_ID);
			}
			set
	        {
				base.Setint(ColumnNamesSOAP_MELDING_SOORTEN.SMS_ID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int SMS_Kind
	    {
			get
	        {
				return base.Getint(ColumnNamesSOAP_MELDING_SOORTEN.SMS_Kind);
			}
			set
	        {
				base.Setint(ColumnNamesSOAP_MELDING_SOORTEN.SMS_Kind, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int SMS_Sub_Kind
	    {
			get
	        {
				return base.Getint(ColumnNamesSOAP_MELDING_SOORTEN.SMS_Sub_Kind);
			}
			set
	        {
				base.Setint(ColumnNamesSOAP_MELDING_SOORTEN.SMS_Sub_Kind, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string SMS_Status
	    {
			get
	        {
				return base.Getstring(ColumnNamesSOAP_MELDING_SOORTEN.SMS_Status);
			}
			set
	        {
				base.Setstring(ColumnNamesSOAP_MELDING_SOORTEN.SMS_Status, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string SMS_Code
	    {
			get
	        {
				return base.Getstring(ColumnNamesSOAP_MELDING_SOORTEN.SMS_Code);
			}
			set
	        {
				base.Setstring(ColumnNamesSOAP_MELDING_SOORTEN.SMS_Code, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string SMS_Omschrijving
	    {
			get
	        {
				return base.Getstring(ColumnNamesSOAP_MELDING_SOORTEN.SMS_Omschrijving);
			}
			set
	        {
				base.Setstring(ColumnNamesSOAP_MELDING_SOORTEN.SMS_Omschrijving, value);
			}
		}


		#endregion
		
	}
}
