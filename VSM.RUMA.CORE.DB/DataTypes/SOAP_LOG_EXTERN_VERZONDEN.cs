
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
	public class SOAP_LOG_EXTERN_VERZONDEN : DataObject
	{
		public SOAP_LOG_EXTERN_VERZONDEN() : base(Database.animal)
		{
			UpdateParams = new String[] 
			{
            	"SLEV_ID"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesSOAP_LOG_EXTERN_VERZONDEN
		{  
            public const string SLEV_ID = "SLEV_ID";
            public const string SLEV_Extern_ID = "SLEV_Extern_ID";
            public const string SLEV_DateTime = "SLEV_DateTime";
            public const string SLEV_Lifenr = "SLEV_Lifenr";
            public const string SLEV_Farmnnumber = "SLEV_Farmnnumber";
            public const string SLEV_Meldnr = "SLEV_Meldnr";
            public const string SMS_ID = "SMS_ID";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int SLEV_ID
	    {
			get
	        {
				return base.Getint(ColumnNamesSOAP_LOG_EXTERN_VERZONDEN.SLEV_ID);
			}
			set
	        {
				base.Setint(ColumnNamesSOAP_LOG_EXTERN_VERZONDEN.SLEV_ID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int SLEV_Extern_ID
	    {
			get
	        {
				return base.Getint(ColumnNamesSOAP_LOG_EXTERN_VERZONDEN.SLEV_Extern_ID);
			}
			set
	        {
				base.Setint(ColumnNamesSOAP_LOG_EXTERN_VERZONDEN.SLEV_Extern_ID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime SLEV_DateTime
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesSOAP_LOG_EXTERN_VERZONDEN.SLEV_DateTime);
			}
			set
	        {
				base.SetDateTime(ColumnNamesSOAP_LOG_EXTERN_VERZONDEN.SLEV_DateTime, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string SLEV_Lifenr
	    {
			get
	        {
				return base.Getstring(ColumnNamesSOAP_LOG_EXTERN_VERZONDEN.SLEV_Lifenr);
			}
			set
	        {
				base.Setstring(ColumnNamesSOAP_LOG_EXTERN_VERZONDEN.SLEV_Lifenr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string SLEV_Farmnnumber
	    {
			get
	        {
				return base.Getstring(ColumnNamesSOAP_LOG_EXTERN_VERZONDEN.SLEV_Farmnnumber);
			}
			set
	        {
				base.Setstring(ColumnNamesSOAP_LOG_EXTERN_VERZONDEN.SLEV_Farmnnumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string SLEV_Meldnr
	    {
			get
	        {
				return base.Getstring(ColumnNamesSOAP_LOG_EXTERN_VERZONDEN.SLEV_Meldnr);
			}
			set
	        {
				base.Setstring(ColumnNamesSOAP_LOG_EXTERN_VERZONDEN.SLEV_Meldnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int SMS_ID
	    {
			get
	        {
				return base.Getint(ColumnNamesSOAP_LOG_EXTERN_VERZONDEN.SMS_ID);
			}
			set
	        {
				base.Setint(ColumnNamesSOAP_LOG_EXTERN_VERZONDEN.SMS_ID, value);
			}
		}


		#endregion
		
	}
}