
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
	public class TILES : DataObject
	{
		public TILES() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"Tile_Id",
            	"TileSet_Inhoud"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesTILES
		{  
            public const string Tile_Id = "Tile_Id";
            public const string ThrFarmId = "ThrFarmId";
            public const string TileIndex = "TileIndex";
            public const string TileSet_Inhoud = "TileSet_Inhoud";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int Tile_Id
	    {
			get
	        {
				return base.Getint(ColumnNamesTILES.Tile_Id);
			}
			set
	        {
				base.Setint(ColumnNamesTILES.Tile_Id, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int ThrFarmId
	    {
			get
	        {
				return base.Getint(ColumnNamesTILES.ThrFarmId);
			}
			set
	        {
				base.Setint(ColumnNamesTILES.ThrFarmId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int TileIndex
	    {
			get
	        {
				return base.Getint(ColumnNamesTILES.TileIndex);
			}
			set
	        {
				base.Setint(ColumnNamesTILES.TileIndex, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int TileSet_Inhoud
	    {
			get
	        {
				return base.Getint(ColumnNamesTILES.TileSet_Inhoud);
			}
			set
	        {
				base.Setint(ColumnNamesTILES.TileSet_Inhoud, value);
			}
		}


		#endregion
		
	}
}