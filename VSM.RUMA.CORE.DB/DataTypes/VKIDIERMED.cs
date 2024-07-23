
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
	public class VKIDIERMED : DataObject
	{
		public VKIDIERMED() : base(Database.animal)
		{
			UpdateParams = new String[] 
			{
            	"Internalnr2"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesVKIDIERMED
		{  
            public const string Internalnr = "Internalnr";
            public const string AniId = "AniId";
            public const string Internalnr2 = "Internalnr2";
            public const string Vraag = "Vraag";
            public const string DiagnoseBehandeling = "DiagnoseBehandeling";
            public const string NaamMedicijn = "NaamMedicijn";
            public const string RegNLmedicijn = "RegNLmedicijn";
            public const string DatumLstBeh = "DatumLstBeh";
            public const string DatumEindeWT = "DatumEindeWT";
            public const string DiagBehString = "DiagBehString";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Internalnr
	    {
			get
	        {
				return base.Getint(ColumnNamesVKIDIERMED.Internalnr);
			}
			set
	        {
				base.Setint(ColumnNamesVKIDIERMED.Internalnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AniId
	    {
			get
	        {
				return base.Getint(ColumnNamesVKIDIERMED.AniId);
			}
			set
	        {
				base.Setint(ColumnNamesVKIDIERMED.AniId, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int Internalnr2
	    {
			get
	        {
				return base.Getint(ColumnNamesVKIDIERMED.Internalnr2);
			}
			set
	        {
				base.Setint(ColumnNamesVKIDIERMED.Internalnr2, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Vraag
	    {
			get
	        {
				return base.Getint(ColumnNamesVKIDIERMED.Vraag);
			}
			set
	        {
				base.Setint(ColumnNamesVKIDIERMED.Vraag, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string DiagnoseBehandeling
	    {
			get
	        {
				return base.Getstring(ColumnNamesVKIDIERMED.DiagnoseBehandeling);
			}
			set
	        {
				base.Setstring(ColumnNamesVKIDIERMED.DiagnoseBehandeling, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string NaamMedicijn
	    {
			get
	        {
				return base.Getstring(ColumnNamesVKIDIERMED.NaamMedicijn);
			}
			set
	        {
				base.Setstring(ColumnNamesVKIDIERMED.NaamMedicijn, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string RegNLmedicijn
	    {
			get
	        {
				return base.Getstring(ColumnNamesVKIDIERMED.RegNLmedicijn);
			}
			set
	        {
				base.Setstring(ColumnNamesVKIDIERMED.RegNLmedicijn, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime DatumLstBeh
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesVKIDIERMED.DatumLstBeh);
			}
			set
	        {
				base.SetDateTime(ColumnNamesVKIDIERMED.DatumLstBeh, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime DatumEindeWT
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesVKIDIERMED.DatumEindeWT);
			}
			set
	        {
				base.SetDateTime(ColumnNamesVKIDIERMED.DatumEindeWT, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string DiagBehString
	    {
			get
	        {
				return base.Getstring(ColumnNamesVKIDIERMED.DiagBehString);
			}
			set
	        {
				base.Setstring(ColumnNamesVKIDIERMED.DiagBehString, value);
			}
		}


		#endregion
		
	}
}