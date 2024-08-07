
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
	public class MACHTIGING : DataObject
	{
		public MACHTIGING() : base(Database.agrolink)
		{
			UpdateParams = new String[] 
			{
            	"Id"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesMACHTIGING
		{  
            public const string Id = "Id";
            public const string CountryId = "CountryId";
            public const string Omschrijving = "Omschrijving";
            public const string Link = "Link";
            public const string Width = "Width";
            public const string Height = "Height";
            public const string EnabledPages = "EnabledPages";
            public const string EnabledScrolling = "EnabledScrolling";
            public const string Rechten = "Rechten";
            public const string MachtKind = "MachtKind";
            public const string ProgramId = "ProgramId";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int Id
	    {
			get
	        {
				return base.Getint(ColumnNamesMACHTIGING.Id);
			}
			set
	        {
				base.Setint(ColumnNamesMACHTIGING.Id, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int CountryId
	    {
			get
	        {
				return base.Getint(ColumnNamesMACHTIGING.CountryId);
			}
			set
	        {
				base.Setint(ColumnNamesMACHTIGING.CountryId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string Omschrijving
	    {
			get
	        {
				return base.Getstring(ColumnNamesMACHTIGING.Omschrijving);
			}
			set
	        {
				base.Setstring(ColumnNamesMACHTIGING.Omschrijving, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string Link
	    {
			get
	        {
				return base.Getstring(ColumnNamesMACHTIGING.Link);
			}
			set
	        {
				base.Setstring(ColumnNamesMACHTIGING.Link, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Width
	    {
			get
	        {
				return base.Getint(ColumnNamesMACHTIGING.Width);
			}
			set
	        {
				base.Setint(ColumnNamesMACHTIGING.Width, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Height
	    {
			get
	        {
				return base.Getint(ColumnNamesMACHTIGING.Height);
			}
			set
	        {
				base.Setint(ColumnNamesMACHTIGING.Height, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string EnabledPages
	    {
			get
	        {
				return base.Getstring(ColumnNamesMACHTIGING.EnabledPages);
			}
			set
	        {
				base.Setstring(ColumnNamesMACHTIGING.EnabledPages, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte EnabledScrolling
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesMACHTIGING.EnabledScrolling);
			}
			set
	        {
				base.Setsbyte(ColumnNamesMACHTIGING.EnabledScrolling, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public long Rechten
	    {
			get
	        {
				return base.Getlong(ColumnNamesMACHTIGING.Rechten);
			}
			set
	        {
				base.Setlong(ColumnNamesMACHTIGING.Rechten, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int MachtKind
	    {
			get
	        {
				return base.Getint(ColumnNamesMACHTIGING.MachtKind);
			}
			set
	        {
				base.Setint(ColumnNamesMACHTIGING.MachtKind, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int ProgramId
	    {
			get
	        {
				return base.Getint(ColumnNamesMACHTIGING.ProgramId);
			}
			set
	        {
				base.Setint(ColumnNamesMACHTIGING.ProgramId, value);
			}
		}


		#endregion
		
	}
}
