
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
	public class SUPPLY1_VOER_VERDELING : DataObject
	{
		public SUPPLY1_VOER_VERDELING() : base(Database.agrofactuur)
		{
			UpdateParams = new String[] 
			{
            	"SupplyID",
            	"SVV_ProgID"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesSUPPLY1_VOER_VERDELING
		{  
            public const string SupplyID = "SupplyID";
            public const string SVV_ProgID = "SVV_ProgID";
            public const string SVV_KG_Jongvee = "SVV_KG_Jongvee";
            public const string SVV_KG_Overig = "SVV_KG_Overig";
            public const string SVV_KG_Toegewezen = "SVV_KG_Toegewezen"; 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int SupplyID
	    {
			get
	        {
				return base.Getint(ColumnNamesSUPPLY1_VOER_VERDELING.SupplyID);
			}
			set
	        {
				base.Setint(ColumnNamesSUPPLY1_VOER_VERDELING.SupplyID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int SVV_ProgID
	    {
			get
	        {
				return base.Getint(ColumnNamesSUPPLY1_VOER_VERDELING.SVV_ProgID);
			}
			set
	        {
				base.Setint(ColumnNamesSUPPLY1_VOER_VERDELING.SVV_ProgID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double SVV_KG_Jongvee
	    {
			get
	        {
				return base.Getdouble(ColumnNamesSUPPLY1_VOER_VERDELING.SVV_KG_Jongvee);
			}
			set
	        {
				base.Setdouble(ColumnNamesSUPPLY1_VOER_VERDELING.SVV_KG_Jongvee, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double SVV_KG_Overig
	    {
			get
	        {
				return base.Getdouble(ColumnNamesSUPPLY1_VOER_VERDELING.SVV_KG_Overig);
			}
			set
	        {
				base.Setdouble(ColumnNamesSUPPLY1_VOER_VERDELING.SVV_KG_Overig, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double SVV_KG_Toegewezen
	    {
			get
	        {
				return base.Getdouble(ColumnNamesSUPPLY1_VOER_VERDELING.SVV_KG_Toegewezen);
			}
			set
	        {
				base.Setdouble(ColumnNamesSUPPLY1_VOER_VERDELING.SVV_KG_Toegewezen, value);
			}
		}


		#endregion
		
	}
}
