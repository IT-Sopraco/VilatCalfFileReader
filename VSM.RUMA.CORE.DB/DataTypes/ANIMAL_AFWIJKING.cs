
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
	public class ANIMAL_AFWIJKING : DataObject
	{
		public ANIMAL_AFWIJKING() : base(Database.animal)
		{
			UpdateParams = new String[] 
			{
            	"AA_ID"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesANIMAL_AFWIJKING
		{  
            public const string AA_ID = "AA_ID";
            public const string AniId = "AniId";
            public const string Datum = "Datum";
            public const string AfwijkingKind = "AfwijkingKind";
            public const string AfwijkingID = "AfwijkingID";
            public const string AA_Type = "AA_Type";
            public const string AA_Waarde = "AA_Waarde";
            public const string AA_by_ThrID = "AA_by_ThrID";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int AA_ID
	    {
			get
	        {
				return base.Getint(ColumnNamesANIMAL_AFWIJKING.AA_ID);
			}
			set
	        {
				base.Setint(ColumnNamesANIMAL_AFWIJKING.AA_ID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AniId
	    {
			get
	        {
				return base.Getint(ColumnNamesANIMAL_AFWIJKING.AniId);
			}
			set
	        {
				base.Setint(ColumnNamesANIMAL_AFWIJKING.AniId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime Datum
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesANIMAL_AFWIJKING.Datum);
			}
			set
	        {
				base.SetDateTime(ColumnNamesANIMAL_AFWIJKING.Datum, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AfwijkingKind
	    {
			get
	        {
				return base.Getint(ColumnNamesANIMAL_AFWIJKING.AfwijkingKind);
			}
			set
	        {
				base.Setint(ColumnNamesANIMAL_AFWIJKING.AfwijkingKind, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AfwijkingID
	    {
			get
	        {
				return base.Getint(ColumnNamesANIMAL_AFWIJKING.AfwijkingID);
			}
			set
	        {
				base.Setint(ColumnNamesANIMAL_AFWIJKING.AfwijkingID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AA_Type
	    {
			get
	        {
				return base.Getint(ColumnNamesANIMAL_AFWIJKING.AA_Type);
			}
			set
	        {
				base.Setint(ColumnNamesANIMAL_AFWIJKING.AA_Type, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string AA_Waarde
	    {
			get
	        {
				return base.Getstring(ColumnNamesANIMAL_AFWIJKING.AA_Waarde);
			}
			set
	        {
				base.Setstring(ColumnNamesANIMAL_AFWIJKING.AA_Waarde, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AA_by_ThrID
	    {
			get
	        {
				return base.Getint(ColumnNamesANIMAL_AFWIJKING.AA_by_ThrID);
			}
			set
	        {
				base.Setint(ColumnNamesANIMAL_AFWIJKING.AA_by_ThrID, value);
			}
		}


		#endregion
		
	}
}
