
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
	public class MBEGIN : DataObject
	{
		public MBEGIN() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"Mestnummer",
            	"Year",
            	"AnimalKind",
            	"AniCategory",
            	"Stalsysteem"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesMBEGIN
		{  
            public const string Mestnummer = "Mestnummer";
            public const string Year = "Year";
            public const string AnimalKind = "AnimalKind";
            public const string AniCategory = "AniCategory";
            public const string Stalsysteem = "Stalsysteem";
            public const string Amount = "Amount";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public string Mestnummer
	    {
			get
	        {
				return base.Getstring(ColumnNamesMBEGIN.Mestnummer);
			}
			set
	        {
				base.Setstring(ColumnNamesMBEGIN.Mestnummer, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int Year
	    {
			get
	        {
				return base.Getint(ColumnNamesMBEGIN.Year);
			}
			set
	        {
				base.Setint(ColumnNamesMBEGIN.Year, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int AnimalKind
	    {
			get
	        {
				return base.Getint(ColumnNamesMBEGIN.AnimalKind);
			}
			set
	        {
				base.Setint(ColumnNamesMBEGIN.AnimalKind, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int AniCategory
	    {
			get
	        {
				return base.Getint(ColumnNamesMBEGIN.AniCategory);
			}
			set
	        {
				base.Setint(ColumnNamesMBEGIN.AniCategory, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int Stalsysteem
	    {
			get
	        {
				return base.Getint(ColumnNamesMBEGIN.Stalsysteem);
			}
			set
	        {
				base.Setint(ColumnNamesMBEGIN.Stalsysteem, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Amount
	    {
			get
	        {
				return base.Getint(ColumnNamesMBEGIN.Amount);
			}
			set
	        {
				base.Setint(ColumnNamesMBEGIN.Amount, value);
			}
		}


		#endregion
		
	}
}
