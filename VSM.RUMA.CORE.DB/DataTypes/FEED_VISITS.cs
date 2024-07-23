
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
	public class FEED_VISITS : DataObject
	{
		public FEED_VISITS() : base(Database.agrodata)
		{
			UpdateParams = new String[] 
			{
            	"FarmID",
            	"AniID",
            	"FV_DateTime",
            	"FV_AB_Feednr"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesFEED_VISITS
		{  
            public const string FarmID = "FarmID";
            public const string AniID = "AniID";
            public const string FV_AB_Feednr = "FV_AB_Feednr";
            public const string FV_DateTime = "FV_DateTime";
            public const string FV_Value = "FV_Value";
            public const string Ts = "ts";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int FarmID
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_VISITS.FarmID);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_VISITS.FarmID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int AniID
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_VISITS.AniID);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_VISITS.AniID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int FV_AB_Feednr
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_VISITS.FV_AB_Feednr);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_VISITS.FV_AB_Feednr, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public DateTime FV_DateTime
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesFEED_VISITS.FV_DateTime);
			}
			set
	        {
				base.SetDateTime(ColumnNamesFEED_VISITS.FV_DateTime, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte FV_Value
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesFEED_VISITS.FV_Value);
			}
			set
	        {
				base.Setsbyte(ColumnNamesFEED_VISITS.FV_Value, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime Ts
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesFEED_VISITS.Ts);
			}
			set
	        {
				base.SetDateTime(ColumnNamesFEED_VISITS.Ts, value);
			}
		}


		#endregion
		
	}
}