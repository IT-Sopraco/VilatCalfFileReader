
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
	public class PROGRAM_MEMBER : DataObject
	{
        public PROGRAM_MEMBER(): base(Database.agrolink)
		{
            UpdateParams = new String[] 
			{
            	"Pm_Id"
			};
		}

	
		#region ColumnNames
		public class ColumnNames
		{  
            public const string Pm_Id = "Pm_Id";
            public const string ProgId = "ProgId";
            public const string ThrId = "ThrId";
            public const string UbnId = "UbnId";
            public const string RolePmId = "RolePmId";
            public const string PmRelationThrIds = "PmRelationThrIds";
            public const string INS = "INS";
 

		}
		#endregion

		#region Properties

        [System.ComponentModel.DataObjectField(true, true, false)] 
		public int Pm_Id
	    {
			get
	        {
				return base.Getint(ColumnNames.Pm_Id);
			}
			set
	        {
				base.Setint(ColumnNames.Pm_Id, value);
			}
		}

        public int ProgId
	    {
			get
	        {
                return base.Getint(ColumnNames.ProgId);
			}
			set
	        {
                base.Setint(ColumnNames.ProgId, value);
			}
		}

        public int ThrId
	    {
			get
	        {
                return base.Getint(ColumnNames.ThrId);
			}
			set
	        {
                base.Setint(ColumnNames.ThrId, value);
			}
		}

        public int UbnId
	    {
			get
	        {
                return base.Getint(ColumnNames.UbnId);
			}
			set
	        {
                base.Setint(ColumnNames.UbnId, value);
			}
		}

        public int RolePmId
	    {
			get
	        {
                return base.Getint(ColumnNames.RolePmId);
			}
			set
	        {
                base.Setint(ColumnNames.RolePmId, value);
			}
		}

        public string PmRelationThrIds
	    {
			get
	        {
                return base.Getstring(ColumnNames.PmRelationThrIds);
			}
			set
	        {
                base.Setstring(ColumnNames.PmRelationThrIds, value);
			}
		}

		public DateTime INS
	    {
			get
	        {
                return base.GetDateTime(ColumnNames.INS);
			}
			set
	        {
                base.SetDateTime(ColumnNames.INS, value);
			}
		}


		#endregion
		
	}
}