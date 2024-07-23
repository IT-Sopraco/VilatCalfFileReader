
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
	public class grootboe : DataObject
	{
		public grootboe() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"GrtbId"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesgrootboe
		{  
            public const string AdmisId = "AdmisId";
            public const string GrtbId = "GrtbId";
            public const string GrtbNummer = "GrtbNummer";
            public const string GrtbNaam = "GrtbNaam";
            public const string GrtbCodeBalans = "GrtbCodeBalans";
            public const string GrtbDebetCredit = "GrtbDebetCredit";
            public const string GrtbStandaardBTW = "GrtbStandaardBTW";
            public const string GrtbAantallenboeken = "GrtbAantallenboeken";
            public const string GrtbKilogrammenboeken = "GrtbKilogrammenboeken";
            public const string GrtbHoofdgroep = "GrtbHoofdgroep";
            public const string GrtbCategorie = "GrtbCategorie";
            public const string GrtbToonInkoop = "GrtbToonInkoop";
            public const string GrtbToonVerkoop = "GrtbToonVerkoop";
            public const string GrtbKasBankGiro = "GrtbKasBankGiro";
            public const string GrtbKengetalId = "GrtbKengetalId";
            public const string GrtbCodeBoekpakket = "GrtbCodeBoekpakket";
            public const string GrtbRekAccountant = "GrtbRekAccountant";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(false, false, false)] 
		public long AdmisId
	    {
			get
	        {
				return base.Getlong(ColumnNamesgrootboe.AdmisId);
			}
			set
	        {
				base.Setlong(ColumnNamesgrootboe.AdmisId, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, true, false)] 
		public long GrtbId
	    {
			get
	        {
				return base.Getlong(ColumnNamesgrootboe.GrtbId);
			}
			set
	        {
				base.Setlong(ColumnNamesgrootboe.GrtbId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string GrtbNummer
	    {
			get
	        {
				return base.Getstring(ColumnNamesgrootboe.GrtbNummer);
			}
			set
	        {
				base.Setstring(ColumnNamesgrootboe.GrtbNummer, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string GrtbNaam
	    {
			get
	        {
				return base.Getstring(ColumnNamesgrootboe.GrtbNaam);
			}
			set
	        {
				base.Setstring(ColumnNamesgrootboe.GrtbNaam, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public long GrtbCodeBalans
	    {
			get
	        {
				return base.Getlong(ColumnNamesgrootboe.GrtbCodeBalans);
			}
			set
	        {
				base.Setlong(ColumnNamesgrootboe.GrtbCodeBalans, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte GrtbDebetCredit
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesgrootboe.GrtbDebetCredit);
			}
			set
	        {
				base.Setsbyte(ColumnNamesgrootboe.GrtbDebetCredit, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public long GrtbStandaardBTW
	    {
			get
	        {
				return base.Getlong(ColumnNamesgrootboe.GrtbStandaardBTW);
			}
			set
	        {
				base.Setlong(ColumnNamesgrootboe.GrtbStandaardBTW, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte GrtbAantallenboeken
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesgrootboe.GrtbAantallenboeken);
			}
			set
	        {
				base.Setsbyte(ColumnNamesgrootboe.GrtbAantallenboeken, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte GrtbKilogrammenboeken
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesgrootboe.GrtbKilogrammenboeken);
			}
			set
	        {
				base.Setsbyte(ColumnNamesgrootboe.GrtbKilogrammenboeken, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public long GrtbHoofdgroep
	    {
			get
	        {
				return base.Getlong(ColumnNamesgrootboe.GrtbHoofdgroep);
			}
			set
	        {
				base.Setlong(ColumnNamesgrootboe.GrtbHoofdgroep, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public long GrtbCategorie
	    {
			get
	        {
				return base.Getlong(ColumnNamesgrootboe.GrtbCategorie);
			}
			set
	        {
				base.Setlong(ColumnNamesgrootboe.GrtbCategorie, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte GrtbToonInkoop
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesgrootboe.GrtbToonInkoop);
			}
			set
	        {
				base.Setsbyte(ColumnNamesgrootboe.GrtbToonInkoop, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte GrtbToonVerkoop
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesgrootboe.GrtbToonVerkoop);
			}
			set
	        {
				base.Setsbyte(ColumnNamesgrootboe.GrtbToonVerkoop, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte GrtbKasBankGiro
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesgrootboe.GrtbKasBankGiro);
			}
			set
	        {
				base.Setsbyte(ColumnNamesgrootboe.GrtbKasBankGiro, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public long GrtbKengetalId
	    {
			get
	        {
				return base.Getlong(ColumnNamesgrootboe.GrtbKengetalId);
			}
			set
	        {
				base.Setlong(ColumnNamesgrootboe.GrtbKengetalId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string GrtbCodeBoekpakket
	    {
			get
	        {
				return base.Getstring(ColumnNamesgrootboe.GrtbCodeBoekpakket);
			}
			set
	        {
				base.Setstring(ColumnNamesgrootboe.GrtbCodeBoekpakket, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string GrtbRekAccountant
	    {
			get
	        {
				return base.Getstring(ColumnNamesgrootboe.GrtbRekAccountant);
			}
			set
	        {
				base.Setstring(ColumnNamesgrootboe.GrtbRekAccountant, value);
			}
		}


		#endregion
		
	}
}
