
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
	public class MDIERNRM : DataObject
	{
		public MDIERNRM() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"Jaar",
            	"Diergroep",
            	"Diercategorie",
            	"Stalsysteem"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesMDIERNRM
		{  
            public const string Jaar = "Jaar";
            public const string Diergroep = "Diergroep";
            public const string Diercategorie = "Diercategorie";
            public const string Stalsysteem = "Stalsysteem";
            public const string Mestproductie = "Mestproductie";
            public const string KgStikstof = "KgStikstof";
            public const string KgFosfaat = "KgFosfaat";
            public const string Stikstofcorrectie = "Stikstofcorrectie";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int Jaar
	    {
			get
	        {
				return base.Getint(ColumnNamesMDIERNRM.Jaar);
			}
			set
	        {
				base.Setint(ColumnNamesMDIERNRM.Jaar, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int Diergroep
	    {
			get
	        {
				return base.Getint(ColumnNamesMDIERNRM.Diergroep);
			}
			set
	        {
				base.Setint(ColumnNamesMDIERNRM.Diergroep, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int Diercategorie
	    {
			get
	        {
				return base.Getint(ColumnNamesMDIERNRM.Diercategorie);
			}
			set
	        {
				base.Setint(ColumnNamesMDIERNRM.Diercategorie, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int Stalsysteem
	    {
			get
	        {
				return base.Getint(ColumnNamesMDIERNRM.Stalsysteem);
			}
			set
	        {
				base.Setint(ColumnNamesMDIERNRM.Stalsysteem, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double Mestproductie
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMDIERNRM.Mestproductie);
			}
			set
	        {
				base.Setdouble(ColumnNamesMDIERNRM.Mestproductie, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double KgStikstof
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMDIERNRM.KgStikstof);
			}
			set
	        {
				base.Setdouble(ColumnNamesMDIERNRM.KgStikstof, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double KgFosfaat
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMDIERNRM.KgFosfaat);
			}
			set
	        {
				base.Setdouble(ColumnNamesMDIERNRM.KgFosfaat, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double Stikstofcorrectie
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMDIERNRM.Stikstofcorrectie);
			}
			set
	        {
				base.Setdouble(ColumnNamesMDIERNRM.Stikstofcorrectie, value);
			}
		}


		#endregion
		
	}
}