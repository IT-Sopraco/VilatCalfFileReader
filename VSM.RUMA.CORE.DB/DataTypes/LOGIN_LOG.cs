
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
	public class LOGIN_LOG : DataObject
	{
		public LOGIN_LOG() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"LL_LoginCode",
            	"LL_Login_DateTime"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesLOGIN_LOG
		{  
            public const string LL_LoginCode = "LL_LoginCode";
            public const string LL_IP_Adress = "LL_IP_Adress";
            public const string LL_Login_DateTime = "LL_Login_DateTime";
            public const string LL_Memo = "LL_Memo";
            public const string LL_Successfully = "LL_Successfully";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public string LL_LoginCode
	    {
			get
	        {
				return base.Getstring(ColumnNamesLOGIN_LOG.LL_LoginCode);
			}
			set
	        {
				base.Setstring(ColumnNamesLOGIN_LOG.LL_LoginCode, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string LL_IP_Adress
	    {
			get
	        {
				return base.Getstring(ColumnNamesLOGIN_LOG.LL_IP_Adress);
			}
			set
	        {
				base.Setstring(ColumnNamesLOGIN_LOG.LL_IP_Adress, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public DateTime LL_Login_DateTime
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesLOGIN_LOG.LL_Login_DateTime);
			}
			set
	        {
				base.SetDateTime(ColumnNamesLOGIN_LOG.LL_Login_DateTime, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string LL_Memo
	    {
			get
	        {
				return base.Getstring(ColumnNamesLOGIN_LOG.LL_Memo);
			}
			set
	        {
				base.Setstring(ColumnNamesLOGIN_LOG.LL_Memo, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte LL_Successfully
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesLOGIN_LOG.LL_Successfully);
			}
			set
	        {
				base.Setsbyte(ColumnNamesLOGIN_LOG.LL_Successfully, value);
			}
		}


		#endregion
		
	}
}
