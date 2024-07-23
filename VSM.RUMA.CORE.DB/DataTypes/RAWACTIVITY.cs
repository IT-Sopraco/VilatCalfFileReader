
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
	public class RAWACTIVITY : DataObject
	{
		public RAWACTIVITY() : base(Database.agrodata)
		{
			UpdateParams = new String[] 
			{

			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesRAWACTIVITY
		{  
            public const string FarmId = "FarmId";
            public const string AniId = "AniId";
            public const string RADateTime = "RADateTime";
            public const string RAValue = "RAValue";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int FarmId
	    {
			get
	        {
				return base.Getint(ColumnNamesRAWACTIVITY.FarmId);
			}
			set
	        {
				base.Setint(ColumnNamesRAWACTIVITY.FarmId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AniId
	    {
			get
	        {
				return base.Getint(ColumnNamesRAWACTIVITY.AniId);
			}
			set
	        {
				base.Setint(ColumnNamesRAWACTIVITY.AniId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime RADateTime
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesRAWACTIVITY.RADateTime);
			}
			set
	        {
				base.SetDateTime(ColumnNamesRAWACTIVITY.RADateTime, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double RAValue
	    {
			get
	        {
				return base.Getdouble(ColumnNamesRAWACTIVITY.RAValue);
			}
			set
	        {
				base.Setdouble(ColumnNamesRAWACTIVITY.RAValue, value);
			}
		}


		#endregion
		
	}
}
