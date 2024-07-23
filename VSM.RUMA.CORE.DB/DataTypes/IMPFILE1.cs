
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
	public class IMPFILE1 : DataObject
	{
		public IMPFILE1() : base(Database.animal)
		{
			UpdateParams = new String[] 
			{
            	"ImpFileID"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesIMPFILE1
		{  
            public const string ImpFileID = "ImpFileID";
            public const string ImpFileName = "ImpFileName";
            public const string ImpFileDate = "ImpFileDate";
            public const string ImpFileTime = "ImpFileTime";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int ImpFileID
	    {
			get
	        {
				return base.Getint(ColumnNamesIMPFILE1.ImpFileID);
			}
			set
	        {
				base.Setint(ColumnNamesIMPFILE1.ImpFileID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string ImpFileName
	    {
			get
	        {
				return base.Getstring(ColumnNamesIMPFILE1.ImpFileName);
			}
			set
	        {
				base.Setstring(ColumnNamesIMPFILE1.ImpFileName, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime ImpFileDate
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesIMPFILE1.ImpFileDate);
			}
			set
	        {
				base.SetDateTime(ColumnNamesIMPFILE1.ImpFileDate, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime ImpFileTime
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesIMPFILE1.ImpFileTime);
			}
			set
	        {
				base.SetDateTime(ColumnNamesIMPFILE1.ImpFileTime, value);
			}
		}


		#endregion
		
	}
}
