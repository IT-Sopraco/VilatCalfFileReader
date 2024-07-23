
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
	public class SUPPLUBN : DataObject
	{
		public SUPPLUBN() : base(Database.animal)
		{
			UpdateParams = new String[] 
			{
            	"FarmNumber",
            	"SupplierNumber"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesSUPPLUBN
		{  
            public const string FarmNumber = "FarmNumber";
            public const string SupplierNumber = "SupplierNumber";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public string FarmNumber
	    {
			get
	        {
				return base.Getstring(ColumnNamesSUPPLUBN.FarmNumber);
			}
			set
	        {
				base.Setstring(ColumnNamesSUPPLUBN.FarmNumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int SupplierNumber
	    {
			get
	        {
				return base.Getint(ColumnNamesSUPPLUBN.SupplierNumber);
			}
			set
	        {
				base.Setint(ColumnNamesSUPPLUBN.SupplierNumber, value);
			}
		}


		#endregion
		
	}
}
