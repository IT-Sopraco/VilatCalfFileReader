
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
	public class REMMINAS : DataObject
	{
		public REMMINAS() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"Mestnummer",
            	"RemKind",
            	"RemId"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesREMMINAS
		{  
            public const string Mestnummer = "Mestnummer";
            public const string RemKind = "RemKind";
            public const string RemId = "RemId";
            public const string RemLabel = "RemLabel";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public string Mestnummer
	    {
			get
	        {
				return base.Getstring(ColumnNamesREMMINAS.Mestnummer);
			}
			set
	        {
				base.Setstring(ColumnNamesREMMINAS.Mestnummer, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int RemKind
	    {
			get
	        {
				return base.Getint(ColumnNamesREMMINAS.RemKind);
			}
			set
	        {
				base.Setint(ColumnNamesREMMINAS.RemKind, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int RemId
	    {
			get
	        {
				return base.Getint(ColumnNamesREMMINAS.RemId);
			}
			set
	        {
				base.Setint(ColumnNamesREMMINAS.RemId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string RemLabel
	    {
			get
	        {
				return base.Getstring(ColumnNamesREMMINAS.RemLabel);
			}
			set
	        {
				base.Setstring(ColumnNamesREMMINAS.RemLabel, value);
			}
		}


		#endregion
		
	}
}
