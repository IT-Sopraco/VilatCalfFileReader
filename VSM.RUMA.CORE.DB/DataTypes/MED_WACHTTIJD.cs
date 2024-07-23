
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
	public class MED_WACHTTIJD : DataObject
	{
		public MED_WACHTTIJD() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"MedReg_id",
            	"Med_DierSoortId",
            	"Med_Wachtsoort"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesMED_WACHTTIJD
		{  
            public const string MedReg_id = "MedReg_id";
            public const string Med_DierSoortId = "Med_DierSoortId";
            public const string Med_Wachtsoort = "Med_Wachtsoort";
            public const string Med_Wachttijd = "Med_Wachttijd";
            public const string Med_Wachttijdeenheid = "Med_Wachttijdeenheid";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int MedReg_id
	    {
			get
	        {
				return base.Getint(ColumnNamesMED_WACHTTIJD.MedReg_id);
			}
			set
	        {
				base.Setint(ColumnNamesMED_WACHTTIJD.MedReg_id, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public string Med_DierSoortId
	    {
			get
	        {
				return base.Getstring(ColumnNamesMED_WACHTTIJD.Med_DierSoortId);
			}
			set
	        {
				base.Setstring(ColumnNamesMED_WACHTTIJD.Med_DierSoortId, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public sbyte Med_Wachtsoort
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesMED_WACHTTIJD.Med_Wachtsoort);
			}
			set
	        {
				base.Setsbyte(ColumnNamesMED_WACHTTIJD.Med_Wachtsoort, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte Med_Wachttijd
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesMED_WACHTTIJD.Med_Wachttijd);
			}
			set
	        {
				base.Setsbyte(ColumnNamesMED_WACHTTIJD.Med_Wachttijd, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Med_Wachttijdeenheid
	    {
			get
	        {
				return base.Getint(ColumnNamesMED_WACHTTIJD.Med_Wachttijdeenheid);
			}
			set
	        {
				base.Setint(ColumnNamesMED_WACHTTIJD.Med_Wachttijdeenheid, value);
			}
		}


		#endregion
		
	}
}
