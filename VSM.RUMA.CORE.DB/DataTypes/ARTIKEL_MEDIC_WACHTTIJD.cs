
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
	public class ARTIKEL_MEDIC_WACHTTIJD : DataObject
	{
		public ARTIKEL_MEDIC_WACHTTIJD() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"amwId"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesARTIKEL_MEDIC_WACHTTIJD
		{  
            public const string AmwId = "amwId";
            public const string AmwArtId = "amwArtId";
            public const string AmwDierSoort = "amwDierSoort";
            public const string AmwWachttijdSoort = "amwWachttijdSoort";
            public const string AmwCountry = "amwCountry";
            public const string AmwWachttijd = "amwWachttijd";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int amwId
	    {
			get
	        {
				return base.Getint(ColumnNamesARTIKEL_MEDIC_WACHTTIJD.AmwId);
			}
			set
	        {
				base.Setint(ColumnNamesARTIKEL_MEDIC_WACHTTIJD.AmwId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int amwArtId
	    {
			get
	        {
				return base.Getint(ColumnNamesARTIKEL_MEDIC_WACHTTIJD.AmwArtId);
			}
			set
	        {
				base.Setint(ColumnNamesARTIKEL_MEDIC_WACHTTIJD.AmwArtId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int amwDierSoort
	    {
			get
	        {
				return base.Getint(ColumnNamesARTIKEL_MEDIC_WACHTTIJD.AmwDierSoort);
			}
			set
	        {
				base.Setint(ColumnNamesARTIKEL_MEDIC_WACHTTIJD.AmwDierSoort, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int amwWachttijdSoort
	    {
			get
	        {
				return base.Getint(ColumnNamesARTIKEL_MEDIC_WACHTTIJD.AmwWachttijdSoort);
			}
			set
	        {
				base.Setint(ColumnNamesARTIKEL_MEDIC_WACHTTIJD.AmwWachttijdSoort, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int amwCountry
	    {
			get
	        {
				return base.Getint(ColumnNamesARTIKEL_MEDIC_WACHTTIJD.AmwCountry);
			}
			set
	        {
				base.Setint(ColumnNamesARTIKEL_MEDIC_WACHTTIJD.AmwCountry, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int amwWachttijd
	    {
			get
	        {
				return base.Getint(ColumnNamesARTIKEL_MEDIC_WACHTTIJD.AmwWachttijd);
			}
			set
	        {
				base.Setint(ColumnNamesARTIKEL_MEDIC_WACHTTIJD.AmwWachttijd, value);
			}
		}


		#endregion
		
	}
}
