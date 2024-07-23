
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
	public class BANK_TRANSACTIE : DataObject
	{
		public BANK_TRANSACTIE() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"BT_ID"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesBANK_TRANSACTIE
		{  
            public const string BT_ID = "BT_ID";
            public const string BT_Transactie_ID = "BT_Transactie_ID";
            public const string BT_Eigen_ThrID = "BT_Eigen_ThrID";
            public const string BT_Eigen_IBAN = "BT_Eigen_IBAN";
            public const string BT_Tegenpartij_ThrID = "BT_Tegenpartij_ThrID";
            public const string BT_Tegenpartij_IBAN = "BT_Tegenpartij_IBAN";
            public const string BT_Tegenpartij_Naam = "BT_Tegenpartij_Naam";
            public const string BT_Tegenpartij_Adres = "BT_Tegenpartij_Adres";
            public const string BT_Tegenpartij_Woonplaats = "BT_Tegenpartij_Woonplaats";
            public const string BT_Afschriftnummer = "BT_Afschriftnummer";
            public const string BT_Rente_Datum = "BT_Rente_Datum";
            public const string BT_Boekdatum = "BT_Boekdatum";
            public const string BT_Bedrag = "BT_Bedrag";
            public const string BT_Credit = "BT_Credit";
            public const string BT_Omschrijving_Transactie = "BT_Omschrijving_Transactie";
            public const string BT_Bestandsnaam = "BT_Bestandsnaam";
            public const string BT_StatusID = "BT_StatusID";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int BT_ID
	    {
			get
	        {
				return base.Getint(ColumnNamesBANK_TRANSACTIE.BT_ID);
			}
			set
	        {
				base.Setint(ColumnNamesBANK_TRANSACTIE.BT_ID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string BT_Transactie_ID
	    {
			get
	        {
				return base.Getstring(ColumnNamesBANK_TRANSACTIE.BT_Transactie_ID);
			}
			set
	        {
				base.Setstring(ColumnNamesBANK_TRANSACTIE.BT_Transactie_ID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int BT_Eigen_ThrID
	    {
			get
	        {
				return base.Getint(ColumnNamesBANK_TRANSACTIE.BT_Eigen_ThrID);
			}
			set
	        {
				base.Setint(ColumnNamesBANK_TRANSACTIE.BT_Eigen_ThrID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string BT_Eigen_IBAN
	    {
			get
	        {
				return base.Getstring(ColumnNamesBANK_TRANSACTIE.BT_Eigen_IBAN);
			}
			set
	        {
				base.Setstring(ColumnNamesBANK_TRANSACTIE.BT_Eigen_IBAN, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int BT_Tegenpartij_ThrID
	    {
			get
	        {
				return base.Getint(ColumnNamesBANK_TRANSACTIE.BT_Tegenpartij_ThrID);
			}
			set
	        {
				base.Setint(ColumnNamesBANK_TRANSACTIE.BT_Tegenpartij_ThrID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string BT_Tegenpartij_IBAN
	    {
			get
	        {
				return base.Getstring(ColumnNamesBANK_TRANSACTIE.BT_Tegenpartij_IBAN);
			}
			set
	        {
				base.Setstring(ColumnNamesBANK_TRANSACTIE.BT_Tegenpartij_IBAN, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string BT_Tegenpartij_Naam
	    {
			get
	        {
				return base.Getstring(ColumnNamesBANK_TRANSACTIE.BT_Tegenpartij_Naam);
			}
			set
	        {
				base.Setstring(ColumnNamesBANK_TRANSACTIE.BT_Tegenpartij_Naam, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string BT_Tegenpartij_Adres
	    {
			get
	        {
				return base.Getstring(ColumnNamesBANK_TRANSACTIE.BT_Tegenpartij_Adres);
			}
			set
	        {
				base.Setstring(ColumnNamesBANK_TRANSACTIE.BT_Tegenpartij_Adres, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string BT_Tegenpartij_Woonplaats
	    {
			get
	        {
				return base.Getstring(ColumnNamesBANK_TRANSACTIE.BT_Tegenpartij_Woonplaats);
			}
			set
	        {
				base.Setstring(ColumnNamesBANK_TRANSACTIE.BT_Tegenpartij_Woonplaats, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string BT_Afschriftnummer
	    {
			get
	        {
				return base.Getstring(ColumnNamesBANK_TRANSACTIE.BT_Afschriftnummer);
			}
			set
	        {
				base.Setstring(ColumnNamesBANK_TRANSACTIE.BT_Afschriftnummer, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime BT_Rente_Datum
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesBANK_TRANSACTIE.BT_Rente_Datum);
			}
			set
	        {
				base.SetDateTime(ColumnNamesBANK_TRANSACTIE.BT_Rente_Datum, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime BT_Boekdatum
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesBANK_TRANSACTIE.BT_Boekdatum);
			}
			set
	        {
				base.SetDateTime(ColumnNamesBANK_TRANSACTIE.BT_Boekdatum, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double BT_Bedrag
	    {
			get
	        {
				return base.Getdouble(ColumnNamesBANK_TRANSACTIE.BT_Bedrag);
			}
			set
	        {
				base.Setdouble(ColumnNamesBANK_TRANSACTIE.BT_Bedrag, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte BT_Credit
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesBANK_TRANSACTIE.BT_Credit);
			}
			set
	        {
				base.Setsbyte(ColumnNamesBANK_TRANSACTIE.BT_Credit, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string BT_Omschrijving_Transactie
	    {
			get
	        {
				return base.Getstring(ColumnNamesBANK_TRANSACTIE.BT_Omschrijving_Transactie);
			}
			set
	        {
				base.Setstring(ColumnNamesBANK_TRANSACTIE.BT_Omschrijving_Transactie, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string BT_Bestandsnaam
	    {
			get
	        {
				return base.Getstring(ColumnNamesBANK_TRANSACTIE.BT_Bestandsnaam);
			}
			set
	        {
				base.Setstring(ColumnNamesBANK_TRANSACTIE.BT_Bestandsnaam, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int BT_StatusID
	    {
			get
	        {
				return base.Getint(ColumnNamesBANK_TRANSACTIE.BT_StatusID);
			}
			set
	        {
				base.Setint(ColumnNamesBANK_TRANSACTIE.BT_StatusID, value);
			}
		}


		#endregion
		
	}
}
