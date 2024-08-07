
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
	public class GEMACHTIGD : DataObject
	{
		public GEMACHTIGD() : base(Database.agrolink)
		{
			UpdateParams = new String[] 
			{

			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesGEMACHTIGD
		{  
            public const string ThrId1 = "ThrId1";
            public const string ThrId2 = "ThrId2";
            public const string GemKind = "GemKind";
            public const string MachtigingId = "MachtigingId";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int ThrId1
	    {
			get
	        {
				return base.Getint(ColumnNamesGEMACHTIGD.ThrId1);
			}
			set
	        {
				base.Setint(ColumnNamesGEMACHTIGD.ThrId1, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int ThrId2
	    {
			get
	        {
				return base.Getint(ColumnNamesGEMACHTIGD.ThrId2);
			}
			set
	        {
				base.Setint(ColumnNamesGEMACHTIGD.ThrId2, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int GemKind
	    {
			get
	        {
				return base.Getint(ColumnNamesGEMACHTIGD.GemKind);
			}
			set
	        {
				base.Setint(ColumnNamesGEMACHTIGD.GemKind, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string MachtigingId
	    {
			get
	        {
				return base.Getstring(ColumnNamesGEMACHTIGD.MachtigingId);
			}
			set
	        {
				base.Setstring(ColumnNamesGEMACHTIGD.MachtigingId, value);
			}
		}


		#endregion
		
	}
}
