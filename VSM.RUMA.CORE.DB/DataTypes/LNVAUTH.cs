
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
	public class LNVAUTH : DataObject
	{
		public LNVAUTH() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{

			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesLNVAUTH
		{  
            public const string FarmNumber = "FarmNumber";
            public const string Program = "Program";
            public const string AuthorizedFarmNumber = "AuthorizedFarmNumber";
            public const string AurhorizedBRS = "AurhorizedBRS";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string FarmNumber
	    {
			get
	        {
				return base.Getstring(ColumnNamesLNVAUTH.FarmNumber);
			}
			set
	        {
				base.Setstring(ColumnNamesLNVAUTH.FarmNumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Program
	    {
			get
	        {
				return base.Getint(ColumnNamesLNVAUTH.Program);
			}
			set
	        {
				base.Setint(ColumnNamesLNVAUTH.Program, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string AuthorizedFarmNumber
	    {
			get
	        {
				return base.Getstring(ColumnNamesLNVAUTH.AuthorizedFarmNumber);
			}
			set
	        {
				base.Setstring(ColumnNamesLNVAUTH.AuthorizedFarmNumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string AurhorizedBRS
	    {
			get
	        {
				return base.Getstring(ColumnNamesLNVAUTH.AurhorizedBRS);
			}
			set
	        {
				base.Setstring(ColumnNamesLNVAUTH.AurhorizedBRS, value);
			}
		}


		#endregion
		
	}
}
