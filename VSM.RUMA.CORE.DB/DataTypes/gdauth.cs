
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
	public class gdauth : DataObject
	{
		public gdauth() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{

			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesgdauth
		{  
            public const string ProgramId = "ProgramId";
            public const string StamboekGDnr = "StamboekGDnr";
            public const string StamboekNaam = "StamboekNaam";
            public const string ReportDate = "ReportDate";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int ProgramId
	    {
			get
	        {
				return base.Getint(ColumnNamesgdauth.ProgramId);
			}
			set
	        {
				base.Setint(ColumnNamesgdauth.ProgramId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string StamboekGDnr
	    {
			get
	        {
				return base.Getstring(ColumnNamesgdauth.StamboekGDnr);
			}
			set
	        {
				base.Setstring(ColumnNamesgdauth.StamboekGDnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string StamboekNaam
	    {
			get
	        {
				return base.Getstring(ColumnNamesgdauth.StamboekNaam);
			}
			set
	        {
				base.Setstring(ColumnNamesgdauth.StamboekNaam, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime ReportDate
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesgdauth.ReportDate);
			}
			set
	        {
				base.SetDateTime(ColumnNamesgdauth.ReportDate, value);
			}
		}


		#endregion
		
	}
}
