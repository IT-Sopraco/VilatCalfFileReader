
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
	public class TEMPORARY_IMPORT_TABLE : DataObject
	{
		public TEMPORARY_IMPORT_TABLE() : base(Database.agrologs)
		{
			UpdateParams = new String[] 
			{

			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesTEMPORARY_IMPORT_TABLE
		{  
            public const string Tit_XML_Name = "tit_XML_Name";
            public const string XML_EVENTID = "XML_EVENTID";
            public const string XML_MOVEMENTID = "XML_MOVEMENTID";
            public const string AGROBASE_ID = "AGROBASE_ID";
            public const string AGROBASE_AniID = "AGROBASE_AniID";
            public const string AGROBASE_ALTERNATENUMBER = "AGROBASE_ALTERNATENUMBER";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string Tit_XML_Name
	    {
			get
	        {
				return base.Getstring(ColumnNamesTEMPORARY_IMPORT_TABLE.Tit_XML_Name);
			}
			set
	        {
				base.Setstring(ColumnNamesTEMPORARY_IMPORT_TABLE.Tit_XML_Name, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int XML_EVENTID
	    {
			get
	        {
				return base.Getint(ColumnNamesTEMPORARY_IMPORT_TABLE.XML_EVENTID);
			}
			set
	        {
				base.Setint(ColumnNamesTEMPORARY_IMPORT_TABLE.XML_EVENTID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int XML_MOVEMENTID
	    {
			get
	        {
				return base.Getint(ColumnNamesTEMPORARY_IMPORT_TABLE.XML_MOVEMENTID);
			}
			set
	        {
				base.Setint(ColumnNamesTEMPORARY_IMPORT_TABLE.XML_MOVEMENTID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AGROBASE_ID
	    {
			get
	        {
				return base.Getint(ColumnNamesTEMPORARY_IMPORT_TABLE.AGROBASE_ID);
			}
			set
	        {
				base.Setint(ColumnNamesTEMPORARY_IMPORT_TABLE.AGROBASE_ID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AGROBASE_AniID
	    {
			get
	        {
				return base.Getint(ColumnNamesTEMPORARY_IMPORT_TABLE.AGROBASE_AniID);
			}
			set
	        {
				base.Setint(ColumnNamesTEMPORARY_IMPORT_TABLE.AGROBASE_AniID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string AGROBASE_ALTERNATENUMBER
	    {
			get
	        {
				return base.Getstring(ColumnNamesTEMPORARY_IMPORT_TABLE.AGROBASE_ALTERNATENUMBER);
			}
			set
	        {
				base.Setstring(ColumnNamesTEMPORARY_IMPORT_TABLE.AGROBASE_ALTERNATENUMBER, value);
			}
		}


		#endregion
		
	}
}
