
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
	public class MILKQUAL : DataObject
	{
		public MILKQUAL() : base(Database.agrofactuur)
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
            	"TimeDeliveryMilk",
            	"NumberMilkinvest"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesMILKQUAL
		{  
            public const string UbnID = "UbnID";
            public const string ProdEenheidnr = "ProdEenheidnr";
            public const string CooperationNumber = "CooperationNumber";
            public const string SupplierNumber = "SupplierNumber";
            public const string InvoiceNumberYear = "InvoiceNumberYear";
            public const string AfrekVolgnr = "AfrekVolgnr";
            public const string DateDeliveryMilk = "DateDeliveryMilk";
            public const string TimeDeliveryMilk = "TimeDeliveryMilk";
            public const string NumberMilkinvest = "NumberMilkinvest";
            public const string DateEndInvoice = "DateEndInvoice";
            public const string DateStartInvoice = "DateStartInvoice";
            public const string Description = "Description";
            public const string Gradatie = "Gradatie";
            public const string NumberPoints = "NumberPoints";
            public const string ResultQualityInvest = "ResultQualityInvest";
            public const string ResultQualityNew = "ResultQualityNew";
		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int UbnID
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKQUAL.UbnID);
			}
			set
	        {
				base.Setint(ColumnNamesMILKQUAL.UbnID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public string ProdEenheidnr
	    {
			get
	        {
				return base.Getstring(ColumnNamesMILKQUAL.ProdEenheidnr);
			}
			set
	        {
				base.Setstring(ColumnNamesMILKQUAL.ProdEenheidnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int CooperationNumber
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKQUAL.CooperationNumber);
			}
			set
	        {
				base.Setint(ColumnNamesMILKQUAL.CooperationNumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int SupplierNumber
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKQUAL.SupplierNumber);
			}
			set
	        {
				base.Setint(ColumnNamesMILKQUAL.SupplierNumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int InvoiceNumberYear
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKQUAL.InvoiceNumberYear);
			}
			set
	        {
				base.Setint(ColumnNamesMILKQUAL.InvoiceNumberYear, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int AfrekVolgnr
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKQUAL.AfrekVolgnr);
			}
			set
	        {
				base.Setint(ColumnNamesMILKQUAL.AfrekVolgnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public DateTime DateDeliveryMilk
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesMILKQUAL.DateDeliveryMilk);
			}
			set
	        {
				base.SetDateTime(ColumnNamesMILKQUAL.DateDeliveryMilk, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public DateTime TimeDeliveryMilk
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesMILKQUAL.TimeDeliveryMilk);
			}
			set
	        {
				base.SetDateTime(ColumnNamesMILKQUAL.TimeDeliveryMilk, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int NumberMilkinvest
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKQUAL.NumberMilkinvest);
			}
			set
	        {
				base.Setint(ColumnNamesMILKQUAL.NumberMilkinvest, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime DateEndInvoice
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesMILKQUAL.DateEndInvoice);
			}
			set
	        {
				base.SetDateTime(ColumnNamesMILKQUAL.DateEndInvoice, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime DateStartInvoice
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesMILKQUAL.DateStartInvoice);
			}
			set
	        {
				base.SetDateTime(ColumnNamesMILKQUAL.DateStartInvoice, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string Description
	    {
			get
	        {
				return base.Getstring(ColumnNamesMILKQUAL.Description);
			}
			set
	        {
				base.Setstring(ColumnNamesMILKQUAL.Description, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Gradatie
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKQUAL.Gradatie);
			}
			set
	        {
				base.Setint(ColumnNamesMILKQUAL.Gradatie, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int NumberPoints
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKQUAL.NumberPoints);
			}
			set
	        {
				base.Setint(ColumnNamesMILKQUAL.NumberPoints, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int ResultQualityInvest
	    {
			get
	        {
				return base.Getint(ColumnNamesMILKQUAL.ResultQualityInvest);
			}
			set
	        {
				base.Setint(ColumnNamesMILKQUAL.ResultQualityInvest, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string ResultQualityNew
	    {
			get
	        {
				return base.Getstring(ColumnNamesMILKQUAL.ResultQualityNew);
			}
			set
	        {
				base.Setstring(ColumnNamesMILKQUAL.ResultQualityNew, value);
			}
		}


		#endregion
		
	}
}