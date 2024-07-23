
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
	public class PSINST : DataObject
	{
		public PSINST() : base(Database.animal)
		{
			UpdateParams = new String[] 
			{
            	"Internalnr"
			};
		
		}

	
		#region ColumnNames
		public class ColumnNamesPSINST
		{  
            public const string Internalnr = "Internalnr";
            public const string Listnr = "Listnr";
            public const string InternalLineNr = "InternalLineNr";
            public const string DDNumber = "DDNumber";
            public const string SubNumber = "SubNumber";
            public const string EnableLine = "EnableLine";
            public const string fd_Text = "fd_Text";
            public const string Visible = "Visible";
 

		}
		#endregion




		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public int Internalnr
	    {
			get
	        {
				return base.Getint(ColumnNamesPSINST.Internalnr);
			}
			set
	        {
				base.Setint(ColumnNamesPSINST.Internalnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int Listnr
	    {
			get
	        {
				return base.Getint(ColumnNamesPSINST.Listnr);
			}
			set
	        {
				base.Setint(ColumnNamesPSINST.Listnr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int InternalLineNr
	    {
			get
	        {
				return base.Getint(ColumnNamesPSINST.InternalLineNr);
			}
			set
	        {
				base.Setint(ColumnNamesPSINST.InternalLineNr, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int DDNumber
	    {
			get
	        {
				return base.Getint(ColumnNamesPSINST.DDNumber);
			}
			set
	        {
				base.Setint(ColumnNamesPSINST.DDNumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int SubNumber
	    {
			get
	        {
				return base.Getint(ColumnNamesPSINST.SubNumber);
			}
			set
	        {
				base.Setint(ColumnNamesPSINST.SubNumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte EnableLine
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesPSINST.EnableLine);
			}
			set
	        {
				base.Setsbyte(ColumnNamesPSINST.EnableLine, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string Fd_Text
	    {
			get
	        {
				return base.Getstring(ColumnNamesPSINST.fd_Text);
			}
			set
	        {
				base.Setstring(ColumnNamesPSINST.fd_Text, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte Visible
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesPSINST.Visible);
			}
			set
	        {
				base.Setsbyte(ColumnNamesPSINST.Visible, value);
			}
		}


		#endregion
		
	}
}
