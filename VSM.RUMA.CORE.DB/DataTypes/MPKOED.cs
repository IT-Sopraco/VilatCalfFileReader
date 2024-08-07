
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
	public class MPKOED : DataObject
	{
		public MPKOED() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"Jaar",
            	"Stalsysteem",
            	"Internalnr",
            	"Ureum"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesMPKOED
		{  
            public const string Jaar = "Jaar";
            public const string Stalsysteem = "Stalsysteem";
            public const string Internalnr = "Internalnr";
            public const string Ureum = "Ureum";
            public const string KgStikstof = "KgStikstof";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int Jaar
	    {
			get
	        {
				return base.Getint(ColumnNamesMPKOED.Jaar);
			}
			set
	        {
				base.Setint(ColumnNamesMPKOED.Jaar, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int Stalsysteem
	    {
			get
	        {
				return base.Getint(ColumnNamesMPKOED.Stalsysteem);
			}
			set
	        {
				base.Setint(ColumnNamesMPKOED.Stalsysteem, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int Internalnr
	    {
			get
	        {
				return base.Getint(ColumnNamesMPKOED.Internalnr);
			}
			set
	        {
				base.Setint(ColumnNamesMPKOED.Internalnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public double Ureum
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMPKOED.Ureum);
			}
			set
	        {
				base.Setdouble(ColumnNamesMPKOED.Ureum, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double KgStikstof
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMPKOED.KgStikstof);
			}
			set
	        {
				base.Setdouble(ColumnNamesMPKOED.KgStikstof, value);
			}
		}


		#endregion
		
	}
}
