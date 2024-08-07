
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
	public class ABILITY_AUTOMATE : DataObject
	{
        public ABILITY_AUTOMATE() : base(Database.agrolink)
        {
            UpdateParams = new String[] 
			{
            	"AaId",
            	"PmId"
			};
		}

	
		#region ColumnNames
		public class ColumnNames
		{  
            public const string AaId = "AaId";
            public const string AaKind = "AaKind";
            public const string AbiId = "AbiId";
            public const string PmId = "PmId";
            public const string AaEmail = "AaEmail";
            public const string AaFtpId = "AaFtpId";
            public const string AaIntervalDay = "AaIntervalDay";
            public const string AaMonthDay = "AaMonthDay";
            public const string AaDay = "AaDay";
            public const string AaLastAutomate = "AaLastAutomate";
            public const string AaTime = "AaTime";


        }
		#endregion

		#region Properties

        [System.ComponentModel.DataObjectField(true, true, false)] 
		public int AaId
	    {
			get
	        {
				return base.Getint(ColumnNames.AaId);
			}
			set
	        {
				base.Setint(ColumnNames.AaId, value);
			}
		}

		public int AaKind
	    {
			get
	        {
				return base.Getint(ColumnNames.AaKind);
			}
			set
	        {
				base.Setint(ColumnNames.AaKind, value);
			}
		}

		public int AbiId
	    {
			get
	        {
				return base.Getint(ColumnNames.AbiId);
			}
			set
	        {
				base.Setint(ColumnNames.AbiId, value);
			}
		}

		public int PmId
	    {
			get
	        {
				return base.Getint(ColumnNames.PmId);
			}
			set
	        {
				base.Setint(ColumnNames.PmId, value);
			}
		}

        public string AaEmail
	    {
			get
	        {
                return base.Getstring(ColumnNames.AaEmail);
			}
			set
	        {
                base.Setstring(ColumnNames.AaEmail, value);
			}
		}

		public int AaFtpId
	    {
			get
	        {
				return base.Getint(ColumnNames.AaFtpId);
			}
			set
	        {
				base.Setint(ColumnNames.AaFtpId, value);
			}
		}

		public int AaIntervalDay
	    {
			get
	        {
				return base.Getint(ColumnNames.AaIntervalDay);
			}
			set
	        {
				base.Setint(ColumnNames.AaIntervalDay, value);
			}
		}

		public int AaMonthDay
	    {
			get
	        {
				return base.Getint(ColumnNames.AaMonthDay);
			}
			set
	        {
				base.Setint(ColumnNames.AaMonthDay, value);
			}
		}

		public int AaDay
	    {
			get
	        {
				return base.Getint(ColumnNames.AaDay);
			}
			set
	        {
				base.Setint(ColumnNames.AaDay, value);
			}
		}

        public DateTime AaLastAutomate
	    {
			get
	        {
                return base.GetDateTime(ColumnNames.AaLastAutomate);
			}
			set
	        {
                base.SetDateTime(ColumnNames.AaLastAutomate, value);
			}
		}

        public TimeSpan AaTime
        {
            get
            {
                return base.GetTimeSpan(ColumnNames.AaTime);
            }
            set
            {
                base.SetTimeSpan(ColumnNames.AaTime, value);
            }
        }
        #endregion

    }
}
