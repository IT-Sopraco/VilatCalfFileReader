
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
	public class MILKINVO : DataObject
	{
		public MILKINVO() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"UbnID",
            	"ProdEenheidnr",
            	"CooperationNumber",
            	"SupplierNumber",
            	"InvoiceNumberYear",
            	"AfrekVolgnr"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesMILKINVO
		{  
            public const string UbnID = "UbnID";
            public const string ProdEenheidnr = "ProdEenheidnr";
            public const string CooperationNumber = "CooperationNumber";
            public const string SupplierNumber = "SupplierNumber";
            public const string InvoiceNumberYear = "InvoiceNumberYear";
            public const string AfrekVolgnr = "AfrekVolgnr";
            public const string City = "City";
            public const string CompanyName = "CompanyName";
            public const string CorrectionFat = "CorrectionFat";
            public const string Country = "Country";
            public const string CumulateFatCorrection = "CumulateFatCorrection";
            public const string DateEndInvoice = "DateEndInvoice";
            public const string DateStartInvoice = "DateStartInvoice";
            public const string FatGrPerLitMonthAvg = "FatGrPerLitMonthAvg";
            public const string FatPercCooperation = "FatPercCooperation";
            public const string FatPercInvoice = "FatPercInvoice";
            public const string FatPercNow3Dec = "FatPercNow3Dec";
            public const string FatPercUntilNow = "FatPercUntilNow";
            public const string HouseNumber = "HouseNumber";
            public const string InvoiceDate = "InvoiceDate";
            public const string InvoiceNumber = "InvoiceNumber";
            public const string KgMilkCumulate = "KgMilkCumulate";
            public const string KgMilkInvoice = "KgMilkInvoice";
            public const string LitersMilkInvoice = "LitersMilkInvoice";
            public const string MilkPriceCooperation = "MilkPriceCooperation";
            public const string MilkpriceExclVAT = "MilkpriceExclVAT";
            public const string MilkpriceInclVAT = "MilkpriceInclVAT";
            public const string MilkPriceLastYear = "MilkPriceLastYear";
            public const string MilkPriceVAT = "MilkPriceVAT";
            public const string NumberPointTotal = "NumberPointTotal";
            public const string ProteinPercCooperation = "ProteinPercCooperation";
            public const string ProteinPercInvoice = "ProteinPercInvoice";
            public const string ProtGrPerLitMonthAvg = "ProtGrPerLitMonthAvg";
            public const string RestNumberPeriod = "RestNumberPeriod";
            public const string Street = "Street";
            public const string UreaCooperation = "UreaCooperation";
            public const string UreaCumulate = "UreaCumulate";
            public const string UreaInvoice = "UreaInvoice";
            public const string ValutaCode = "ValutaCode";
            public const string Versionnr = "Versionnr";
            public const string Zipcode = "Zipcode";
            public const string LactosePercInvoice = "LactosePercInvoice";
            public const string LactosePercCooperation = "LactosePercCooperation";
            public const string LactoseGrPerLitMonthAvg = "LactoseGrPerLitMonthAvg";

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int UbnID
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKINVO.UbnID);
			}
			set
	        {
				base.Setint(ColumnNamesMILKINVO.UbnID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public string ProdEenheidnr
	    {
			get
	        {
				return base.Getstring(ColumnNamesMILKINVO.ProdEenheidnr);
			}
			set
	        {
				base.Setstring(ColumnNamesMILKINVO.ProdEenheidnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int CooperationNumber
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKINVO.CooperationNumber);
			}
			set
	        {
				base.Setint(ColumnNamesMILKINVO.CooperationNumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int SupplierNumber
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKINVO.SupplierNumber);
			}
			set
	        {
				base.Setint(ColumnNamesMILKINVO.SupplierNumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int InvoiceNumberYear
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKINVO.InvoiceNumberYear);
			}
			set
	        {
				base.Setint(ColumnNamesMILKINVO.InvoiceNumberYear, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int AfrekVolgnr
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKINVO.AfrekVolgnr);
			}
			set
	        {
				base.Setint(ColumnNamesMILKINVO.AfrekVolgnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string City
	    {
			get
	        {
				return base.Getstring(ColumnNamesMILKINVO.City);
			}
			set
	        {
				base.Setstring(ColumnNamesMILKINVO.City, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string CompanyName
	    {
			get
	        {
				return base.Getstring(ColumnNamesMILKINVO.CompanyName);
			}
			set
	        {
				base.Setstring(ColumnNamesMILKINVO.CompanyName, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double CorrectionFat
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKINVO.CorrectionFat);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKINVO.CorrectionFat, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string Country
	    {
			get
	        {
				return base.Getstring(ColumnNamesMILKINVO.Country);
			}
			set
	        {
				base.Setstring(ColumnNamesMILKINVO.Country, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int CumulateFatCorrection
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKINVO.CumulateFatCorrection);
			}
			set
	        {
				base.Setint(ColumnNamesMILKINVO.CumulateFatCorrection, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime DateEndInvoice
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesMILKINVO.DateEndInvoice);
			}
			set
	        {
				base.SetDateTime(ColumnNamesMILKINVO.DateEndInvoice, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime DateStartInvoice
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesMILKINVO.DateStartInvoice);
			}
			set
	        {
				base.SetDateTime(ColumnNamesMILKINVO.DateStartInvoice, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double FatGrPerLitMonthAvg
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKINVO.FatGrPerLitMonthAvg);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKINVO.FatGrPerLitMonthAvg, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double FatPercCooperation
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKINVO.FatPercCooperation);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKINVO.FatPercCooperation, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double FatPercInvoice
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKINVO.FatPercInvoice);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKINVO.FatPercInvoice, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double FatPercNow3Dec
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKINVO.FatPercNow3Dec);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKINVO.FatPercNow3Dec, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double FatPercUntilNow
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKINVO.FatPercUntilNow);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKINVO.FatPercUntilNow, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string HouseNumber
	    {
			get
	        {
				return base.Getstring(ColumnNamesMILKINVO.HouseNumber);
			}
			set
	        {
				base.Setstring(ColumnNamesMILKINVO.HouseNumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime InvoiceDate
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesMILKINVO.InvoiceDate);
			}
			set
	        {
				base.SetDateTime(ColumnNamesMILKINVO.InvoiceDate, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string InvoiceNumber
	    {
			get
	        {
				return base.Getstring(ColumnNamesMILKINVO.InvoiceNumber);
			}
			set
	        {
				base.Setstring(ColumnNamesMILKINVO.InvoiceNumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int KgMilkCumulate
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKINVO.KgMilkCumulate);
			}
			set
	        {
				base.Setint(ColumnNamesMILKINVO.KgMilkCumulate, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double KgMilkInvoice
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKINVO.KgMilkInvoice);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKINVO.KgMilkInvoice, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double LitersMilkInvoice
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKINVO.LitersMilkInvoice);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKINVO.LitersMilkInvoice, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double MilkPriceCooperation
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKINVO.MilkPriceCooperation);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKINVO.MilkPriceCooperation, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double MilkpriceExclVAT
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKINVO.MilkpriceExclVAT);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKINVO.MilkpriceExclVAT, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double MilkpriceInclVAT
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKINVO.MilkpriceInclVAT);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKINVO.MilkpriceInclVAT, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double MilkPriceLastYear
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKINVO.MilkPriceLastYear);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKINVO.MilkPriceLastYear, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double MilkPriceVAT
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKINVO.MilkPriceVAT);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKINVO.MilkPriceVAT, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int NumberPointTotal
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKINVO.NumberPointTotal);
			}
			set
	        {
				base.Setint(ColumnNamesMILKINVO.NumberPointTotal, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double ProteinPercCooperation
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKINVO.ProteinPercCooperation);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKINVO.ProteinPercCooperation, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double ProteinPercInvoice
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKINVO.ProteinPercInvoice);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKINVO.ProteinPercInvoice, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double ProtGrPerLitMonthAvg
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKINVO.ProtGrPerLitMonthAvg);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKINVO.ProtGrPerLitMonthAvg, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int RestNumberPeriod
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKINVO.RestNumberPeriod);
			}
			set
	        {
				base.Setint(ColumnNamesMILKINVO.RestNumberPeriod, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string Street
	    {
			get
	        {
				return base.Getstring(ColumnNamesMILKINVO.Street);
			}
			set
	        {
				base.Setstring(ColumnNamesMILKINVO.Street, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double UreaCooperation
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKINVO.UreaCooperation);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKINVO.UreaCooperation, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double UreaCumulate
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKINVO.UreaCumulate);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKINVO.UreaCumulate, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double UreaInvoice
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKINVO.UreaInvoice);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKINVO.UreaInvoice, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int ValutaCode
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKINVO.ValutaCode);
			}
			set
	        {
				base.Setint(ColumnNamesMILKINVO.ValutaCode, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Versionnr
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKINVO.Versionnr);
			}
			set
	        {
				base.Setint(ColumnNamesMILKINVO.Versionnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string Zipcode
	    {
			get
	        {
				return base.Getstring(ColumnNamesMILKINVO.Zipcode);
			}
			set
	        {
				base.Setstring(ColumnNamesMILKINVO.Zipcode, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double LactosePercInvoice
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKINVO.LactosePercInvoice);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKINVO.LactosePercInvoice, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double LactosePercCooperation
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKINVO.LactosePercCooperation);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKINVO.LactosePercCooperation, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double LactoseGrPerLitMonthAvg
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMILKINVO.LactoseGrPerLitMonthAvg);
			}
			set
	        {
				base.Setdouble(ColumnNamesMILKINVO.LactoseGrPerLitMonthAvg, value);
			}
		}

		#endregion
		
	}
}
