
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
	public class TILES_SETTINGS : DataObject
	{
		public TILES_SETTINGS() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"TileSet_Id",
            	"TileSet_ProgramId",
            	"TileSet_Inhoud"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesTILES_SETTINGS
		{  
            public const string TileSet_Inhoud = "TileSet_Inhoud";
            public const string TileSet_ProgramId = "TileSet_ProgramId";
            public const string TileSet_Id = "TileSet_Id";
            public const string TileSet_Title = "TileSet_Title";
            public const string TileSet_Query = "TileSet_Query";
            public const string TileSet_Weergave1 = "TileSet_Weergave1";
            public const string TileSet_Weergave2 = "TileSet_Weergave2";
            public const string TileSet_Weergave3 = "TileSet_Weergave3";
            public const string TileSet_Weergave4 = "TileSet_Weergave4";
            public const string TileSet_StandWeergave = "TileSet_StandWeergave";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int TileSet_Inhoud
	    {
			get
	        {
				return base.Getint(ColumnNamesTILES_SETTINGS.TileSet_Inhoud);
			}
			set
	        {
				base.Setint(ColumnNamesTILES_SETTINGS.TileSet_Inhoud, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int TileSet_ProgramId
	    {
			get
	        {
				return base.Getint(ColumnNamesTILES_SETTINGS.TileSet_ProgramId);
			}
			set
	        {
				base.Setint(ColumnNamesTILES_SETTINGS.TileSet_ProgramId, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int TileSet_Id
	    {
			get
	        {
				return base.Getint(ColumnNamesTILES_SETTINGS.TileSet_Id);
			}
			set
	        {
				base.Setint(ColumnNamesTILES_SETTINGS.TileSet_Id, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string TileSet_Title
	    {
			get
	        {
				return base.Getstring(ColumnNamesTILES_SETTINGS.TileSet_Title);
			}
			set
	        {
				base.Setstring(ColumnNamesTILES_SETTINGS.TileSet_Title, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string TileSet_Query
	    {
			get
	        {
				return base.Getstring(ColumnNamesTILES_SETTINGS.TileSet_Query);
			}
			set
	        {
				base.Setstring(ColumnNamesTILES_SETTINGS.TileSet_Query, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte TileSet_Weergave1
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesTILES_SETTINGS.TileSet_Weergave1);
			}
			set
	        {
				base.Setsbyte(ColumnNamesTILES_SETTINGS.TileSet_Weergave1, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte TileSet_Weergave2
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesTILES_SETTINGS.TileSet_Weergave2);
			}
			set
	        {
				base.Setsbyte(ColumnNamesTILES_SETTINGS.TileSet_Weergave2, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte TileSet_Weergave3
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesTILES_SETTINGS.TileSet_Weergave3);
			}
			set
	        {
				base.Setsbyte(ColumnNamesTILES_SETTINGS.TileSet_Weergave3, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte TileSet_Weergave4
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesTILES_SETTINGS.TileSet_Weergave4);
			}
			set
	        {
				base.Setsbyte(ColumnNamesTILES_SETTINGS.TileSet_Weergave4, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte TileSet_StandWeergave
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesTILES_SETTINGS.TileSet_StandWeergave);
			}
			set
	        {
				base.Setsbyte(ColumnNamesTILES_SETTINGS.TileSet_StandWeergave, value);
			}
		}


		#endregion
		
	}
}
