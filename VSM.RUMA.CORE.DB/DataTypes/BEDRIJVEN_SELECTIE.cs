
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
	public class BEDRIJVEN_SELECTIE : DataObject
	{
		public BEDRIJVEN_SELECTIE() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"BS_ID"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesBEDRIJVEN_SELECTIE
		{  
            public const string BS_ID = "BS_ID";
            public const string BS_ProgramID = "BS_ProgramID";
            public const string BS_Name = "BS_Name";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int BS_ID
	    {
			get
	        {
				return base.Getint(ColumnNamesBEDRIJVEN_SELECTIE.BS_ID);
			}
			set
	        {
				base.Setint(ColumnNamesBEDRIJVEN_SELECTIE.BS_ID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int BS_ProgramID
	    {
			get
	        {
				return base.Getint(ColumnNamesBEDRIJVEN_SELECTIE.BS_ProgramID);
			}
			set
	        {
				base.Setint(ColumnNamesBEDRIJVEN_SELECTIE.BS_ProgramID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string BS_Name
	    {
			get
	        {
				return base.Getstring(ColumnNamesBEDRIJVEN_SELECTIE.BS_Name);
			}
			set
	        {
				base.Setstring(ColumnNamesBEDRIJVEN_SELECTIE.BS_Name, value);
			}
		}


		#endregion
		
	}
}
