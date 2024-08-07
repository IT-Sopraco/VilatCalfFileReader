
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
	public class MESTTANK : DataObject
	{
		public MESTTANK() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{

			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesMESTTANK
		{  
            public const string Mestnummer = "Mestnummer";
            public const string Tanknummer = "Tanknummer";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string Mestnummer
	    {
			get
	        {
				return base.Getstring(ColumnNamesMESTTANK.Mestnummer);
			}
			set
	        {
				base.Setstring(ColumnNamesMESTTANK.Mestnummer, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Tanknummer
	    {
			get
	        {
				return base.Getint(ColumnNamesMESTTANK.Tanknummer);
			}
			set
	        {
				base.Setint(ColumnNamesMESTTANK.Tanknummer, value);
			}
		}


		#endregion
		
	}
}
