
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
	public class TRANSPLA : DataObject
	{
		public TRANSPLA() : base(Database.animal)
		{
			UpdateParams = new String[] 
			{
            	"EventId"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesTRANSPLA
		{  
            public const string AniId = "AniId";
            public const string EventId = "EventId";
			public const string TrePlaceDate = "TrePlaceDate";
			public const string RemId = "RemId";
            public const string ThrId = "ThrId";
            public const string AniIdFather = "AniIdFather";
            public const string AniIdMother = "AniIdMother";
            public const string TreSex = "TreSex";
            public const string AgeEmbryo = "AgeEmbryo";
            public const string EmsId = "EmsId";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AniId
	    {
			get
	        {
				return base.Getint(ColumnNamesTRANSPLA.AniId);
			}
			set
	        {
				base.Setint(ColumnNamesTRANSPLA.AniId, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int EventId
	    {
			get
	        {
				return base.Getint(ColumnNamesTRANSPLA.EventId);
			}
			set
	        {
				base.Setint(ColumnNamesTRANSPLA.EventId, value);
			}
		}


		[System.ComponentModel.DataObjectField(false, false, false)]
		public DateTime TrePlaceDate
		{
			get
			{
				return base.GetDateTime(ColumnNamesTRANSPLA.TrePlaceDate);
			}
			set
			{
				base.SetDateTime(ColumnNamesTRANSPLA.TrePlaceDate, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int RemId
	    {
			get
	        {
				return base.Getint(ColumnNamesTRANSPLA.RemId);
			}
			set
	        {
				base.Setint(ColumnNamesTRANSPLA.RemId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int ThrId
	    {
			get
	        {
				return base.Getint(ColumnNamesTRANSPLA.ThrId);
			}
			set
	        {
				base.Setint(ColumnNamesTRANSPLA.ThrId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AniIdFather
	    {
			get
	        {
				return base.Getint(ColumnNamesTRANSPLA.AniIdFather);
			}
			set
	        {
				base.Setint(ColumnNamesTRANSPLA.AniIdFather, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AniIdMother
	    {
			get
	        {
				return base.Getint(ColumnNamesTRANSPLA.AniIdMother);
			}
			set
	        {
				base.Setint(ColumnNamesTRANSPLA.AniIdMother, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int TreSex
	    {
			get
	        {
				return base.Getint(ColumnNamesTRANSPLA.TreSex);
			}
			set
	        {
				base.Setint(ColumnNamesTRANSPLA.TreSex, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AgeEmbryo
	    {
			get
	        {
				return base.Getint(ColumnNamesTRANSPLA.AgeEmbryo);
			}
			set
	        {
				base.Setint(ColumnNamesTRANSPLA.AgeEmbryo, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int EmsId
	    {
			get
	        {
				return base.Getint(ColumnNamesTRANSPLA.EmsId);
			}
			set
	        {
				base.Setint(ColumnNamesTRANSPLA.EmsId, value);
			}
		}


		#endregion
		
	}
}
