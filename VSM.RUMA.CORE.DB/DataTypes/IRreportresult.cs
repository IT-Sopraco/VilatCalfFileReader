using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE.DB.DataTypes
{
    public class IRreportresult : DataObject
    {
        public IRreportresult() : base(Database.temptable)
        {

        }


        #region Properties

        public String Levensnummer
        {
            get
            {
                return base.Getstring(ColumnNames.Levensnummer);
            }
            set
            {
                base.Setstring(ColumnNames.Levensnummer, value);
            }
        }
        public String Omschrijving
        {
            get
            {
                return base.Getstring(ColumnNames.Omschrijving);
            }
            set
            {
                base.Setstring(ColumnNames.Omschrijving, value);
            }
        }

        public String UBN
        {
            get
            {
                return base.Getstring(ColumnNames.UBN);
            }
            set
            {
                base.Setstring(ColumnNames.UBN, value);
            }
        }

        public int Type
        {
            get
            {
                return base.Getint(ColumnNames.Type);
            }
            set
            {
                base.Setint(ColumnNames.Type, value);
            }
        }

        public DateTime Date
        {
            get
            {
                return base.GetDateTime(ColumnNames.Date);
            }
            set
            {
                base.SetDateTime(ColumnNames.Date, value);
            }
        }

        #endregion

        class ColumnNames
        {
            public const string Levensnummer = "Levensnummer";
            public const string Omschrijving = "Omschrijving";
            public const string UBN = "UBN";
            public const string Type = "Type";
            public const string Date = "Date";
        }
    }
}
