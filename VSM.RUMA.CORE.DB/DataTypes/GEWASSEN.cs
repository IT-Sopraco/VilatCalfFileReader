
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
	public class GEWASSEN : DataObject
	{
		public GEWASSEN() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"GewGewasid"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesGEWASSEN
		{  
            public const string GewGewasid = "GewGewasid";
            public const string GewGewasnaam = "GewGewasnaam";
            public const string GewGewasgroep = "GewGewasgroep";
            public const string GewVoorkeur = "GewVoorkeur";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int GewGewasid
	    {
			get
	        {
				return base.Getint(ColumnNamesGEWASSEN.GewGewasid);
			}
			set
	        {
				base.Setint(ColumnNamesGEWASSEN.GewGewasid, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string GewGewasnaam
	    {
			get
	        {
				return base.Getstring(ColumnNamesGEWASSEN.GewGewasnaam);
			}
			set
	        {
				base.Setstring(ColumnNamesGEWASSEN.GewGewasnaam, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int GewGewasgroep
	    {
			get
	        {
				return base.Getint(ColumnNamesGEWASSEN.GewGewasgroep);
			}
			set
	        {
				base.Setint(ColumnNamesGEWASSEN.GewGewasgroep, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int GewVoorkeur
	    {
			get
	        {
				return base.Getint(ColumnNamesGEWASSEN.GewVoorkeur);
			}
			set
	        {
				base.Setint(ColumnNamesGEWASSEN.GewVoorkeur, value);
			}
		}


		#endregion
		
	}
}
