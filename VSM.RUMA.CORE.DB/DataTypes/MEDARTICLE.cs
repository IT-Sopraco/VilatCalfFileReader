
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
	public class MEDARTICLE : DataObject
	{
		public MEDARTICLE() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"MedId"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesMEDARTICLE
		{  
            public const string MedId = "MedId";
            public const string MedName = "MedName";
            public const string MedActiveProduct = "MedActiveProduct";
            public const string MedDaysWaitingMilk = "MedDaysWaitingMilk";
            public const string MedDaysWaitingMeat = "MedDaysWaitingMeat";
            public const string MedUnit = "MedUnit";
            public const string MedUnitVolume = "MedUnitVolume";
            public const string MedPrice = "MedPrice";
            public const string MedPriceEuro = "MedPriceEuro";
            public const string MedPriceUnit = "MedPriceUnit";
            public const string MedCurrencyEuro = "MedCurrencyEuro";
            public const string MedHoursWaitingMilk = "MedHoursWaitingMilk";
            public const string MedHoursWaitingMeat = "MedHoursWaitingMeat";
            public const string MedCode = "MedCode";
            public const string MedRegEu = "MedRegEu";
            public const string MedThrId = "MedThrId";
            public const string MedFunction = "MedFunction";
            public const string MedReg = "MedReg";
            public const string MedUDD = "MedUDD";
            public const string MedDisGroup = "MedDisGroup";
            public const string MedDaysTreat = "MedDaysTreat";
            public const string MedApply = "MedApply";
            public const string MedPreference = "MedPreference";
            public const string MedHoursRepeat = "MedHoursRepeat";
            public const string MedMP3file = "MedMP3file";
            public const string MedAmountPerXKg = "MedAmountPerXKg";
            public const string MedKgAliveWeight = "MedKgAliveWeight";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int MedId
	    {
			get
	        {
				return base.Getint(ColumnNamesMEDARTICLE.MedId);
			}
			set
	        {
				base.Setint(ColumnNamesMEDARTICLE.MedId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string MedName
	    {
			get
	        {
				return base.Getstring(ColumnNamesMEDARTICLE.MedName);
			}
			set
	        {
				base.Setstring(ColumnNamesMEDARTICLE.MedName, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string MedActiveProduct
	    {
			get
	        {
				return base.Getstring(ColumnNamesMEDARTICLE.MedActiveProduct);
			}
			set
	        {
				base.Setstring(ColumnNamesMEDARTICLE.MedActiveProduct, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int MedDaysWaitingMilk
	    {
			get
	        {
				return base.Getint(ColumnNamesMEDARTICLE.MedDaysWaitingMilk);
			}
			set
	        {
				base.Setint(ColumnNamesMEDARTICLE.MedDaysWaitingMilk, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int MedDaysWaitingMeat
	    {
			get
	        {
				return base.Getint(ColumnNamesMEDARTICLE.MedDaysWaitingMeat);
			}
			set
	        {
				base.Setint(ColumnNamesMEDARTICLE.MedDaysWaitingMeat, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int MedUnit
	    {
			get
	        {
				return base.Getint(ColumnNamesMEDARTICLE.MedUnit);
			}
			set
	        {
				base.Setint(ColumnNamesMEDARTICLE.MedUnit, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int MedUnitVolume
	    {
			get
	        {
				return base.Getint(ColumnNamesMEDARTICLE.MedUnitVolume);
			}
			set
	        {
				base.Setint(ColumnNamesMEDARTICLE.MedUnitVolume, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double MedPrice
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMEDARTICLE.MedPrice);
			}
			set
	        {
				base.Setdouble(ColumnNamesMEDARTICLE.MedPrice, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double MedPriceEuro
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMEDARTICLE.MedPriceEuro);
			}
			set
	        {
				base.Setdouble(ColumnNamesMEDARTICLE.MedPriceEuro, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int MedPriceUnit
	    {
			get
	        {
				return base.Getint(ColumnNamesMEDARTICLE.MedPriceUnit);
			}
			set
	        {
				base.Setint(ColumnNamesMEDARTICLE.MedPriceUnit, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte MedCurrencyEuro
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesMEDARTICLE.MedCurrencyEuro);
			}
			set
	        {
				base.Setsbyte(ColumnNamesMEDARTICLE.MedCurrencyEuro, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int MedHoursWaitingMilk
	    {
			get
	        {
				return base.Getint(ColumnNamesMEDARTICLE.MedHoursWaitingMilk);
			}
			set
	        {
				base.Setint(ColumnNamesMEDARTICLE.MedHoursWaitingMilk, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int MedHoursWaitingMeat
	    {
			get
	        {
				return base.Getint(ColumnNamesMEDARTICLE.MedHoursWaitingMeat);
			}
			set
	        {
				base.Setint(ColumnNamesMEDARTICLE.MedHoursWaitingMeat, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string MedCode
	    {
			get
	        {
				return base.Getstring(ColumnNamesMEDARTICLE.MedCode);
			}
			set
	        {
				base.Setstring(ColumnNamesMEDARTICLE.MedCode, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string MedRegEu
	    {
			get
	        {
				return base.Getstring(ColumnNamesMEDARTICLE.MedRegEu);
			}
			set
	        {
				base.Setstring(ColumnNamesMEDARTICLE.MedRegEu, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int MedThrId
	    {
			get
	        {
				return base.Getint(ColumnNamesMEDARTICLE.MedThrId);
			}
			set
	        {
				base.Setint(ColumnNamesMEDARTICLE.MedThrId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int MedFunction
	    {
			get
	        {
				return base.Getint(ColumnNamesMEDARTICLE.MedFunction);
			}
			set
	        {
				base.Setint(ColumnNamesMEDARTICLE.MedFunction, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte MedReg
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesMEDARTICLE.MedReg);
			}
			set
	        {
				base.Setsbyte(ColumnNamesMEDARTICLE.MedReg, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte MedUDD
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesMEDARTICLE.MedUDD);
			}
			set
	        {
				base.Setsbyte(ColumnNamesMEDARTICLE.MedUDD, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int MedDisGroup
	    {
			get
	        {
				return base.Getint(ColumnNamesMEDARTICLE.MedDisGroup);
			}
			set
	        {
				base.Setint(ColumnNamesMEDARTICLE.MedDisGroup, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int MedDaysTreat
	    {
			get
	        {
				return base.Getint(ColumnNamesMEDARTICLE.MedDaysTreat);
			}
			set
	        {
				base.Setint(ColumnNamesMEDARTICLE.MedDaysTreat, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int MedApply
	    {
			get
	        {
				return base.Getint(ColumnNamesMEDARTICLE.MedApply);
			}
			set
	        {
				base.Setint(ColumnNamesMEDARTICLE.MedApply, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int MedPreference
	    {
			get
	        {
				return base.Getint(ColumnNamesMEDARTICLE.MedPreference);
			}
			set
	        {
				base.Setint(ColumnNamesMEDARTICLE.MedPreference, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int MedHoursRepeat
	    {
			get
	        {
				return base.Getint(ColumnNamesMEDARTICLE.MedHoursRepeat);
			}
			set
	        {
				base.Setint(ColumnNamesMEDARTICLE.MedHoursRepeat, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string MedMP3file
	    {
			get
	        {
				return base.Getstring(ColumnNamesMEDARTICLE.MedMP3file);
			}
			set
	        {
				base.Setstring(ColumnNamesMEDARTICLE.MedMP3file, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double MedAmountPerXKg
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMEDARTICLE.MedAmountPerXKg);
			}
			set
	        {
				base.Setdouble(ColumnNamesMEDARTICLE.MedAmountPerXKg, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double MedKgAliveWeight
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMEDARTICLE.MedKgAliveWeight);
			}
			set
	        {
				base.Setdouble(ColumnNamesMEDARTICLE.MedKgAliveWeight, value);
			}
		}


		#endregion
		
	}
}
