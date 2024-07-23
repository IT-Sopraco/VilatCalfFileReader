
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
	public class WEIGHT : DataObject
	{
		public WEIGHT() : base(Database.animal)
		{
			UpdateParams = new String[] 
			{
            	"AniId",
            	"WeightDate",
            	"WeightOrder"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesWEIGHT
		{  
            public const string AniId = "AniId";
            public const string WeightDate = "WeightDate";
            public const string WeightOrder = "WeightOrder";
            public const string WeightKg = "WeightKg";
            public const string ChestMeasure = "ChestMeasure";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int AniId
	    {
			get
	        {
				return base.Getint(ColumnNamesWEIGHT.AniId);
			}
			set
	        {
				base.Setint(ColumnNamesWEIGHT.AniId, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public DateTime WeightDate
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesWEIGHT.WeightDate);
			}
			set
	        {
				base.SetDateTime(ColumnNamesWEIGHT.WeightDate, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int WeightOrder
	    {
			get
	        {
				return base.Getint(ColumnNamesWEIGHT.WeightOrder);
			}
			set
	        {
				base.Setint(ColumnNamesWEIGHT.WeightOrder, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double WeightKg
	    {
			get
	        {
				return base.Getdouble(ColumnNamesWEIGHT.WeightKg);
			}
			set
	        {
				base.Setdouble(ColumnNamesWEIGHT.WeightKg, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double ChestMeasure
	    {
			get
	        {
				return base.Getdouble(ColumnNamesWEIGHT.ChestMeasure);
			}
			set
	        {
				base.Setdouble(ColumnNamesWEIGHT.ChestMeasure, value);
			}
		}


		#endregion
		
	}
}