
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
	public class VERIFIED_ANIMAL : DataObject
	{
		public VERIFIED_ANIMAL() : base(Database.agrodata)
		{
			UpdateParams = new String[] 
			{
            	"AnimalLifenumber"
			};
		
		}
	
		#region ColumnNames
		public class ColumnNamesVERIFIED_ANIMAL
		{  
            public const string AnimalLifenumber = "AnimalLifenumber";
            public const string AnimalBirthDate = "AnimalBirthDate";
            public const string AnimalMotherLifenumber = "AnimalMotherLifenumber";
            public const string AnimalSex = "AnimalSex";
            public const string AnimalHaircolor = "AnimalHaircolor";
            public const string AnimalBornOnUBN = "AnimalBornOnUBN";
            public const string AnimalSpecies = "AnimalSpecies";
            public const string MutationDate = "MutationDate";
            public const string MutationDataSource = "MutationDataSource";
            public const string MutationIdentifier = "MutationIdentifier";
            public const string CurrentUBN = "CurrentUBN";
            public const string AnimalWorkNr = "AnimalWorkNr";
            public const string AnimalImportDate = "AnimalImportDate";
            public const string AnimalDateEnd = "AnimalDateEnd";
            public const string AnimalReasonEnd = "AnimalReasonEnd";
            public const string AnimalReplacementLifenumber = "AnimalReplacementLifenumber";
            public const string AnimalCountryCodeOrigin1 = "AnimalCountryCodeOrigin1";
            public const string AnimalCountryCodeOrigin2 = "AnimalCountryCodeOrigin2";
        }
		#endregion

		#region Properties
	
		[System.ComponentModel.DataObjectField(true, false, false)] 
		public string AnimalLifenumber
	    {
			get
	        {
				return base.Getstring(ColumnNamesVERIFIED_ANIMAL.AnimalLifenumber);
			}
			set
	        {
				base.Setstring(ColumnNamesVERIFIED_ANIMAL.AnimalLifenumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime AnimalBirthDate
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesVERIFIED_ANIMAL.AnimalBirthDate);
			}
			set
	        {
				base.SetDateTime(ColumnNamesVERIFIED_ANIMAL.AnimalBirthDate, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string AnimalMotherLifenumber
	    {
			get
	        {
				return base.Getstring(ColumnNamesVERIFIED_ANIMAL.AnimalMotherLifenumber);
			}
			set
	        {
				base.Setstring(ColumnNamesVERIFIED_ANIMAL.AnimalMotherLifenumber, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public sbyte AnimalSex
	    {
			get
	        {
				return base.Getsbyte(ColumnNamesVERIFIED_ANIMAL.AnimalSex);
			}
			set
	        {
				base.Setsbyte(ColumnNamesVERIFIED_ANIMAL.AnimalSex, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string AnimalHaircolor
	    {
			get
	        {
				return base.Getstring(ColumnNamesVERIFIED_ANIMAL.AnimalHaircolor);
			}
			set
	        {
				base.Setstring(ColumnNamesVERIFIED_ANIMAL.AnimalHaircolor, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string AnimalBornOnUBN
	    {
			get
	        {
				return base.Getstring(ColumnNamesVERIFIED_ANIMAL.AnimalBornOnUBN);
			}
			set
	        {
				base.Setstring(ColumnNamesVERIFIED_ANIMAL.AnimalBornOnUBN, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int AnimalSpecies
	    {
			get
	        {
				return base.Getint(ColumnNamesVERIFIED_ANIMAL.AnimalSpecies);
			}
			set
	        {
				base.Setint(ColumnNamesVERIFIED_ANIMAL.AnimalSpecies, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public DateTime MutationDate
	    {
			get
	        {
				return base.GetDateTime(ColumnNamesVERIFIED_ANIMAL.MutationDate);
			}
			set
	        {
				base.SetDateTime(ColumnNamesVERIFIED_ANIMAL.MutationDate, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int MutationDataSource
	    {
			get
	        {
				return base.Getint(ColumnNamesVERIFIED_ANIMAL.MutationDataSource);
			}
			set
	        {
				base.Setint(ColumnNamesVERIFIED_ANIMAL.MutationDataSource, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public int MutationIdentifier
	    {
			get
	        {
				return base.Getint(ColumnNamesVERIFIED_ANIMAL.MutationIdentifier);
			}
			set
	        {
				base.Setint(ColumnNamesVERIFIED_ANIMAL.MutationIdentifier, value);
			}
		}

		[System.ComponentModel.DataObjectField(false, false, false)] 
		public string CurrentUBN
	    {
			get
	        {
				return base.Getstring(ColumnNamesVERIFIED_ANIMAL.CurrentUBN);
			}
			set
	        {
				base.Setstring(ColumnNamesVERIFIED_ANIMAL.CurrentUBN, value);
			}
		}

        [System.ComponentModel.DataObjectField(false, false, false)]
        public string AnimalWorkNr
        {
            get
            {
                return base.Getstring(ColumnNamesVERIFIED_ANIMAL.AnimalWorkNr);
            }
            set
            {
                base.Setstring(ColumnNamesVERIFIED_ANIMAL.AnimalWorkNr, value);
            }
        }

        [System.ComponentModel.DataObjectField(false, false, false)]
        public DateTime AnimalImportDate
        {
            get
            {
                return base.GetDateTime(ColumnNamesVERIFIED_ANIMAL.AnimalImportDate);
            }
            set
            {
                base.SetDateTime(ColumnNamesVERIFIED_ANIMAL.AnimalImportDate, value);
            }
        }

        [System.ComponentModel.DataObjectField(false, false, false)]
        public DateTime AnimalDateEnd
        {
            get
            {
                return base.GetDateTime(ColumnNamesVERIFIED_ANIMAL.AnimalDateEnd);
            }
            set
            {
                base.SetDateTime(ColumnNamesVERIFIED_ANIMAL.AnimalDateEnd, value);
            }
        }

        public string AnimalReasonEnd
        {
            get
            {
                return base.Getstring(ColumnNamesVERIFIED_ANIMAL.AnimalReasonEnd);
            }
            set
            {
                base.Setstring(ColumnNamesVERIFIED_ANIMAL.AnimalReasonEnd, value);
            }
        }

        public string AnimalReplacementLifenumber
        {
            get
            {
                return base.Getstring(ColumnNamesVERIFIED_ANIMAL.AnimalReplacementLifenumber);
            }
            set
            {
                base.Setstring(ColumnNamesVERIFIED_ANIMAL.AnimalReplacementLifenumber, value);
            }
        }

        public string AnimalCountryCodeOrigin1
        {
            get
            {
                return base.Getstring(ColumnNamesVERIFIED_ANIMAL.AnimalCountryCodeOrigin1);
            }
            set
            {
                base.Setstring(ColumnNamesVERIFIED_ANIMAL.AnimalCountryCodeOrigin1, value);
            }
        }

        public string AnimalCountryCodeOrigin2
        {
            get
            {
                return base.Getstring(ColumnNamesVERIFIED_ANIMAL.AnimalCountryCodeOrigin2);
            }
            set
            {
                base.Setstring(ColumnNamesVERIFIED_ANIMAL.AnimalCountryCodeOrigin2, value);
            }
        }    
        
        #endregion

    }
}
