
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
	public class EVENT : DataObject
	{
		public EVENT() : base(Database.animal)
		{
			UpdateParams = new String[] 
			{
            	"EventId",
            	"AniId"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesEVENT
		{  
            public const string AniId = "AniId";
            public const string EveDate = "EveDate";
            public const string EveOrder = "EveOrder";
            public const string EveKind = "EveKind";
            public const string EventId = "EventId";
            public const string UBNId = "UBNId";
            public const string RemId = "RemId";
            public const string ThirdId = "ThirdId";
            public const string Tbv_ThrID = "tbv_ThrID";
            public const string EveComment = "EveComment";
            public const string EveMutationDate = "EveMutationDate";
            public const string EveMutationTime = "EveMutationTime";
            public const string EveMutationBy = "EveMutationBy";
            public const string Happened_at_FarmID = "happened_at_FarmID";
            public const string Eve_RUMA_EventID = "Eve_RUMA_EventID";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int AniId
	    {
			get
	        {
				return base.Getint(ColumnNamesEVENT.AniId);
			}
			set
	        {
				base.Setint(ColumnNamesEVENT.AniId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime EveDate
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesEVENT.EveDate);
			}
			set
	        {
				base.SetDateTime(ColumnNamesEVENT.EveDate, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int EveOrder
	    {
			get
	        {
				return base.Getint(ColumnNamesEVENT.EveOrder);
			}
			set
	        {
				base.Setint(ColumnNamesEVENT.EveOrder, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int EveKind
	    {
			get
	        {
				return base.Getint(ColumnNamesEVENT.EveKind);
			}
			set
	        {
				base.Setint(ColumnNamesEVENT.EveKind, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int EventId
	    {
			get
	        {
				return base.Getint(ColumnNamesEVENT.EventId);
			}
			set
	        {
				base.Setint(ColumnNamesEVENT.EventId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int UBNId
	    {
			get
	        {
				return base.Getint(ColumnNamesEVENT.UBNId);
			}
			set
	        {
				base.Setint(ColumnNamesEVENT.UBNId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int RemId
	    {
			get
	        {
				return base.Getint(ColumnNamesEVENT.RemId);
			}
			set
	        {
				base.Setint(ColumnNamesEVENT.RemId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int ThirdId
	    {
			get
	        {
				return base.Getint(ColumnNamesEVENT.ThirdId);
			}
			set
	        {
				base.Setint(ColumnNamesEVENT.ThirdId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int tbv_ThrID
	    {
			get
	        {
				return base.Getint(ColumnNamesEVENT.Tbv_ThrID);
			}
			set
	        {
				base.Setint(ColumnNamesEVENT.Tbv_ThrID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string EveComment
	    {
			get
	        {
				return base.Getstring(ColumnNamesEVENT.EveComment);
			}
			set
	        {
				base.Setstring(ColumnNamesEVENT.EveComment, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime EveMutationDate
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesEVENT.EveMutationDate);
			}
			set
	        {
				base.SetDateTime(ColumnNamesEVENT.EveMutationDate, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime EveMutationTime
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesEVENT.EveMutationTime);
			}
			set
	        {
				base.SetDateTime(ColumnNamesEVENT.EveMutationTime, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int EveMutationBy
	    {
			get
	        {
				return base.Getint(ColumnNamesEVENT.EveMutationBy);
			}
			set
	        {
				base.Setint(ColumnNamesEVENT.EveMutationBy, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int happened_at_FarmID
	    {
			get
	        {
				return base.Getint(ColumnNamesEVENT.Happened_at_FarmID);
			}
			set
	        {
				base.Setint(ColumnNamesEVENT.Happened_at_FarmID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Eve_RUMA_EventID
	    {
			get
	        {
				return base.Getint(ColumnNamesEVENT.Eve_RUMA_EventID);
			}
			set
	        {
				base.Setint(ColumnNamesEVENT.Eve_RUMA_EventID, value);
			}
		}


		#endregion
		
	}
}
