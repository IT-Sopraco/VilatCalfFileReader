
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
	public class ARTIKEL_MEDIC_REGNR : DataObject
	{
		public ARTIKEL_MEDIC_REGNR() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"amrId"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesARTIKEL_MEDIC_REGNR
		{  
            public const string AmrId = "amrId";
            public const string AmrArtId = "amrArtId";
            public const string AmrCountry = "amrCountry";
            public const string AmrRegnr = "amrRegnr";
            public const string AmrRegnrString = "amrRegnrString";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int amrId
	    {
			get
	        {
				return base.Getint(ColumnNamesARTIKEL_MEDIC_REGNR.AmrId);
			}
			set
	        {
				base.Setint(ColumnNamesARTIKEL_MEDIC_REGNR.AmrId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int amrArtId
	    {
			get
	        {
				return base.Getint(ColumnNamesARTIKEL_MEDIC_REGNR.AmrArtId);
			}
			set
	        {
				base.Setint(ColumnNamesARTIKEL_MEDIC_REGNR.AmrArtId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int amrCountry
	    {
			get
	        {
				return base.Getint(ColumnNamesARTIKEL_MEDIC_REGNR.AmrCountry);
			}
			set
	        {
				base.Setint(ColumnNamesARTIKEL_MEDIC_REGNR.AmrCountry, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string amrRegnr
	    {
			get
	        {
				return base.Getstring(ColumnNamesARTIKEL_MEDIC_REGNR.AmrRegnr);
			}
			set
	        {
				base.Setstring(ColumnNamesARTIKEL_MEDIC_REGNR.AmrRegnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string amrRegnrString
	    {
			get
	        {
				return base.Getstring(ColumnNamesARTIKEL_MEDIC_REGNR.AmrRegnrString);
			}
			set
	        {
				base.Setstring(ColumnNamesARTIKEL_MEDIC_REGNR.AmrRegnrString, value);
			}
		}


		#endregion
		
	}
}
