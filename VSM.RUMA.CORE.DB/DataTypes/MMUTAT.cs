
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
	public class MMUTAT : DataObject
	{
		public MMUTAT() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"Internalnr"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesMMUTAT
		{  
            public const string Internalnr = "Internalnr";
            public const string Mestnummer = "Mestnummer";
            public const string Year = "Year";
            public const string MutDate = "MutDate";
            public const string AniGroup = "AniGroup";
            public const string AniCategory = "AniCategory";
            public const string AniSubCategory = "AniSubCategory";
            public const string Reason = "Reason";
            public const string YoungAnimal = "YoungAnimal";
            public const string Weigth = "Weigth";
            public const string Number = "Number";
            public const string Stalsysteem = "Stalsysteem";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int Internalnr
	    {
			get
	        {
				return base.Getint(ColumnNamesMMUTAT.Internalnr);
			}
			set
	        {
				base.Setint(ColumnNamesMMUTAT.Internalnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string Mestnummer
	    {
			get
	        {
				return base.Getstring(ColumnNamesMMUTAT.Mestnummer);
			}
			set
	        {
				base.Setstring(ColumnNamesMMUTAT.Mestnummer, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Year
	    {
			get
	        {
				return base.Getint(ColumnNamesMMUTAT.Year);
			}
			set
	        {
				base.Setint(ColumnNamesMMUTAT.Year, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime MutDate
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesMMUTAT.MutDate);
			}
			set
	        {
				base.SetDateTime(ColumnNamesMMUTAT.MutDate, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AniGroup
	    {
			get
	        {
				return base.Getint(ColumnNamesMMUTAT.AniGroup);
			}
			set
	        {
				base.Setint(ColumnNamesMMUTAT.AniGroup, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AniCategory
	    {
			get
	        {
				return base.Getint(ColumnNamesMMUTAT.AniCategory);
			}
			set
	        {
				base.Setint(ColumnNamesMMUTAT.AniCategory, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AniSubCategory
	    {
			get
	        {
				return base.Getint(ColumnNamesMMUTAT.AniSubCategory);
			}
			set
	        {
				base.Setint(ColumnNamesMMUTAT.AniSubCategory, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Reason
	    {
			get
	        {
				return base.Getint(ColumnNamesMMUTAT.Reason);
			}
			set
	        {
				base.Setint(ColumnNamesMMUTAT.Reason, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int YoungAnimal
	    {
			get
	        {
				return base.Getint(ColumnNamesMMUTAT.YoungAnimal);
			}
			set
	        {
				base.Setint(ColumnNamesMMUTAT.YoungAnimal, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double Weigth
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMMUTAT.Weigth);
			}
			set
	        {
				base.Setdouble(ColumnNamesMMUTAT.Weigth, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double Number
	    {
			get
	        {
				return base.Getdouble(ColumnNamesMMUTAT.Number);
			}
			set
	        {
				base.Setdouble(ColumnNamesMMUTAT.Number, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Stalsysteem
	    {
			get
	        {
				return base.Getint(ColumnNamesMMUTAT.Stalsysteem);
			}
			set
	        {
				base.Setint(ColumnNamesMMUTAT.Stalsysteem, value);
			}
		}


		#endregion
		
	}
}
