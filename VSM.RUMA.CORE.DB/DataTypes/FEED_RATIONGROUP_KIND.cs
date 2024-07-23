
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
	public class FEED_RATIONGROUP_KIND : DataObject
	{
		public FEED_RATIONGROUP_KIND() : base(Database.animal)
		{
			UpdateParams = new String[] 
			{
            	"UBNid",
            	"AnimalKind_ID",
            	"FRK_Kind_of_Animal"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesFEED_RATIONGROUP_KIND
		{  
            public const string UBNid = "UBNid";
            public const string AnimalKind_ID = "AnimalKind_ID";
            public const string FRK_GroupID = "FRK_GroupID";
            public const string FRK_Kind_of_Animal = "FRK_Kind_of_Animal";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int UBNid
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_RATIONGROUP_KIND.UBNid);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_RATIONGROUP_KIND.UBNid, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int AnimalKind_ID
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_RATIONGROUP_KIND.AnimalKind_ID);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_RATIONGROUP_KIND.AnimalKind_ID, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int FRK_GroupID
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_RATIONGROUP_KIND.FRK_GroupID);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_RATIONGROUP_KIND.FRK_GroupID, value);
			}
		}

		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int FRK_Kind_of_Animal
	    {
			get
	        {
				return base.Getint(ColumnNamesFEED_RATIONGROUP_KIND.FRK_Kind_of_Animal);
			}
			set
	        {
				base.Setint(ColumnNamesFEED_RATIONGROUP_KIND.FRK_Kind_of_Animal, value);
			}
		}


		#endregion
		
	}
}