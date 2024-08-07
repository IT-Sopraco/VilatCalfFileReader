
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
	public class PSC_PROCESCOMPUTER : DataObject
	{
		public PSC_PROCESCOMPUTER() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"PspcId"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesPSC_PROCESCOMPUTER
		{  
            public const string PspcId = "pspcId";
            public const string PsId = "psId";
            public const string PspcType = "pspcType";
            public const string PspcmachineId = "pspcmachineId";
            public const string Pspcname = "pspcname";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, true, false)] 
		public int PspcId
	    {
			get
	        {
				return base.Getint(ColumnNamesPSC_PROCESCOMPUTER.PspcId);
			}
			set
	        {
				base.Setint(ColumnNamesPSC_PROCESCOMPUTER.PspcId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int PsId
	    {
			get
	        {
				return base.Getint(ColumnNamesPSC_PROCESCOMPUTER.PsId);
			}
			set
	        {
				base.Setint(ColumnNamesPSC_PROCESCOMPUTER.PsId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int PspcType
	    {
			get
	        {
				return base.Getint(ColumnNamesPSC_PROCESCOMPUTER.PspcType);
			}
			set
	        {
				base.Setint(ColumnNamesPSC_PROCESCOMPUTER.PspcType, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int PspcmachineId
	    {
			get
	        {
				return base.Getint(ColumnNamesPSC_PROCESCOMPUTER.PspcmachineId);
			}
			set
	        {
				base.Setint(ColumnNamesPSC_PROCESCOMPUTER.PspcmachineId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string Pspcname
	    {
			get
	        {
				return base.Getstring(ColumnNamesPSC_PROCESCOMPUTER.Pspcname);
			}
			set
	        {
				base.Setstring(ColumnNamesPSC_PROCESCOMPUTER.Pspcname, value);
			}
		}


		#endregion
		
	}
}
