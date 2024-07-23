
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
	public class BEDRIJVEN_SELECTIE_DETAIL : DataObject
	{
		public BEDRIJVEN_SELECTIE_DETAIL() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"BSD_ID",
            	"BSD_FarmID"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesBEDRIJVEN_SELECTIE_DETAIL
		{  
            public const string BSD_ID = "BSD_ID";
            public const string BSD_FarmID = "BSD_FarmID";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int BSD_ID
	    {
			get
	        {
				return base.Getint(ColumnNamesBEDRIJVEN_SELECTIE_DETAIL.BSD_ID);
			}
			set
	        {
				base.Setint(ColumnNamesBEDRIJVEN_SELECTIE_DETAIL.BSD_ID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int BSD_FarmID
	    {
			get
	        {
				return base.Getint(ColumnNamesBEDRIJVEN_SELECTIE_DETAIL.BSD_FarmID);
			}
			set
	        {
				base.Setint(ColumnNamesBEDRIJVEN_SELECTIE_DETAIL.BSD_FarmID, value);
			}
		}


		#endregion
		
	}
}
