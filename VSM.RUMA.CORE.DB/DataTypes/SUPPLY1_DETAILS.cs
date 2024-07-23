
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
	public class SUPPLY1_DETAILS : DataObject
	{
		public SUPPLY1_DETAILS() : base(Database.animal)
		{
			UpdateParams = new String[] 
			{
            	"AdmisID",
            	"FactID",
            	"SupplyID",
            	"AniID",
            	"EventID",
            	"MovID"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesSUPPLY1_DETAILS
		{  
            public const string AdmisID = "AdmisID";
            public const string FactID = "FactID";
            public const string SupplyID = "SupplyID";
            public const string AniID = "AniID";
            public const string EventID = "EventID";
            public const string MovID = "MovID";
            public const string se_Mutation_Date = "se_Mutation_Date";
            public const string se_Mutation_by_ThrID = "se_Mutation_by_ThrID";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int AdmisID
	    {
			get
	        {
				return base.Getint(ColumnNamesSUPPLY1_DETAILS.AdmisID);
			}
			set
	        {
				base.Setint(ColumnNamesSUPPLY1_DETAILS.AdmisID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int FactID
	    {
			get
	        {
				return base.Getint(ColumnNamesSUPPLY1_DETAILS.FactID);
			}
			set
	        {
				base.Setint(ColumnNamesSUPPLY1_DETAILS.FactID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int SupplyID
	    {
			get
	        {
				return base.Getint(ColumnNamesSUPPLY1_DETAILS.SupplyID);
			}
			set
	        {
				base.Setint(ColumnNamesSUPPLY1_DETAILS.SupplyID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int AniID
	    {
			get
	        {
				return base.Getint(ColumnNamesSUPPLY1_DETAILS.AniID);
			}
			set
	        {
				base.Setint(ColumnNamesSUPPLY1_DETAILS.AniID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int EventID
	    {
			get
	        {
				return base.Getint(ColumnNamesSUPPLY1_DETAILS.EventID);
			}
			set
	        {
				base.Setint(ColumnNamesSUPPLY1_DETAILS.EventID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int MovID
	    {
			get
	        {
				return base.Getint(ColumnNamesSUPPLY1_DETAILS.MovID);
			}
			set
	        {
				base.Setint(ColumnNamesSUPPLY1_DETAILS.MovID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime Se_Mutation_Date
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesSUPPLY1_DETAILS.se_Mutation_Date);
			}
			set
	        {
				base.SetDateTime(ColumnNamesSUPPLY1_DETAILS.se_Mutation_Date, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Se_Mutation_by_ThrID
	    {
			get
	        {
				return base.Getint(ColumnNamesSUPPLY1_DETAILS.se_Mutation_by_ThrID);
			}
			set
	        {
				base.Setint(ColumnNamesSUPPLY1_DETAILS.se_Mutation_by_ThrID, value);
			}
		}


		#endregion
		
	}
}