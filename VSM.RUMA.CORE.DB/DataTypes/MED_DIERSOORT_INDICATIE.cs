
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
	public class MED_DIERSOORT_INDICATIE : DataObject
	{
		public MED_DIERSOORT_INDICATIE() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"MedRegId",
            	"Med_DiersoortId",
            	"Med_IndicatieId"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesMED_DIERSOORT_INDICATIE
		{  
            public const string MedRegId = "MedRegId";
            public const string Med_DiersoortId = "Med_DiersoortId";
            public const string Med_IndicatieId = "Med_IndicatieId";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int MedRegId
	    {
			get
	        {
				return base.Getint(ColumnNamesMED_DIERSOORT_INDICATIE.MedRegId);
			}
			set
	        {
				base.Setint(ColumnNamesMED_DIERSOORT_INDICATIE.MedRegId, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public sbyte Med_DiersoortId
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesMED_DIERSOORT_INDICATIE.Med_DiersoortId);
			}
			set
	        {
				base.Setsbyte(ColumnNamesMED_DIERSOORT_INDICATIE.Med_DiersoortId, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public short Med_IndicatieId
	    {
			get
	        {
				return base.Getshort(ColumnNamesMED_DIERSOORT_INDICATIE.Med_IndicatieId);
			}
			set
	        {
				base.Setshort(ColumnNamesMED_DIERSOORT_INDICATIE.Med_IndicatieId, value);
			}
		}


		#endregion
		
	}
}
