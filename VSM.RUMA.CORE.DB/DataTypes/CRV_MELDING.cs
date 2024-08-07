
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
	public class CRV_MELDING : DataObject
	{
        public CRV_MELDING()
        {
            UpdateParams = new String[]
            {
               "crv_melding_ID"
            };
        }

	
		#region ColumnNames
		public class ColumnNames
		{  
            public const string Crv_melding_ID = "crv_melding_ID";
            public const string FarmId = "FarmId";
            public const string UBNId = "UBNId";
            public const string AniID = "AniID";
            public const string AniAlternateNumber = "AniAlternateNumber";
            public const string AniWorknumber_OLD = "AniWorknumber_OLD";
            public const string AniWorknumber_NEW = "AniWorknumber_NEW";
            public const string AniName_OLD = "AniName_OLD";
            public const string AniName_NEW = "AniName_NEW";
			public const string Transmitter_CON_OLD = "Transmitter_CON_OLD";
			public const string Transmitter_CON_NEW = "Transmitter_CON_NEW";
			public const string Transmitter_PED_OLD = "Transmitter_PED_OLD";
			public const string Transmitter_PED_NEW = "Transmitter_PED_NEW";
			public const string Cm_Insert_TS = "cm_Insert_TS";
            public const string Cm_Report_TS = "cm_Report_TS";
            public const string Cm_Report_State = "cm_Report_State";

        }
		#endregion

		#region Properties
	
		public int Crv_melding_ID
	    {
			get
	        {
				return base.Getint(ColumnNames.Crv_melding_ID);
			}
			set
	        {
				base.Setint(ColumnNames.Crv_melding_ID, value);
			}
		}

		public int FarmId
	    {
			get
	        {
				return base.Getint(ColumnNames.FarmId);
			}
			set
	        {
				base.Setint(ColumnNames.FarmId, value);
			}
		}

		public int UBNId
	    {
			get
	        {
				return base.Getint(ColumnNames.UBNId);
			}
			set
	        {
				base.Setint(ColumnNames.UBNId, value);
			}
		}

		public int AniID
	    {
			get
	        {
				return base.Getint(ColumnNames.AniID);
			}
			set
	        {
				base.Setint(ColumnNames.AniID, value);
			}
		}

		public string AniAlternateNumber
	    {
			get
	        {
				return base.Getstring(ColumnNames.AniAlternateNumber);
			}
			set
	        {
				base.Setstring(ColumnNames.AniAlternateNumber, value);
			}
		}

		public string AniWorknumber_OLD
	    {
			get
	        {
				return base.Getstring(ColumnNames.AniWorknumber_OLD);
			}
			set
	        {
				base.Setstring(ColumnNames.AniWorknumber_OLD, value);
			}
		}

		public string AniWorknumber_NEW
	    {
			get
	        {
				return base.Getstring(ColumnNames.AniWorknumber_NEW);
			}
			set
	        {
				base.Setstring(ColumnNames.AniWorknumber_NEW, value);
			}
		}

		public string AniName_OLD
	    {
			get
	        {
				return base.Getstring(ColumnNames.AniName_OLD);
			}
			set
	        {
				base.Setstring(ColumnNames.AniName_OLD, value);
			}
		}

		public string AniName_NEW
	    {
			get
	        {
				return base.Getstring(ColumnNames.AniName_NEW);
			}
			set
	        {
				base.Setstring(ColumnNames.AniName_NEW, value);
			}
		}

		public string Transmitter_CON_OLD
		{
			get
			{
				return base.Getstring(ColumnNames.Transmitter_CON_OLD);
			}
			set
			{
				base.Setstring(ColumnNames.Transmitter_CON_OLD, value);
			}
		}

		public string Transmitter_CON_NEW
		{
			get
			{
				return base.Getstring(ColumnNames.Transmitter_CON_NEW);
			}
			set
			{
				base.Setstring(ColumnNames.Transmitter_CON_NEW, value);
			}
		}

		public string Transmitter_PED_OLD
		{
			get
			{
				return base.Getstring(ColumnNames.Transmitter_PED_OLD);
			}
			set
			{
				base.Setstring(ColumnNames.Transmitter_PED_OLD, value);
			}
		}

		public string Transmitter_PED_NEW
		{
			get
			{
				return base.Getstring(ColumnNames.Transmitter_PED_NEW);
			}
			set
			{
				base.Setstring(ColumnNames.Transmitter_PED_NEW, value);
			}
		}

		public DateTime Cm_Insert_TS
	    {
			get
	        {
				return base.GetDateTime(ColumnNames.Cm_Insert_TS);
			}
			set
	        {
				base.SetDateTime(ColumnNames.Cm_Insert_TS, value);
			}
		}

		public DateTime Cm_Report_TS
	    {
			get
	        {
				return base.GetDateTime(ColumnNames.Cm_Report_TS);
			}
			set
	        {
				base.SetDateTime(ColumnNames.Cm_Report_TS, value);
			}
		}

        public int Cm_Report_State
        {
            get
            {
                return base.Getint(ColumnNames.Cm_Report_State);
            }
            set
            {
                base.Setint(ColumnNames.Cm_Report_State, value);
            }
        }

        #endregion

    }
}
