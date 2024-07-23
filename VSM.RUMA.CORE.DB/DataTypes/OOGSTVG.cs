
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
	public class OOGSTVG : DataObject
	{
		public OOGSTVG() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"OvgPerceelid",
            	"OvgActid"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesOOGSTVG
		{  
            public const string OvgPerceelid = "OvgPerceelid";
            public const string OvgActid = "OvgActid";
            public const string OvgMethode = "OvgMethode";
            public const string OvgGroeidagen = "OvgGroeidagen";
            public const string OvgKgDSperHa = "OvgKgDSperHa";
            public const string OvgKgPercDS = "OvgKgPercDS";
            public const string OvgKwaliteit = "OvgKwaliteit";
            public const string OvgToevoegmiddel = "OvgToevoegmiddel";
            public const string OvgVelddagen = "OvgVelddagen";
            public const string OvgOpbrengstperHa = "OvgOpbrengstperHa";
            public const string OvgOpslagplaats = "OvgOpslagplaats";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int OvgPerceelid
	    {
			get
	        {
				return base.Getint(ColumnNamesOOGSTVG.OvgPerceelid);
			}
			set
	        {
				base.Setint(ColumnNamesOOGSTVG.OvgPerceelid, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int OvgActid
	    {
			get
	        {
				return base.Getint(ColumnNamesOOGSTVG.OvgActid);
			}
			set
	        {
				base.Setint(ColumnNamesOOGSTVG.OvgActid, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int OvgMethode
	    {
			get
	        {
				return base.Getint(ColumnNamesOOGSTVG.OvgMethode);
			}
			set
	        {
				base.Setint(ColumnNamesOOGSTVG.OvgMethode, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int OvgGroeidagen
	    {
			get
	        {
				return base.Getint(ColumnNamesOOGSTVG.OvgGroeidagen);
			}
			set
	        {
				base.Setint(ColumnNamesOOGSTVG.OvgGroeidagen, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int OvgKgDSperHa
	    {
			get
	        {
				return base.Getint(ColumnNamesOOGSTVG.OvgKgDSperHa);
			}
			set
	        {
				base.Setint(ColumnNamesOOGSTVG.OvgKgDSperHa, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double OvgKgPercDS
	    {
			get
	        {
				return base.Getdouble(ColumnNamesOOGSTVG.OvgKgPercDS);
			}
			set
	        {
				base.Setdouble(ColumnNamesOOGSTVG.OvgKgPercDS, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int OvgKwaliteit
	    {
			get
	        {
				return base.Getint(ColumnNamesOOGSTVG.OvgKwaliteit);
			}
			set
	        {
				base.Setint(ColumnNamesOOGSTVG.OvgKwaliteit, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int OvgToevoegmiddel
	    {
			get
	        {
				return base.Getint(ColumnNamesOOGSTVG.OvgToevoegmiddel);
			}
			set
	        {
				base.Setint(ColumnNamesOOGSTVG.OvgToevoegmiddel, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int OvgVelddagen
	    {
			get
	        {
				return base.Getint(ColumnNamesOOGSTVG.OvgVelddagen);
			}
			set
	        {
				base.Setint(ColumnNamesOOGSTVG.OvgVelddagen, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double OvgOpbrengstperHa
	    {
			get
	        {
				return base.Getdouble(ColumnNamesOOGSTVG.OvgOpbrengstperHa);
			}
			set
	        {
				base.Setdouble(ColumnNamesOOGSTVG.OvgOpbrengstperHa, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int OvgOpslagplaats
	    {
			get
	        {
				return base.Getint(ColumnNamesOOGSTVG.OvgOpslagplaats);
			}
			set
	        {
				base.Setint(ColumnNamesOOGSTVG.OvgOpslagplaats, value);
			}
		}


		#endregion
		
	}
}