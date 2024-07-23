
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
	public class MKOEMLK : DataObject
	{
		public MKOEMLK() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"Mestnummer",
            	"Prognose",
            	"Jaar"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesMKOEMLK
		{  
            public const string Mestnummer = "Mestnummer";
            public const string Prognose = "Prognose";
            public const string Jaar = "Jaar";
            public const string Kgmelk = "Kgmelk";
            public const string Ureum = "Ureum";
            public const string PercVet = "PercVet";
            public const string PercEiwit = "PercEiwit";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public string Mestnummer
	    {
			get
	        {
				return base.Getstring(ColumnNamesMKOEMLK.Mestnummer);
			}
			set
	        {
				base.Setstring(ColumnNamesMKOEMLK.Mestnummer, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int Prognose
	    {
			get
	        {
				return base.Getint(ColumnNamesMKOEMLK.Prognose);
			}
			set
	        {
				base.Setint(ColumnNamesMKOEMLK.Prognose, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int Jaar
	    {
			get
	        {
				return base.Getint(ColumnNamesMKOEMLK.Jaar);
			}
			set
	        {
				base.Setint(ColumnNamesMKOEMLK.Jaar, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Kgmelk
	    {
			get
	        {
				return base.Getint(ColumnNamesMKOEMLK.Kgmelk);
			}
			set
	        {
				base.Setint(ColumnNamesMKOEMLK.Kgmelk, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Ureum
	    {
			get
	        {
				return base.Getint(ColumnNamesMKOEMLK.Ureum);
			}
			set
	        {
				base.Setint(ColumnNamesMKOEMLK.Ureum, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double PercVet
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMKOEMLK.PercVet);
			}
			set
	        {
				base.Setdouble(ColumnNamesMKOEMLK.PercVet, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double PercEiwit
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMKOEMLK.PercEiwit);
			}
			set
	        {
				base.Setdouble(ColumnNamesMKOEMLK.PercEiwit, value);
			}
		}


		#endregion
		
	}
}
