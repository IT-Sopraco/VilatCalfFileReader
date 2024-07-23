
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
	public class TILES_SETTINGS_DETAIL : DataObject
	{
		public TILES_SETTINGS_DETAIL() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"TileSetD_Id",
            	"TileSet_ProgramId",
            	"TileSet_Inhoud"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesTILES_SETTINGS_DETAIL
		{  
            public const string TileSet_Inhoud = "TileSet_Inhoud";
            public const string TileSet_ProgramId = "TileSet_ProgramId";
            public const string TileSetD_Id = "TileSetD_Id";
            public const string TileSet_Weergave = "TileSet_Weergave";
            public const string TileSetD_AllowMinValue = "TileSetD_AllowMinValue";
            public const string TileSetD_AllowMaxValue = "TileSetD_AllowMaxValue";
            public const string TileSetD_AllowMinAlarm = "TileSetD_AllowMinAlarm";
            public const string TileSetD_AllowMaxAlarm = "TileSetD_AllowMaxAlarm";
            public const string TileSetD_MinWaarde = "TileSetD_MinWaarde";
            public const string TileSetD_MaxWaarde = "TileSetD_MaxWaarde";
            public const string TileSetD_MinAlarmWaarde = "TileSetD_MinAlarmWaarde";
            public const string TileSetD_MaxAlarmWaarde = "TileSetD_MaxAlarmWaarde";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int TileSet_Inhoud
	    {
			get
	        {
				return base.Getint(ColumnNamesTILES_SETTINGS_DETAIL.TileSet_Inhoud);
			}
			set
	        {
				base.Setint(ColumnNamesTILES_SETTINGS_DETAIL.TileSet_Inhoud, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public sbyte TileSet_ProgramId
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesTILES_SETTINGS_DETAIL.TileSet_ProgramId);
			}
			set
	        {
				base.Setsbyte(ColumnNamesTILES_SETTINGS_DETAIL.TileSet_ProgramId, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int TileSetD_Id
	    {
			get
	        {
				return base.Getint(ColumnNamesTILES_SETTINGS_DETAIL.TileSetD_Id);
			}
			set
	        {
				base.Setint(ColumnNamesTILES_SETTINGS_DETAIL.TileSetD_Id, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte TileSet_Weergave
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesTILES_SETTINGS_DETAIL.TileSet_Weergave);
			}
			set
	        {
				base.Setsbyte(ColumnNamesTILES_SETTINGS_DETAIL.TileSet_Weergave, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte TileSetD_AllowMinValue
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesTILES_SETTINGS_DETAIL.TileSetD_AllowMinValue);
			}
			set
	        {
				base.Setsbyte(ColumnNamesTILES_SETTINGS_DETAIL.TileSetD_AllowMinValue, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte TileSetD_AllowMaxValue
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesTILES_SETTINGS_DETAIL.TileSetD_AllowMaxValue);
			}
			set
	        {
				base.Setsbyte(ColumnNamesTILES_SETTINGS_DETAIL.TileSetD_AllowMaxValue, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte TileSetD_AllowMinAlarm
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesTILES_SETTINGS_DETAIL.TileSetD_AllowMinAlarm);
			}
			set
	        {
				base.Setsbyte(ColumnNamesTILES_SETTINGS_DETAIL.TileSetD_AllowMinAlarm, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte TileSetD_AllowMaxAlarm
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesTILES_SETTINGS_DETAIL.TileSetD_AllowMaxAlarm);
			}
			set
	        {
				base.Setsbyte(ColumnNamesTILES_SETTINGS_DETAIL.TileSetD_AllowMaxAlarm, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double TileSetD_MinWaarde
	    {
			get
	        {
				return base.Getdouble(ColumnNamesTILES_SETTINGS_DETAIL.TileSetD_MinWaarde);
			}
			set
	        {
				base.Setdouble(ColumnNamesTILES_SETTINGS_DETAIL.TileSetD_MinWaarde, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double TileSetD_MaxWaarde
	    {
			get
	        {
				return base.Getdouble(ColumnNamesTILES_SETTINGS_DETAIL.TileSetD_MaxWaarde);
			}
			set
	        {
				base.Setdouble(ColumnNamesTILES_SETTINGS_DETAIL.TileSetD_MaxWaarde, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double TileSetD_MinAlarmWaarde
	    {
			get
	        {
				return base.Getdouble(ColumnNamesTILES_SETTINGS_DETAIL.TileSetD_MinAlarmWaarde);
			}
			set
	        {
				base.Setdouble(ColumnNamesTILES_SETTINGS_DETAIL.TileSetD_MinAlarmWaarde, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double TileSetD_MaxAlarmWaarde
	    {
			get
	        {
				return base.Getdouble(ColumnNamesTILES_SETTINGS_DETAIL.TileSetD_MaxAlarmWaarde);
			}
			set
	        {
				base.Setdouble(ColumnNamesTILES_SETTINGS_DETAIL.TileSetD_MaxAlarmWaarde, value);
			}
		}


		#endregion
		
	}
}