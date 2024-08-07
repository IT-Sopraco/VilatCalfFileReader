
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
	public class MDIERNRM_OLD : DataObject
	{
		public MDIERNRM_OLD() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"Jaar",
            	"Diercategorie",
            	"Stalsysteem"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesMDIERNRM_OLD
		{  
            public const string Jaar = "Jaar";
            public const string Diercategorie = "Diercategorie";
            public const string Stalsysteem = "Stalsysteem";
            public const string Mestproductie = "Mestproductie";
            public const string KgStikstof = "KgStikstof";
            public const string KgFosfaat = "KgFosfaat";
            public const string Stikstofcorrectie = "Stikstofcorrectie";
            public const string Diergroep_OUD = "Diergroep_OUD";
            public const string Diercategorie_OUD = "Diercategorie_OUD";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int Jaar
	    {
			get
	        {
				return base.Getint(ColumnNamesMDIERNRM_OLD.Jaar);
			}
			set
	        {
				base.Setint(ColumnNamesMDIERNRM_OLD.Jaar, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int Diercategorie
	    {
			get
	        {
				return base.Getint(ColumnNamesMDIERNRM_OLD.Diercategorie);
			}
			set
	        {
				base.Setint(ColumnNamesMDIERNRM_OLD.Diercategorie, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int Stalsysteem
	    {
			get
	        {
				return base.Getint(ColumnNamesMDIERNRM_OLD.Stalsysteem);
			}
			set
	        {
				base.Setint(ColumnNamesMDIERNRM_OLD.Stalsysteem, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double Mestproductie
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMDIERNRM_OLD.Mestproductie);
			}
			set
	        {
				base.Setdouble(ColumnNamesMDIERNRM_OLD.Mestproductie, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double KgStikstof
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMDIERNRM_OLD.KgStikstof);
			}
			set
	        {
				base.Setdouble(ColumnNamesMDIERNRM_OLD.KgStikstof, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double KgFosfaat
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMDIERNRM_OLD.KgFosfaat);
			}
			set
	        {
				base.Setdouble(ColumnNamesMDIERNRM_OLD.KgFosfaat, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double Stikstofcorrectie
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMDIERNRM_OLD.Stikstofcorrectie);
			}
			set
	        {
				base.Setdouble(ColumnNamesMDIERNRM_OLD.Stikstofcorrectie, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Diergroep_OUD
	    {
			get
	        {
				return base.Getint(ColumnNamesMDIERNRM_OLD.Diergroep_OUD);
			}
			set
	        {
				base.Setint(ColumnNamesMDIERNRM_OLD.Diergroep_OUD, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Diercategorie_OUD
	    {
			get
	        {
				return base.Getint(ColumnNamesMDIERNRM_OLD.Diercategorie_OUD);
			}
			set
	        {
				base.Setint(ColumnNamesMDIERNRM_OLD.Diercategorie_OUD, value);
			}
		}


		#endregion
		
	}
}
