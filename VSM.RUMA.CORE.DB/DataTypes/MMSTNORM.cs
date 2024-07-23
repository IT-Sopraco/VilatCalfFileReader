
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
	public class MMSTNORM : DataObject
	{
		public MMSTNORM() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"Year",
            	"Mestcode"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesMMSTNORM
		{  
            public const string Year = "Year";
            public const string Mestcode = "Mestcode";
            public const string KgNperTon = "KgNperTon";
            public const string KgP2O5perTon = "KgP2O5perTon";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int Year
	    {
			get
	        {
				return base.Getint(ColumnNamesMMSTNORM.Year);
			}
			set
	        {
				base.Setint(ColumnNamesMMSTNORM.Year, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int Mestcode
	    {
			get
	        {
				return base.Getint(ColumnNamesMMSTNORM.Mestcode);
			}
			set
	        {
				base.Setint(ColumnNamesMMSTNORM.Mestcode, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double KgNperTon
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMMSTNORM.KgNperTon);
			}
			set
	        {
				base.Setdouble(ColumnNamesMMSTNORM.KgNperTon, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double KgP2O5perTon
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMMSTNORM.KgP2O5perTon);
			}
			set
	        {
				base.Setdouble(ColumnNamesMMSTNORM.KgP2O5perTon, value);
			}
		}


		#endregion
		
	}
}