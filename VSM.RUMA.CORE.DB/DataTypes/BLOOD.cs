
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
	public class BLOOD : DataObject
	{
		public BLOOD() : base(Database.animal)
		{
			UpdateParams = new String[] 
			{
            	"EventId"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesBLOOD
		{  
            public const string EventId = "EventId";
            public const string BloKind = "BloKind";
            public const string BloResult = "BloResult";
            public const string BloReason = "BloReason";
            public const string BloResultText = "BloResultText";
            public const string BloAuthorized = "BloAuthorized";
            public const string BloAuthorizedByThrID = "BloAuthorizedByThrID";
            public const string BloAuthorizedDate = "BloAuthorizedDate";
            public const string BloAuthorizedComment = "BloAuthorizedComment";
            public const string BloType = "BloType";
            public const string BloStatus = "BloStatus";
            public const string BloNumber = "BloNumber";
            public const string BRD_BloID = "BRD_BloID";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int EventId
	    {
			get
	        {
				return base.Getint(ColumnNamesBLOOD.EventId);
			}
			set
	        {
				base.Setint(ColumnNamesBLOOD.EventId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int BloKind
	    {
			get
	        {
				return base.Getint(ColumnNamesBLOOD.BloKind);
			}
			set
	        {
				base.Setint(ColumnNamesBLOOD.BloKind, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double BloResult
	    {
			get
	        {
				return base.Getdouble(ColumnNamesBLOOD.BloResult);
			}
			set
	        {
				base.Setdouble(ColumnNamesBLOOD.BloResult, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int BloReason
	    {
			get
	        {
				return base.Getint(ColumnNamesBLOOD.BloReason);
			}
			set
	        {
				base.Setint(ColumnNamesBLOOD.BloReason, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int BloResultText
	    {
			get
	        {
				return base.Getint(ColumnNamesBLOOD.BloResultText);
			}
			set
	        {
				base.Setint(ColumnNamesBLOOD.BloResultText, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)]
        public int BloAuthorized
	    {
			get
	        {
                return base.Getint(ColumnNamesBLOOD.BloAuthorized);
			}
			set
	        {
                base.Setint(ColumnNamesBLOOD.BloAuthorized, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int BloAuthorizedByThrID
	    {
			get
	        {
				return base.Getint(ColumnNamesBLOOD.BloAuthorizedByThrID);
			}
			set
	        {
				base.Setint(ColumnNamesBLOOD.BloAuthorizedByThrID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime BloAuthorizedDate
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesBLOOD.BloAuthorizedDate);
			}
			set
	        {
				base.SetDateTime(ColumnNamesBLOOD.BloAuthorizedDate, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string BloAuthorizedComment
	    {
			get
	        {
				return base.Getstring(ColumnNamesBLOOD.BloAuthorizedComment);
			}
			set
	        {
				base.Setstring(ColumnNamesBLOOD.BloAuthorizedComment, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public short BloType
	    {
			get
	        {
				return base.Getshort(ColumnNamesBLOOD.BloType);
			}
			set
	        {
				base.Setshort(ColumnNamesBLOOD.BloType, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public short BloStatus
	    {
			get
	        {
				return base.Getshort(ColumnNamesBLOOD.BloStatus);
			}
			set
	        {
				base.Setshort(ColumnNamesBLOOD.BloStatus, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public short BloNumber
	    {
			get
	        {
				return base.Getshort(ColumnNamesBLOOD.BloNumber);
			}
			set
	        {
				base.Setshort(ColumnNamesBLOOD.BloNumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int BRD_BloID
	    {
			get
	        {
				return base.Getint(ColumnNamesBLOOD.BRD_BloID);
			}
			set
	        {
				base.Setint(ColumnNamesBLOOD.BRD_BloID, value);
			}
		}


		#endregion
		
	}
}