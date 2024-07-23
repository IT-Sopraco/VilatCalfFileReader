
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
	public class MILKSAMP : DataObject
	{
		public MILKSAMP() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"UbnID",
            	"ProdEenheidnr",
            	"CooperationNumber",
            	"SupplierNumber",
            	"InvoiceNumberYear",
            	"AfrekVolgnr",
            	"DateDeliveryMilk",
            	"TimeDeliveryMilk"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesMILKSAMP
		{  
            public const string UbnID = "UbnID";
            public const string ProdEenheidnr = "ProdEenheidnr";
            public const string CooperationNumber = "CooperationNumber";
            public const string SupplierNumber = "SupplierNumber";
            public const string InvoiceNumberYear = "InvoiceNumberYear";
            public const string AfrekVolgnr = "AfrekVolgnr";
            public const string DateDeliveryMilk = "DateDeliveryMilk";
            public const string TimeDeliveryMilk = "TimeDeliveryMilk";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int UbnID
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKSAMP.UbnID);
			}
			set
	        {
				base.Setint(ColumnNamesMILKSAMP.UbnID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public string ProdEenheidnr
	    {
			get
	        {
				return base.Getstring(ColumnNamesMILKSAMP.ProdEenheidnr);
			}
			set
	        {
				base.Setstring(ColumnNamesMILKSAMP.ProdEenheidnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int CooperationNumber
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKSAMP.CooperationNumber);
			}
			set
	        {
				base.Setint(ColumnNamesMILKSAMP.CooperationNumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int SupplierNumber
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKSAMP.SupplierNumber);
			}
			set
	        {
				base.Setint(ColumnNamesMILKSAMP.SupplierNumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int InvoiceNumberYear
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKSAMP.InvoiceNumberYear);
			}
			set
	        {
				base.Setint(ColumnNamesMILKSAMP.InvoiceNumberYear, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int AfrekVolgnr
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKSAMP.AfrekVolgnr);
			}
			set
	        {
				base.Setint(ColumnNamesMILKSAMP.AfrekVolgnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public DateTime DateDeliveryMilk
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesMILKSAMP.DateDeliveryMilk);
			}
			set
	        {
				base.SetDateTime(ColumnNamesMILKSAMP.DateDeliveryMilk, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public DateTime TimeDeliveryMilk
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesMILKSAMP.TimeDeliveryMilk);
			}
			set
	        {
				base.SetDateTime(ColumnNamesMILKSAMP.TimeDeliveryMilk, value);
			}
		}


		#endregion
		
	}
}
