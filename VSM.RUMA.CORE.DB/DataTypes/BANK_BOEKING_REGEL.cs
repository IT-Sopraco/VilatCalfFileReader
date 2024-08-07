
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
	public class BANK_BOEKING_REGEL : DataObject
	{
		public BANK_BOEKING_REGEL() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{

			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesBANK_BOEKING_REGEL
		{  
            public const string BB_ID = "BB_ID";
            public const string BBR_Regel_Nr = "BBR_Regel_Nr";
            public const string BBR_Regel_Omschrijving = "BBR_Regel_Omschrijving";
            public const string BBR_Regel_Datum = "BBR_Regel_Datum";
            public const string BBR_Betaling = "BBR_Betaling";
            public const string BBR_Regel_Bedrag = "BBR_Regel_Bedrag";
            public const string FactId = "FactId";
            public const string SupplyID = "SupplyID";
            public const string BT_ID = "BT_ID";
            public const string ME_ID = "ME_ID";
            public const string MER_Regel_NR = "MER_Regel_NR";
            public const string BBR_ThrID = "BBR_ThrID";
            public const string BBR_IBAN = "BBR_IBAN";
            public const string GrtbId = "GrtbId";
            public const string BBR_Directe_Boeking = "BBR_Directe_Boeking";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int BB_ID
	    {
			get
	        {
				return base.Getint(ColumnNamesBANK_BOEKING_REGEL.BB_ID);
			}
			set
	        {
				base.Setint(ColumnNamesBANK_BOEKING_REGEL.BB_ID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int BBR_Regel_Nr
	    {
			get
	        {
				return base.Getint(ColumnNamesBANK_BOEKING_REGEL.BBR_Regel_Nr);
			}
			set
	        {
				base.Setint(ColumnNamesBANK_BOEKING_REGEL.BBR_Regel_Nr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string BBR_Regel_Omschrijving
	    {
			get
	        {
				return base.Getstring(ColumnNamesBANK_BOEKING_REGEL.BBR_Regel_Omschrijving);
			}
			set
	        {
				base.Setstring(ColumnNamesBANK_BOEKING_REGEL.BBR_Regel_Omschrijving, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime BBR_Regel_Datum
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesBANK_BOEKING_REGEL.BBR_Regel_Datum);
			}
			set
	        {
				base.SetDateTime(ColumnNamesBANK_BOEKING_REGEL.BBR_Regel_Datum, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte BBR_Betaling
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesBANK_BOEKING_REGEL.BBR_Betaling);
			}
			set
	        {
				base.Setsbyte(ColumnNamesBANK_BOEKING_REGEL.BBR_Betaling, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double BBR_Regel_Bedrag
	    {
			get
	        {
				return base.Getdouble(ColumnNamesBANK_BOEKING_REGEL.BBR_Regel_Bedrag);
			}
			set
	        {
				base.Setdouble(ColumnNamesBANK_BOEKING_REGEL.BBR_Regel_Bedrag, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int FactId
	    {
			get
	        {
				return base.Getint(ColumnNamesBANK_BOEKING_REGEL.FactId);
			}
			set
	        {
				base.Setint(ColumnNamesBANK_BOEKING_REGEL.FactId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int SupplyID
	    {
			get
	        {
				return base.Getint(ColumnNamesBANK_BOEKING_REGEL.SupplyID);
			}
			set
	        {
				base.Setint(ColumnNamesBANK_BOEKING_REGEL.SupplyID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int BT_ID
	    {
			get
	        {
				return base.Getint(ColumnNamesBANK_BOEKING_REGEL.BT_ID);
			}
			set
	        {
				base.Setint(ColumnNamesBANK_BOEKING_REGEL.BT_ID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int ME_ID
	    {
			get
	        {
				return base.Getint(ColumnNamesBANK_BOEKING_REGEL.ME_ID);
			}
			set
	        {
				base.Setint(ColumnNamesBANK_BOEKING_REGEL.ME_ID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int MER_Regel_NR
	    {
			get
	        {
				return base.Getint(ColumnNamesBANK_BOEKING_REGEL.MER_Regel_NR);
			}
			set
	        {
				base.Setint(ColumnNamesBANK_BOEKING_REGEL.MER_Regel_NR, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int BBR_ThrID
	    {
			get
	        {
				return base.Getint(ColumnNamesBANK_BOEKING_REGEL.BBR_ThrID);
			}
			set
	        {
				base.Setint(ColumnNamesBANK_BOEKING_REGEL.BBR_ThrID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string BBR_IBAN
	    {
			get
	        {
				return base.Getstring(ColumnNamesBANK_BOEKING_REGEL.BBR_IBAN);
			}
			set
	        {
				base.Setstring(ColumnNamesBANK_BOEKING_REGEL.BBR_IBAN, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int GrtbId
	    {
			get
	        {
				return base.Getint(ColumnNamesBANK_BOEKING_REGEL.GrtbId);
			}
			set
	        {
				base.Setint(ColumnNamesBANK_BOEKING_REGEL.GrtbId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte BBR_Directe_Boeking
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesBANK_BOEKING_REGEL.BBR_Directe_Boeking);
			}
			set
	        {
				base.Setsbyte(ColumnNamesBANK_BOEKING_REGEL.BBR_Directe_Boeking, value);
			}
		}


		#endregion
		
	}
}
