
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
	public class GESTATIO : DataObject
	{
		public GESTATIO() : base(Database.animal)
		{
			UpdateParams = new String[] 
			{
            	"EventId"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesGESTATIO
		{  
            public const string EventId = "EventId";
            public const string GesStatus = "GesStatus";
            public const string GesFutureUse = "GesFutureUse";
            public const string GesExpAmountBorn = "GesExpAmountBorn";
            public const string GesDaysPregnant = "GesDaysPregnant";
            public const string GesPrice = "GesPrice";
            public const string GesMethod = "GesMethod";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int EventId
	    {
			get
	        {
				return base.Getint(ColumnNamesGESTATIO.EventId);
			}
			set
	        {
				base.Setint(ColumnNamesGESTATIO.EventId, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int GesStatus
	    {
			get
	        {
				return base.Getint(ColumnNamesGESTATIO.GesStatus);
			}
			set
	        {
				base.Setint(ColumnNamesGESTATIO.GesStatus, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int GesFutureUse
	    {
			get
	        {
				return base.Getint(ColumnNamesGESTATIO.GesFutureUse);
			}
			set
	        {
				base.Setint(ColumnNamesGESTATIO.GesFutureUse, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int GesExpAmountBorn
	    {
			get
	        {
				return base.Getint(ColumnNamesGESTATIO.GesExpAmountBorn);
			}
			set
	        {
				base.Setint(ColumnNamesGESTATIO.GesExpAmountBorn, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public short GesDaysPregnant
	    {
			get
	        {
				return base.Getshort(ColumnNamesGESTATIO.GesDaysPregnant);
			}
			set
	        {
				base.Setshort(ColumnNamesGESTATIO.GesDaysPregnant, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public double GesPrice
	    {
			get
	        {
				return base.Getdouble(ColumnNamesGESTATIO.GesPrice);
			}
			set
	        {
				base.Setdouble(ColumnNamesGESTATIO.GesPrice, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public short GesMethod
	    {
			get
	        {
				return base.Getshort(ColumnNamesGESTATIO.GesMethod);
			}
			set
	        {
				base.Setshort(ColumnNamesGESTATIO.GesMethod, value);
			}
		}


		#endregion
		
	}
}
