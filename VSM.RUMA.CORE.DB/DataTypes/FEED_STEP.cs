
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
	public class FEED_STEP : DataObject
	{
		public FEED_STEP() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"FarmID",
            	"FS_AB_Feednr",
            	"FS_Production_State"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesFEED_STEP
		{  
            public const string FarmID = "farmID";
            public const string FS_AB_Feednr = "FS_AB_Feednr";
            public const string FS_Production_State = "FS_Production_State";
            public const string FS_KG_Max_Rise = "FS_KG_Max_Rise";
            public const string FS_KG_Max_Decent = "FS_KG_Max_Decent";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int FarmID
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_STEP.FarmID);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_STEP.FarmID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int FS_AB_Feednr
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_STEP.FS_AB_Feednr);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_STEP.FS_AB_Feednr, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int FS_Production_State
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_STEP.FS_Production_State);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_STEP.FS_Production_State, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double FS_KG_Max_Rise
	    {
			get
	        {
				return base.Getdouble(ColumnNamesFEED_STEP.FS_KG_Max_Rise);
			}
			set
	        {
				base.Setdouble(ColumnNamesFEED_STEP.FS_KG_Max_Rise, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double FS_KG_Max_Decent
	    {
			get
	        {
				return base.Getdouble(ColumnNamesFEED_STEP.FS_KG_Max_Decent);
			}
			set
	        {
				base.Setdouble(ColumnNamesFEED_STEP.FS_KG_Max_Decent, value);
			}
		}


		#endregion
		
	}
}
