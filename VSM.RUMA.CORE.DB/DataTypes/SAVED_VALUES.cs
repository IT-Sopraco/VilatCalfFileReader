
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
	public class SAVED_VALUES : DataObject
	{
		public SAVED_VALUES() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"SvId"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesSAVED_VALUES
		{  
            public const string SvId = "svId";
            public const string ProgramId = "programId";
            public const string FarmId = "farmId";
            public const string SvType = "svType";
            public const string SvTitle = "svTitle";
            public const string SvDateTime = "svDateTime";
            public const string SvComment = "svComment";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int SvId
	    {
			get
	        {
				return base.Getint(ColumnNamesSAVED_VALUES.SvId);
			}
			set
	        {
				base.Setint(ColumnNamesSAVED_VALUES.SvId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int ProgramId
	    {
			get
	        {
				return base.Getint(ColumnNamesSAVED_VALUES.ProgramId);
			}
			set
	        {
				base.Setint(ColumnNamesSAVED_VALUES.ProgramId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int FarmId
	    {
			get
	        {
				return base.Getint(ColumnNamesSAVED_VALUES.FarmId);
			}
			set
	        {
				base.Setint(ColumnNamesSAVED_VALUES.FarmId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int SvType
	    {
			get
	        {
				return base.Getint(ColumnNamesSAVED_VALUES.SvType);
			}
			set
	        {
				base.Setint(ColumnNamesSAVED_VALUES.SvType, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string SvTitle
	    {
			get
	        {
				return base.Getstring(ColumnNamesSAVED_VALUES.SvTitle);
			}
			set
	        {
				base.Setstring(ColumnNamesSAVED_VALUES.SvTitle, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime SvDateTime
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesSAVED_VALUES.SvDateTime);
			}
			set
	        {
				base.SetDateTime(ColumnNamesSAVED_VALUES.SvDateTime, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string SvComment
	    {
			get
	        {
				return base.Getstring(ColumnNamesSAVED_VALUES.SvComment);
			}
			set
	        {
				base.Setstring(ColumnNamesSAVED_VALUES.SvComment, value);
			}
		}


		#endregion
		
	}
}
