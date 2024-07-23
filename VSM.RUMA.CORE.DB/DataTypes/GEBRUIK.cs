
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
	public class GEBRUIK : DataObject
	{
		public GEBRUIK() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"GbrPerceelID",
            	"GbrID"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesGEBRUIK
		{  
            public const string GbrPerceelID = "GbrPerceelID";
            public const string GbrID = "GbrID";
            public const string GbrDate = "GbrDate";
            public const string GbrOrder = "GbrOrder";
            public const string GbrKind = "GbrKind";
            public const string GbrRemark = "GbrRemark";
            public const string GbrComment = "GbrComment";
            public const string GbrThrID = "GbrThrID";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int GbrPerceelID
	    {
			get
	        {
				return base.Getint(ColumnNamesGEBRUIK.GbrPerceelID);
			}
			set
	        {
				base.Setint(ColumnNamesGEBRUIK.GbrPerceelID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int GbrID
	    {
			get
	        {
				return base.Getint(ColumnNamesGEBRUIK.GbrID);
			}
			set
	        {
				base.Setint(ColumnNamesGEBRUIK.GbrID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime GbrDate
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesGEBRUIK.GbrDate);
			}
			set
	        {
				base.SetDateTime(ColumnNamesGEBRUIK.GbrDate, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int GbrOrder
	    {
			get
	        {
				return base.Getint(ColumnNamesGEBRUIK.GbrOrder);
			}
			set
	        {
				base.Setint(ColumnNamesGEBRUIK.GbrOrder, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int GbrKind
	    {
			get
	        {
				return base.Getint(ColumnNamesGEBRUIK.GbrKind);
			}
			set
	        {
				base.Setint(ColumnNamesGEBRUIK.GbrKind, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int GbrRemark
	    {
			get
	        {
				return base.Getint(ColumnNamesGEBRUIK.GbrRemark);
			}
			set
	        {
				base.Setint(ColumnNamesGEBRUIK.GbrRemark, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string GbrComment
	    {
			get
	        {
				return base.Getstring(ColumnNamesGEBRUIK.GbrComment);
			}
			set
	        {
				base.Setstring(ColumnNamesGEBRUIK.GbrComment, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int GbrThrID
	    {
			get
	        {
				return base.Getint(ColumnNamesGEBRUIK.GbrThrID);
			}
			set
	        {
				base.Setint(ColumnNamesGEBRUIK.GbrThrID, value);
			}
		}


		#endregion
		
	}
}
