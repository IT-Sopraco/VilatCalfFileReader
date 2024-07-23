
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
	public class MILKPST2 : DataObject
	{
		public MILKPST2() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"UbnID",
            	"ProdEenheidnr",
            	"CooperationNumber",
            	"SupplierNumber",
            	"InvoiceNumberYear",
            	"AfrekVolgnr",
            	"CodeCooperation",
            	"BookNumber",
            	"Internalnr"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesMILKPST2
		{  
            public const string UbnID = "UbnID";
            public const string ProdEenheidnr = "ProdEenheidnr";
            public const string CooperationNumber = "CooperationNumber";
            public const string SupplierNumber = "SupplierNumber";
            public const string InvoiceNumberYear = "InvoiceNumberYear";
            public const string AfrekVolgnr = "AfrekVolgnr";
            public const string CodeCooperation = "CodeCooperation";
            public const string BookNumber = "BookNumber";
            public const string Internalnr = "Internalnr";
            public const string AmountExclVAT = "AmountExclVAT";
            public const string BookNumberText = "BookNumberText";
            public const string DateEndInvoice = "DateEndInvoice";
            public const string DateStartInvoice = "DateStartInvoice";
            public const string PenaltyPer100Liter = "PenaltyPer100Liter";
            public const string PriceExclVAT = "PriceExclVAT";
            public const string PriceVAT = "PriceVAT";
            public const string Quantity = "Quantity";
            public const string Total = "Total";
            public const string ValutaCode = "ValutaCode";
            public const string VATPercentage = "VATPercentage";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int UbnID
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKPST2.UbnID);
			}
			set
	        {
				base.Setint(ColumnNamesMILKPST2.UbnID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public string ProdEenheidnr
	    {
			get
	        {
				return base.Getstring(ColumnNamesMILKPST2.ProdEenheidnr);
			}
			set
	        {
				base.Setstring(ColumnNamesMILKPST2.ProdEenheidnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int CooperationNumber
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKPST2.CooperationNumber);
			}
			set
	        {
				base.Setint(ColumnNamesMILKPST2.CooperationNumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int SupplierNumber
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKPST2.SupplierNumber);
			}
			set
	        {
				base.Setint(ColumnNamesMILKPST2.SupplierNumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int InvoiceNumberYear
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKPST2.InvoiceNumberYear);
			}
			set
	        {
				base.Setint(ColumnNamesMILKPST2.InvoiceNumberYear, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int AfrekVolgnr
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKPST2.AfrekVolgnr);
			}
			set
	        {
				base.Setint(ColumnNamesMILKPST2.AfrekVolgnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public string CodeCooperation
	    {
			get
	        {
				return base.Getstring(ColumnNamesMILKPST2.CodeCooperation);
			}
			set
	        {
				base.Setstring(ColumnNamesMILKPST2.CodeCooperation, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public string BookNumber
	    {
			get
	        {
				return base.Getstring(ColumnNamesMILKPST2.BookNumber);
			}
			set
	        {
				base.Setstring(ColumnNamesMILKPST2.BookNumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int Internalnr
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKPST2.Internalnr);
			}
			set
	        {
				base.Setint(ColumnNamesMILKPST2.Internalnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double AmountExclVAT
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKPST2.AmountExclVAT);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKPST2.AmountExclVAT, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string BookNumberText
	    {
			get
	        {
				return base.Getstring(ColumnNamesMILKPST2.BookNumberText);
			}
			set
	        {
				base.Setstring(ColumnNamesMILKPST2.BookNumberText, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime DateEndInvoice
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesMILKPST2.DateEndInvoice);
			}
			set
	        {
				base.SetDateTime(ColumnNamesMILKPST2.DateEndInvoice, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime DateStartInvoice
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesMILKPST2.DateStartInvoice);
			}
			set
	        {
				base.SetDateTime(ColumnNamesMILKPST2.DateStartInvoice, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double PenaltyPer100Liter
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKPST2.PenaltyPer100Liter);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKPST2.PenaltyPer100Liter, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double PriceExclVAT
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKPST2.PriceExclVAT);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKPST2.PriceExclVAT, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double PriceVAT
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKPST2.PriceVAT);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKPST2.PriceVAT, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double Quantity
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKPST2.Quantity);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKPST2.Quantity, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double Total
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKPST2.Total);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKPST2.Total, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int ValutaCode
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKPST2.ValutaCode);
			}
			set
	        {
				base.Setint(ColumnNamesMILKPST2.ValutaCode, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double VATPercentage
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKPST2.VATPercentage);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKPST2.VATPercentage, value);
			}
		}


		#endregion
		
	}
}
