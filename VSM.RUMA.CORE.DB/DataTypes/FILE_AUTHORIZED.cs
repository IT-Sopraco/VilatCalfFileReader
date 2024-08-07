
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
	public class FILE_AUTHORIZED : DataObject
	{
		public FILE_AUTHORIZED() : base(Database.agrolink)
		{
			UpdateParams = new String[] 
			{
            	"ThrId1",
            	"ThrId2",
            	"FileId"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesFILE_AUTHORIZED
		{  
            public const string ThrId1 = "ThrId1";
            public const string ThrId2 = "ThrId2";
            public const string FileId = "FileId";
            public const string INS = "INS";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int ThrId1
	    {
			get
	        {
				return base.Getint(ColumnNamesFILE_AUTHORIZED.ThrId1);
			}
			set
	        {
				base.Setint(ColumnNamesFILE_AUTHORIZED.ThrId1, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int ThrId2
	    {
			get
	        {
				return base.Getint(ColumnNamesFILE_AUTHORIZED.ThrId2);
			}
			set
	        {
				base.Setint(ColumnNamesFILE_AUTHORIZED.ThrId2, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int FileId
	    {
			get
	        {
				return base.Getint(ColumnNamesFILE_AUTHORIZED.FileId);
			}
			set
	        {
				base.Setint(ColumnNamesFILE_AUTHORIZED.FileId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime INS
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesFILE_AUTHORIZED.INS);
			}
			set
	        {
				base.SetDateTime(ColumnNamesFILE_AUTHORIZED.INS, value);
			}
		}


		#endregion
		
	}
}
