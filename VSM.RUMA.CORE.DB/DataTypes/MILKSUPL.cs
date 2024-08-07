
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
	public class MILKSUPL : DataObject
	{
		public MILKSUPL() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"UbnID",
            	"ProdEenheidnr",
            	"CooperationNumber",
            	"SupplierNumber",
            	"InvoiceNumberYear",
            	"AfrekVolgnr",
            	"DeliveryDate",
            	"DeliveryTime"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesMILKSUPL
		{  
            public const string UbnID = "UbnID";
            public const string ProdEenheidnr = "ProdEenheidnr";
            public const string CooperationNumber = "CooperationNumber";
            public const string SupplierNumber = "SupplierNumber";
            public const string InvoiceNumberYear = "InvoiceNumberYear";
            public const string AfrekVolgnr = "AfrekVolgnr";
            public const string DeliveryDate = "DeliveryDate";
            public const string DeliveryTime = "DeliveryTime";
            public const string EndDateInvoice = "EndDateInvoice";
            public const string FatGrPerLiter = "FatGrPerLiter";
            public const string FatProteinCalc = "FatProteinCalc";
            public const string FatTank = "FatTank";
            public const string KgBacterieDiscount = "KgBacterieDiscount";
            public const string KgMilk = "KgMilk";
            public const string KindBacterieStop = "KindBacterieStop";
            public const string KindBactStopNew = "KindBactStopNew";
            public const string LactoseTank = "LactoseTank";
            public const string LitersMilk = "LitersMilk";
            public const string PriceBacterieDiscount = "PriceBacterieDiscount";
            public const string ProteinTank = "ProteinTank";
            public const string ProtGrPerLiter = "ProtGrPerLiter";
            public const string ResultBacterieStop = "ResultBacterieStop";
            public const string ResultBactStopNew = "ResultBactStopNew";
            public const string StartDateInvoice = "StartDateInvoice";
            public const string Temperature = "Temperature";
            public const string UreumTank = "UreumTank";
            public const string ValutaCode = "ValutaCode";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int UbnID
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKSUPL.UbnID);
			}
			set
	        {
				base.Setint(ColumnNamesMILKSUPL.UbnID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public string ProdEenheidnr
	    {
			get
	        {
				return base.Getstring(ColumnNamesMILKSUPL.ProdEenheidnr);
			}
			set
	        {
				base.Setstring(ColumnNamesMILKSUPL.ProdEenheidnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int CooperationNumber
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKSUPL.CooperationNumber);
			}
			set
	        {
				base.Setint(ColumnNamesMILKSUPL.CooperationNumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int SupplierNumber
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKSUPL.SupplierNumber);
			}
			set
	        {
				base.Setint(ColumnNamesMILKSUPL.SupplierNumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int InvoiceNumberYear
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKSUPL.InvoiceNumberYear);
			}
			set
	        {
				base.Setint(ColumnNamesMILKSUPL.InvoiceNumberYear, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int AfrekVolgnr
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKSUPL.AfrekVolgnr);
			}
			set
	        {
				base.Setint(ColumnNamesMILKSUPL.AfrekVolgnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public DateTime DeliveryDate
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesMILKSUPL.DeliveryDate);
			}
			set
	        {
				base.SetDateTime(ColumnNamesMILKSUPL.DeliveryDate, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public DateTime DeliveryTime
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesMILKSUPL.DeliveryTime);
			}
			set
	        {
				base.SetDateTime(ColumnNamesMILKSUPL.DeliveryTime, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime EndDateInvoice
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesMILKSUPL.EndDateInvoice);
			}
			set
	        {
				base.SetDateTime(ColumnNamesMILKSUPL.EndDateInvoice, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double FatGrPerLiter
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKSUPL.FatGrPerLiter);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKSUPL.FatGrPerLiter, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int FatProteinCalc
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKSUPL.FatProteinCalc);
			}
			set
	        {
				base.Setint(ColumnNamesMILKSUPL.FatProteinCalc, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double FatTank
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKSUPL.FatTank);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKSUPL.FatTank, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double KgBacterieDiscount
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKSUPL.KgBacterieDiscount);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKSUPL.KgBacterieDiscount, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double KgMilk
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKSUPL.KgMilk);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKSUPL.KgMilk, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int KindBacterieStop
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKSUPL.KindBacterieStop);
			}
			set
	        {
				base.Setint(ColumnNamesMILKSUPL.KindBacterieStop, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string KindBactStopNew
	    {
			get
	        {
				return base.Getstring(ColumnNamesMILKSUPL.KindBactStopNew);
			}
			set
	        {
				base.Setstring(ColumnNamesMILKSUPL.KindBactStopNew, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double LactoseTank
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKSUPL.LactoseTank);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKSUPL.LactoseTank, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double LitersMilk
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKSUPL.LitersMilk);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKSUPL.LitersMilk, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double PriceBacterieDiscount
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKSUPL.PriceBacterieDiscount);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKSUPL.PriceBacterieDiscount, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double ProteinTank
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKSUPL.ProteinTank);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKSUPL.ProteinTank, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double ProtGrPerLiter
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKSUPL.ProtGrPerLiter);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKSUPL.ProtGrPerLiter, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int ResultBacterieStop
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKSUPL.ResultBacterieStop);
			}
			set
	        {
				base.Setint(ColumnNamesMILKSUPL.ResultBacterieStop, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string ResultBactStopNew
	    {
			get
	        {
				return base.Getstring(ColumnNamesMILKSUPL.ResultBactStopNew);
			}
			set
	        {
				base.Setstring(ColumnNamesMILKSUPL.ResultBactStopNew, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime StartDateInvoice
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesMILKSUPL.StartDateInvoice);
			}
			set
	        {
				base.SetDateTime(ColumnNamesMILKSUPL.StartDateInvoice, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double Temperature
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKSUPL.Temperature);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKSUPL.Temperature, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int UreumTank
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKSUPL.UreumTank);
			}
			set
	        {
				base.Setint(ColumnNamesMILKSUPL.UreumTank, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int ValutaCode
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKSUPL.ValutaCode);
			}
			set
	        {
				base.Setint(ColumnNamesMILKSUPL.ValutaCode, value);
			}
		}


		#endregion
		
	}
}
