
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
	public class FEED_IN_FEEDCOMPUTER : DataObject
	{
		public FEED_IN_FEEDCOMPUTER() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"FarmID",
            	"FIF_AB_Feednr"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesFEED_IN_FEEDCOMPUTER
		{  
            public const string FarmID = "farmID";
            public const string FIF_AB_Feednr = "FIF_AB_Feednr";
            public const string FIF_Feedcomputer_Nr = "FIF_Feedcomputer_Nr";
            public const string FIF_Feedcomputer_Feednr = "FIF_Feedcomputer_Feednr";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int FarmID
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_IN_FEEDCOMPUTER.FarmID);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_IN_FEEDCOMPUTER.FarmID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int FIF_AB_Feednr
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_IN_FEEDCOMPUTER.FIF_AB_Feednr);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_IN_FEEDCOMPUTER.FIF_AB_Feednr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int FIF_Feedcomputer_Nr
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_IN_FEEDCOMPUTER.FIF_Feedcomputer_Nr);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_IN_FEEDCOMPUTER.FIF_Feedcomputer_Nr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int FIF_Feedcomputer_Feednr
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_IN_FEEDCOMPUTER.FIF_Feedcomputer_Feednr);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_IN_FEEDCOMPUTER.FIF_Feedcomputer_Feednr, value);
			}
		}


		#endregion
		
	}
}
