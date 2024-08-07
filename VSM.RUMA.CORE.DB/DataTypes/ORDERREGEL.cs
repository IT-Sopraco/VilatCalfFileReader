
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
    public class ORDERREGEL : DataObject
    {
        [Obsolete("DataObject zonder database tabel")]
        public ORDERREGEL()
            : base(Database.temptable)
        {
        }


        #region ColumnNames
        public class ColumnNamesORDERREGEL
        {
            public const string OrderregelID = "OrderregelID";
            public const string OrderId = "OrderId";
            public const string OrdStatus = "OrdStatus";
            public const string OrdLeveringsDatum = "OrdLeveringsDatum";
            public const string OrdVolumeMeasurement = "OrdVolumeMeasurement";
            public const string OrdVolume = "OrdVolume";
            public const string OrdQuantityMeasurement = "OrdQuantityMeasurement";
            public const string OrdQuantity = "OrdQuantity";
            public const string Artikelid = "Artikelid";
            public const string OrdPayPriceTyp = "OrdPayPriceTyp";
            public const string OrdPayPrice = "OrdPayPrice";
            public const string OrdTotalPriceEx = "OrdTotalPriceEx";
            public const string OrdVatPrice = "OrdVatPrice";
            public const string OrdShowIncludingVat = "OrdShowIncludingVat";
            public const string OrdBTWId = "OrdBTWId";
            public const string OrdDiscount = "OrdDiscount";
            public const string OrdMemo = "OrdMemo";
            public const string OrdWerkID = "OrdWerkID";
            public const string OrdLeverDatum = "OrdLeverDatum";
            public const string OrdGefactureerd = "OrdGefactureerd";
            public const string FarmId_Ontvanger = "FarmId_Ontvanger";
            public const string Ord_Batchnr = "Ord_Batchnr";

        }
        #endregion

        #region Properties

        public int OrderregelID
        {
            get
            {
                return base.Getint(ColumnNamesORDERREGEL.OrderregelID);
            }
            set
            {
                base.Setint(ColumnNamesORDERREGEL.OrderregelID, value);
            }
        }

        public int OrderId
        {
            get
            {
                return base.Getint(ColumnNamesORDERREGEL.OrderId);
            }
            set
            {
                base.Setint(ColumnNamesORDERREGEL.OrderId, value);
            }
        }

        public sbyte OrdStatus
        {
            get
            {
                return base.Getsbyte(ColumnNamesORDERREGEL.OrdStatus);
            }
            set
            {
                base.Setsbyte(ColumnNamesORDERREGEL.OrdStatus, value);
            }
        }

        public DateTime OrdLeveringsDatum
        {
            get
            {
                return base.GetDateTime(ColumnNamesORDERREGEL.OrdLeveringsDatum);
            }
            set
            {
                base.SetDateTime(ColumnNamesORDERREGEL.OrdLeveringsDatum, value);
            }
        }

        public sbyte OrdVolumeMeasurement
        {
            get
            {
                return base.Getsbyte(ColumnNamesORDERREGEL.OrdVolumeMeasurement);
            }
            set
            {
                base.Setsbyte(ColumnNamesORDERREGEL.OrdVolumeMeasurement, value);
            }
        }

        public double OrdVolume
        {
            get
            {
                return base.Getdouble(ColumnNamesORDERREGEL.OrdVolume);
            }
            set
            {
                base.Setdouble(ColumnNamesORDERREGEL.OrdVolume, value);
            }
        }

        public sbyte OrdQuantityMeasurement
        {
            get
            {
                return base.Getsbyte(ColumnNamesORDERREGEL.OrdQuantityMeasurement);
            }
            set
            {
                base.Setsbyte(ColumnNamesORDERREGEL.OrdQuantityMeasurement, value);
            }
        }

        public double OrdQuantity
        {
            get
            {
                return base.Getdouble(ColumnNamesORDERREGEL.OrdQuantity);
            }
            set
            {
                base.Setdouble(ColumnNamesORDERREGEL.OrdQuantity, value);
            }
        }

        public int Artikelid
        {
            get
            {
                return base.Getint(ColumnNamesORDERREGEL.Artikelid);
            }
            set
            {
                base.Setint(ColumnNamesORDERREGEL.Artikelid, value);
            }
        }

        public sbyte OrdPayPriceTyp
        {
            get
            {
                return base.Getsbyte(ColumnNamesORDERREGEL.OrdPayPriceTyp);
            }
            set
            {
                base.Setsbyte(ColumnNamesORDERREGEL.OrdPayPriceTyp, value);
            }
        }

        public double OrdPayPrice
        {
            get
            {
                return base.Getdouble(ColumnNamesORDERREGEL.OrdPayPrice);
            }
            set
            {
                base.Setdouble(ColumnNamesORDERREGEL.OrdPayPrice, value);
            }
        }

        public double OrdTotalPriceEx
        {
            get
            {
                return base.Getdouble(ColumnNamesORDERREGEL.OrdTotalPriceEx);
            }
            set
            {
                base.Setdouble(ColumnNamesORDERREGEL.OrdTotalPriceEx, value);
            }
        }

        public double OrdVatPrice
        {
            get
            {
                return base.Getdouble(ColumnNamesORDERREGEL.OrdVatPrice);
            }
            set
            {
                base.Setdouble(ColumnNamesORDERREGEL.OrdVatPrice, value);
            }
        }

        public sbyte OrdShowIncludingVat
        {
            get
            {
                return base.Getsbyte(ColumnNamesORDERREGEL.OrdShowIncludingVat);
            }
            set
            {
                base.Setsbyte(ColumnNamesORDERREGEL.OrdShowIncludingVat, value);
            }
        }

        public int OrdBTWId
        {
            get
            {
                return base.Getint(ColumnNamesORDERREGEL.OrdBTWId);
            }
            set
            {
                base.Setint(ColumnNamesORDERREGEL.OrdBTWId, value);
            }
        }

        public double OrdDiscount
        {
            get
            {
                return base.Getdouble(ColumnNamesORDERREGEL.OrdDiscount);
            }
            set
            {
                base.Setdouble(ColumnNamesORDERREGEL.OrdDiscount, value);
            }
        }

        public string OrdMemo
        {
            get
            {
                return base.Getstring(ColumnNamesORDERREGEL.OrdMemo);
            }
            set
            {
                base.Setstring(ColumnNamesORDERREGEL.OrdMemo, value);
            }
        }

        public int OrdWerkID
        {
            get
            {
                return base.Getint(ColumnNamesORDERREGEL.OrdWerkID);
            }
            set
            {
                base.Setint(ColumnNamesORDERREGEL.OrdWerkID, value);
            }
        }

        public DateTime OrdLeverDatum
        {
            get
            {
                return base.GetDateTime(ColumnNamesORDERREGEL.OrdLeverDatum);
            }
            set
            {
                base.SetDateTime(ColumnNamesORDERREGEL.OrdLeverDatum, value);
            }
        }

        public sbyte OrdGefactureerd
        {
            get
            {
                return base.Getsbyte(ColumnNamesORDERREGEL.OrdGefactureerd);
            }
            set
            {
                base.Setsbyte(ColumnNamesORDERREGEL.OrdGefactureerd, value);
            }
        }

        public int FarmId_Ontvanger
        {
            get
            {
                return base.Getint(ColumnNamesORDERREGEL.FarmId_Ontvanger);
            }
            set
            {
                base.Setint(ColumnNamesORDERREGEL.FarmId_Ontvanger, value);
            }
        }

        public string Ord_Batchnr
        {
            get
            {
                return base.Getstring(ColumnNamesORDERREGEL.Ord_Batchnr);
            }
            set
            {
                base.Setstring(ColumnNamesORDERREGEL.Ord_Batchnr, value);
            }
        }
      
        #endregion

    }
}


