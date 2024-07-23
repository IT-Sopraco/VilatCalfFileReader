
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
	public class ANALYSE : DataObject
	{
		public ANALYSE() : base(Database.animal)
		{
			UpdateParams = new String[] 
			{
            	"AniId",
            	"AnaMilkDate"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesANALYSE
		{  
            public const string AniId = "AniId";
            public const string AnaMilkDate = "AnaMilkDate";
            public const string UbnId = "UbnId";
            public const string AnaCalfDate = "AnaCalfDate";
            public const string AnaMemo = "AnaMemo";
            public const string AnaKgMilk = "AnaKgMilk";
            public const string AnaPercFat = "AnaPercFat";
            public const string AnaPercProtein = "AnaPercProtein";
            public const string AnaPercFat_Original = "AnaPercFat_Original";
            public const string AnaPercProtein_Original = "AnaPercProtein_Original";
            public const string AnaMilkCellcount = "AnaMilkCellcount";
            public const string AnaTypeOfControl = "AnaTypeOfControl";
            public const string AnaBSKValue = "AnaBSKValue";
            public const string AnaKgMilk305 = "AnaKgMilk305";
            public const string AnaKgFat305 = "AnaKgFat305";
            public const string AnaKgProtein305 = "AnaKgProtein305";
            public const string AnaPercFat305 = "AnaPercFat305";
            public const string AnaPercProtein305 = "AnaPercProtein305";
            public const string AnaUrea = "AnaUrea";
            public const string AnaLactose = "AnaLactose";
            public const string AnaCummulatedKg = "AnaCummulatedKg";
            public const string AnaCummulatedPercFat = "AnaCummulatedPercFat";
            public const string AnaCummulatedPercProtein = "AnaCummulatedPercProtein";
            public const string AnaNumberBottle = "AnaNumberBottle";
            public const string AnaKgMilk1 = "AnaKgMilk1";
            public const string AnaKgMilk2 = "AnaKgMilk2";
            public const string AnaKgMilk3 = "AnaKgMilk3";
            public const string AnaKgMilk4 = "AnaKgMilk4";
            public const string AnaLactationValue = "AnaLactationValue";
            public const string AnaLactNr = "AnaLactNr";
            public const string ThirdID = "ThirdID";
            public const string BaseForExpected = "BaseForExpected";
            public const string ExpectedProduction = "ExpectedProduction";
            public const string NumberOfFalls = "NumberOfFalls";
            public const string Codemilkcount = "Codemilkcount";
            public const string AnaNettIncome = "AnaNettIncome";
            public const string AnaKgMilkSt = "AnaKgMilkSt";
            public const string AnaAnimalStatus = "AnaAnimalStatus";
            public const string AnaIndAlternate = "AnaIndAlternate";
            public const string AnaStatusFict = "AnaStatusFict";
            public const string AnaCurrencyEuro = "AnaCurrencyEuro";
            public const string AnaIndKetose = "AnaIndKetose";
            public const string AnaKgMilkUnreliable = "AnaKgMilkUnreliable";
            public const string AnaPercFatUnreliable = "AnaPercFatUnreliable";
            public const string AnaPercProtUnreliable = "AnaPercProtUnreliable";
            public const string Ana_FW_Disabled = "Ana_FW_Disabled";
            public const string Ana_Corrected_By = "Ana_Corrected_By";

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int AniId
	    {
			get
	        {
				return base.Getint(ColumnNamesANALYSE.AniId);
			}
			set
	        {
				base.Setint(ColumnNamesANALYSE.AniId, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public DateTime AnaMilkDate
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesANALYSE.AnaMilkDate);
			}
			set
	        {
				base.SetDateTime(ColumnNamesANALYSE.AnaMilkDate, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int UbnId
	    {
			get
	        {
				return base.Getint(ColumnNamesANALYSE.UbnId);
			}
			set
	        {
				base.Setint(ColumnNamesANALYSE.UbnId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime AnaCalfDate
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesANALYSE.AnaCalfDate);
			}
			set
	        {
				base.SetDateTime(ColumnNamesANALYSE.AnaCalfDate, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string AnaMemo
	    {
			get
	        {
				return base.Getstring(ColumnNamesANALYSE.AnaMemo);
			}
			set
	        {
				base.Setstring(ColumnNamesANALYSE.AnaMemo, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double AnaKgMilk
	    {
			get
	        {
				return base.Getdouble(ColumnNamesANALYSE.AnaKgMilk);
			}
			set
	        {
				base.Setdouble(ColumnNamesANALYSE.AnaKgMilk, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double AnaPercFat
	    {
			get
	        {
				return base.Getdouble(ColumnNamesANALYSE.AnaPercFat);
			}
			set
	        {
				base.Setdouble(ColumnNamesANALYSE.AnaPercFat, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double AnaPercProtein
	    {
			get
	        {
				return base.Getdouble(ColumnNamesANALYSE.AnaPercProtein);
			}
			set
	        {
				base.Setdouble(ColumnNamesANALYSE.AnaPercProtein, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double AnaPercFat_Original
	    {
			get
	        {
				return base.Getdouble(ColumnNamesANALYSE.AnaPercFat_Original);
			}
			set
	        {
				base.Setdouble(ColumnNamesANALYSE.AnaPercFat_Original, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double AnaPercProtein_Original
	    {
			get
	        {
				return base.Getdouble(ColumnNamesANALYSE.AnaPercProtein_Original);
			}
			set
	        {
				base.Setdouble(ColumnNamesANALYSE.AnaPercProtein_Original, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double AnaMilkCellcount
	    {
			get
	        {
				return base.Getdouble(ColumnNamesANALYSE.AnaMilkCellcount);
			}
			set
	        {
				base.Setdouble(ColumnNamesANALYSE.AnaMilkCellcount, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AnaTypeOfControl
	    {
			get
	        {
				return base.Getint(ColumnNamesANALYSE.AnaTypeOfControl);
			}
			set
	        {
				base.Setint(ColumnNamesANALYSE.AnaTypeOfControl, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double AnaBSKValue
	    {
			get
	        {
				return base.Getdouble(ColumnNamesANALYSE.AnaBSKValue);
			}
			set
	        {
				base.Setdouble(ColumnNamesANALYSE.AnaBSKValue, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AnaKgMilk305
	    {
			get
	        {
				return base.Getint(ColumnNamesANALYSE.AnaKgMilk305);
			}
			set
	        {
				base.Setint(ColumnNamesANALYSE.AnaKgMilk305, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AnaKgFat305
	    {
			get
	        {
				return base.Getint(ColumnNamesANALYSE.AnaKgFat305);
			}
			set
	        {
				base.Setint(ColumnNamesANALYSE.AnaKgFat305, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AnaKgProtein305
	    {
			get
	        {
				return base.Getint(ColumnNamesANALYSE.AnaKgProtein305);
			}
			set
	        {
				base.Setint(ColumnNamesANALYSE.AnaKgProtein305, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double AnaPercFat305
	    {
			get
	        {
				return base.Getdouble(ColumnNamesANALYSE.AnaPercFat305);
			}
			set
	        {
				base.Setdouble(ColumnNamesANALYSE.AnaPercFat305, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double AnaPercProtein305
	    {
			get
	        {
				return base.Getdouble(ColumnNamesANALYSE.AnaPercProtein305);
			}
			set
	        {
				base.Setdouble(ColumnNamesANALYSE.AnaPercProtein305, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double AnaUrea
	    {
			get
	        {
				return base.Getdouble(ColumnNamesANALYSE.AnaUrea);
			}
			set
	        {
				base.Setdouble(ColumnNamesANALYSE.AnaUrea, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double AnaLactose
	    {
			get
	        {
				return base.Getdouble(ColumnNamesANALYSE.AnaLactose);
			}
			set
	        {
				base.Setdouble(ColumnNamesANALYSE.AnaLactose, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AnaCummulatedKg
	    {
			get
	        {
				return base.Getint(ColumnNamesANALYSE.AnaCummulatedKg);
			}
			set
	        {
				base.Setint(ColumnNamesANALYSE.AnaCummulatedKg, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double AnaCummulatedPercFat
	    {
			get
	        {
				return base.Getdouble(ColumnNamesANALYSE.AnaCummulatedPercFat);
			}
			set
	        {
				base.Setdouble(ColumnNamesANALYSE.AnaCummulatedPercFat, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double AnaCummulatedPercProtein
	    {
			get
	        {
				return base.Getdouble(ColumnNamesANALYSE.AnaCummulatedPercProtein);
			}
			set
	        {
				base.Setdouble(ColumnNamesANALYSE.AnaCummulatedPercProtein, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AnaNumberBottle
	    {
			get
	        {
				return base.Getint(ColumnNamesANALYSE.AnaNumberBottle);
			}
			set
	        {
				base.Setint(ColumnNamesANALYSE.AnaNumberBottle, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double AnaKgMilk1
	    {
			get
	        {
				return base.Getdouble(ColumnNamesANALYSE.AnaKgMilk1);
			}
			set
	        {
				base.Setdouble(ColumnNamesANALYSE.AnaKgMilk1, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double AnaKgMilk2
	    {
			get
	        {
				return base.Getdouble(ColumnNamesANALYSE.AnaKgMilk2);
			}
			set
	        {
				base.Setdouble(ColumnNamesANALYSE.AnaKgMilk2, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double AnaKgMilk3
	    {
			get
	        {
				return base.Getdouble(ColumnNamesANALYSE.AnaKgMilk3);
			}
			set
	        {
				base.Setdouble(ColumnNamesANALYSE.AnaKgMilk3, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double AnaKgMilk4
	    {
			get
	        {
				return base.Getdouble(ColumnNamesANALYSE.AnaKgMilk4);
			}
			set
	        {
				base.Setdouble(ColumnNamesANALYSE.AnaKgMilk4, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AnaLactationValue
	    {
			get
	        {
				return base.Getint(ColumnNamesANALYSE.AnaLactationValue);
			}
			set
	        {
				base.Setint(ColumnNamesANALYSE.AnaLactationValue, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AnaLactNr
	    {
			get
	        {
				return base.Getint(ColumnNamesANALYSE.AnaLactNr);
			}
			set
	        {
				base.Setint(ColumnNamesANALYSE.AnaLactNr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string ThirdID
	    {
			get
	        {
				return base.Getstring(ColumnNamesANALYSE.ThirdID);
			}
			set
	        {
				base.Setstring(ColumnNamesANALYSE.ThirdID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double BaseForExpected
	    {
			get
	        {
				return base.Getdouble(ColumnNamesANALYSE.BaseForExpected);
			}
			set
	        {
				base.Setdouble(ColumnNamesANALYSE.BaseForExpected, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double ExpectedProduction
	    {
			get
	        {
				return base.Getdouble(ColumnNamesANALYSE.ExpectedProduction);
			}
			set
	        {
				base.Setdouble(ColumnNamesANALYSE.ExpectedProduction, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int NumberOfFalls
	    {
			get
	        {
				return base.Getint(ColumnNamesANALYSE.NumberOfFalls);
			}
			set
	        {
				base.Setint(ColumnNamesANALYSE.NumberOfFalls, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Codemilkcount
	    {
			get
	        {
				return base.Getint(ColumnNamesANALYSE.Codemilkcount);
			}
			set
	        {
				base.Setint(ColumnNamesANALYSE.Codemilkcount, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AnaNettIncome
	    {
			get
	        {
				return base.Getint(ColumnNamesANALYSE.AnaNettIncome);
			}
			set
	        {
				base.Setint(ColumnNamesANALYSE.AnaNettIncome, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double AnaKgMilkSt
	    {
			get
	        {
				return base.Getdouble(ColumnNamesANALYSE.AnaKgMilkSt);
			}
			set
	        {
				base.Setdouble(ColumnNamesANALYSE.AnaKgMilkSt, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AnaAnimalStatus
	    {
			get
	        {
				return base.Getint(ColumnNamesANALYSE.AnaAnimalStatus);
			}
			set
	        {
				base.Setint(ColumnNamesANALYSE.AnaAnimalStatus, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AnaIndAlternate
	    {
			get
	        {
				return base.Getint(ColumnNamesANALYSE.AnaIndAlternate);
			}
			set
	        {
				base.Setint(ColumnNamesANALYSE.AnaIndAlternate, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AnaStatusFict
	    {
			get
	        {
				return base.Getint(ColumnNamesANALYSE.AnaStatusFict);
			}
			set
	        {
				base.Setint(ColumnNamesANALYSE.AnaStatusFict, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte AnaCurrencyEuro
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesANALYSE.AnaCurrencyEuro);
			}
			set
	        {
				base.Setsbyte(ColumnNamesANALYSE.AnaCurrencyEuro, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AnaIndKetose
	    {
			get
	        {
                return base.Getint(ColumnNamesANALYSE.AnaIndKetose);
			}
			set
	        {
                base.Setint(ColumnNamesANALYSE.AnaIndKetose, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AnaKgMilkUnreliable
	    {
			get
	        {
				return base.Getint(ColumnNamesANALYSE.AnaKgMilkUnreliable);
			}
			set
	        {
				base.Setint(ColumnNamesANALYSE.AnaKgMilkUnreliable, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AnaPercFatUnreliable
	    {
			get
	        {
				return base.Getint(ColumnNamesANALYSE.AnaPercFatUnreliable);
			}
			set
	        {
				base.Setint(ColumnNamesANALYSE.AnaPercFatUnreliable, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AnaPercProtUnreliable
	    {
			get
	        {
				return base.Getint(ColumnNamesANALYSE.AnaPercProtUnreliable);
			}
			set
	        {
				base.Setint(ColumnNamesANALYSE.AnaPercProtUnreliable, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte Ana_FW_Disabled
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesANALYSE.Ana_FW_Disabled);
			}
			set
	        {
				base.Setsbyte(ColumnNamesANALYSE.Ana_FW_Disabled, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte Ana_Corrected_By
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesANALYSE.Ana_Corrected_By);
			}
			set
	        {
				base.Setsbyte(ColumnNamesANALYSE.Ana_Corrected_By, value);
			}
		}

		#endregion
		
	}
}