
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
	public class ARTIKEL_MEDIC_DOSERING : DataObject
	{
		public ARTIKEL_MEDIC_DOSERING() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"amdId"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesARTIKEL_MEDIC_DOSERING
		{  
            public const string AmdId = "amdId";
            public const string AmdArtId = "amdArtId";
            public const string AmdDierSoort = "amdDierSoort";
            public const string AmwsId = "amwsId";
            public const string AmdDosering = "amdDosering";
            public const string AmdEenheid = "amdEenheid";
            public const string AmdMinDosering = "amdMinDosering";
            public const string AmdMaxDosering = "amdMaxDosering";
            public const string AmdCorrectieDagDosering = "amdCorrectieDagDosering";
            public const string AmdTherapieDuur = "amdTherapieDuur";
            public const string AmdTherapieDuurGvp = "amdTherapieDuurGvp";
            public const string AmdDagDoseringsFactor = "amdDagDoseringsFactor";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int amdId
	    {
			get
	        {
				return base.Getint(ColumnNamesARTIKEL_MEDIC_DOSERING.AmdId);
			}
			set
	        {
				base.Setint(ColumnNamesARTIKEL_MEDIC_DOSERING.AmdId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int amdArtId
	    {
			get
	        {
				return base.Getint(ColumnNamesARTIKEL_MEDIC_DOSERING.AmdArtId);
			}
			set
	        {
				base.Setint(ColumnNamesARTIKEL_MEDIC_DOSERING.AmdArtId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int amdDierSoort
	    {
			get
	        {
				return base.Getint(ColumnNamesARTIKEL_MEDIC_DOSERING.AmdDierSoort);
			}
			set
	        {
				base.Setint(ColumnNamesARTIKEL_MEDIC_DOSERING.AmdDierSoort, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int amwsId
	    {
			get
	        {
				return base.Getint(ColumnNamesARTIKEL_MEDIC_DOSERING.AmwsId);
			}
			set
	        {
				base.Setint(ColumnNamesARTIKEL_MEDIC_DOSERING.AmwsId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double amdDosering
	    {
			get
	        {
				return base.Getdouble(ColumnNamesARTIKEL_MEDIC_DOSERING.AmdDosering);
			}
			set
	        {
				base.Setdouble(ColumnNamesARTIKEL_MEDIC_DOSERING.AmdDosering, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int amdEenheid
	    {
			get
	        {
				return base.Getint(ColumnNamesARTIKEL_MEDIC_DOSERING.AmdEenheid);
			}
			set
	        {
				base.Setint(ColumnNamesARTIKEL_MEDIC_DOSERING.AmdEenheid, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double amdMinDosering
	    {
			get
	        {
				return base.Getdouble(ColumnNamesARTIKEL_MEDIC_DOSERING.AmdMinDosering);
			}
			set
	        {
				base.Setdouble(ColumnNamesARTIKEL_MEDIC_DOSERING.AmdMinDosering, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double amdMaxDosering
	    {
			get
	        {
				return base.Getdouble(ColumnNamesARTIKEL_MEDIC_DOSERING.AmdMaxDosering);
			}
			set
	        {
				base.Setdouble(ColumnNamesARTIKEL_MEDIC_DOSERING.AmdMaxDosering, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double amdCorrectieDagDosering
	    {
			get
	        {
				return base.Getdouble(ColumnNamesARTIKEL_MEDIC_DOSERING.AmdCorrectieDagDosering);
			}
			set
	        {
				base.Setdouble(ColumnNamesARTIKEL_MEDIC_DOSERING.AmdCorrectieDagDosering, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double amdTherapieDuur
	    {
			get
	        {
				return base.Getdouble(ColumnNamesARTIKEL_MEDIC_DOSERING.AmdTherapieDuur);
			}
			set
	        {
				base.Setdouble(ColumnNamesARTIKEL_MEDIC_DOSERING.AmdTherapieDuur, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double amdTherapieDuurGvp
	    {
			get
	        {
				return base.Getdouble(ColumnNamesARTIKEL_MEDIC_DOSERING.AmdTherapieDuurGvp);
			}
			set
	        {
				base.Setdouble(ColumnNamesARTIKEL_MEDIC_DOSERING.AmdTherapieDuurGvp, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double amdDagDoseringsFactor
	    {
			get
	        {
				return base.Getdouble(ColumnNamesARTIKEL_MEDIC_DOSERING.AmdDagDoseringsFactor);
			}
			set
	        {
				base.Setdouble(ColumnNamesARTIKEL_MEDIC_DOSERING.AmdDagDoseringsFactor, value);
			}
		}


		#endregion
		
	}
}
