using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace VSM.RUMA.CORE.DB.DataTypes
{

    public enum Database
    {
        animal,
        agrodata,
        agrofactuur,
        agrolab,
        agrolink,
        agrologs,
        chip_supplier,
        temptable
    }

    [Serializable()]
    public abstract class DataObject
    {

        protected DataObject()
        {
            DatabasePrefix = Database.temptable;
            Changed_By = 0;
            SourceID = 0;
        }

        internal DataObject(Database db)
        {
            DatabasePrefix = db;
            Changed_By = 0;
            SourceID = 0;
        }

        private const string TS = "ts";
        private const string CHANGEDBY = "Changed_By";
        private const string SOURCEID = "SourceID";

        public DateTime ts
        {
            get
            {
                return GetDateTime(TS);
            }
            set
            {
                SetDateTime(TS, value);
            }
        }

        public int Changed_By
        {
            get
            {
                return Getint(CHANGEDBY);
            }
            set
            {
                Setint(CHANGEDBY, value);
            }
        }

        public int SourceID
        {
            get
            {
                return Getint(SOURCEID);
            }
            set
            {
                Setint(SOURCEID, value);
            }
        }

 
        Dictionary<String, Object> Columns = new Dictionary<String, Object>();
        public String[] GetUpdateArray()
        {
            return null;
        }

        private bool lFilledByDb = false;

        internal bool FilledByDb
	    {
			get
	        {
                return lFilledByDb;
			}
			set
	        {
				lFilledByDb = value;
			}
		}

        public void SetIsFilledByDB(bool IsFilledByDB)
        {
            FilledByDb = IsFilledByDB;
        }   

        private Database DatabasePrefix;

        internal Database getDB
        {
            get
            {
                return DatabasePrefix;
            }
        }

        internal String[] UpdateParams
        {
            get;
            set;
        }

        protected sbyte Getsbyte(String ColumnName)
        {
            if (Columns.ContainsKey(ColumnName))
            {
                return Convert.ToSByte(Columns[ColumnName]);
            }
            else return 0;
        }

        protected void Setbyte(String ColumnName, byte Value)
        {
            if (!Columns.ContainsKey(ColumnName))
                Columns.Add(ColumnName, Value);
            else
                Columns[ColumnName] = Value;
        }

        protected byte Getbyte(String ColumnName)
        {
            if (Columns.ContainsKey(ColumnName))
            {
                return Convert.ToByte(Columns[ColumnName]);
            }
            else return 0;
        }

        protected void Setsbyte(String ColumnName, sbyte Value)
        {
            if (!Columns.ContainsKey(ColumnName))
                Columns.Add(ColumnName, Value);
            else
                Columns[ColumnName] = Value;
        }

        protected short Getshort(String ColumnName)
        {
            if (Columns.ContainsKey(ColumnName))
            {
                return Convert.ToInt16(Columns[ColumnName]);
            }
            else return 0;
        }

        protected void Setshort(String ColumnName, short Value)
        {
            if (!Columns.ContainsKey(ColumnName))
                Columns.Add(ColumnName, Value);
            else
                Columns[ColumnName] = Value;
        }



        protected ushort Getushort(String ColumnName)
        {
            if (Columns.ContainsKey(ColumnName))
            {
                return Convert.ToUInt16(Columns[ColumnName]);
            }
            else return 0;
        }

        protected void Setushort(String ColumnName, ushort Value)
        {
            if (!Columns.ContainsKey(ColumnName))
                Columns.Add(ColumnName, Value);
            else
                Columns[ColumnName] = Value;
        }

        protected int Getint(String ColumnName)
        {
            if (Columns.ContainsKey(ColumnName))
            {
                return Convert.ToInt32(Columns[ColumnName]);
            }
            else return 0;
        }

        protected void Setint(String ColumnName, int Value)
        {
            if (!Columns.ContainsKey(ColumnName))
                Columns.Add(ColumnName, Value);
            else
                Columns[ColumnName] = Value;
        }

        protected Int64 Getint64(String ColumnName)
        {
            if (Columns.ContainsKey(ColumnName))
            {
                return Convert.ToInt64(Columns[ColumnName]);
            }
            else return 0;
        }

        protected void Setint64(String ColumnName, Int64 Value)
        {
            if (!Columns.ContainsKey(ColumnName))
                Columns.Add(ColumnName, Value);
            else
                Columns[ColumnName] = Value;
        }

        protected uint Getuint(String ColumnName)
        {
            if (Columns.ContainsKey(ColumnName))
            {
                return Convert.ToUInt32(Columns[ColumnName]);
            }
            else return 0;
        }

        protected void Setuint(String ColumnName, uint Value)
        {
            if (!Columns.ContainsKey(ColumnName))
                Columns.Add(ColumnName, Value);
            else
                Columns[ColumnName] = Value;
        }


        protected long Getlong(String ColumnName)
        {
            if (Columns.ContainsKey(ColumnName))
            {
                return Convert.ToInt64(Columns[ColumnName]);
            }
            else return 0;
        }

        protected void Setlong(String ColumnName, long Value)
        {
            if (!Columns.ContainsKey(ColumnName))
                Columns.Add(ColumnName, Value);
            else
                Columns[ColumnName] = Value;
        }

        protected ulong Getulong(String ColumnName)
        {
            if (Columns.ContainsKey(ColumnName))
            {
                return Convert.ToUInt64(Columns[ColumnName]);
            }
            else return 0;
        }

        protected void Setulong(String ColumnName, ulong Value)
        {
            if (!Columns.ContainsKey(ColumnName))
                Columns.Add(ColumnName, Value);
            else
                Columns[ColumnName] = Value;
        }


        protected double Getdouble(String ColumnName)
        {
            if (Columns.ContainsKey(ColumnName))
            {
                return Convert.ToDouble(Columns[ColumnName]);
            }
            else return 0;
        }

        protected void Setdouble(String ColumnName, double Value)
        {
            if (!Columns.ContainsKey(ColumnName))
                Columns.Add(ColumnName, Value);
            else
                Columns[ColumnName] = Value;
        }


        protected String Getstring(String ColumnName)
        {
            if (Columns.ContainsKey(ColumnName))
            {
                return Convert.ToString(Columns[ColumnName]);
            }
            else return String.Empty;

        }

        protected void Setstring(String ColumnName, String Value)
        {
            if (!Columns.ContainsKey(ColumnName))
                Columns.Add(ColumnName, Value);
            else
                Columns[ColumnName] = Value;
        }
        
        protected DateTime GetDateTime(String ColumnName)
        {
            if (Columns.ContainsKey(ColumnName))
            {
                return Convert.ToDateTime(Columns[ColumnName]);
            }
            else return DateTime.MinValue;

        }

        protected void SetDateTime(String ColumnName, DateTime Value)
        {
            if (!Columns.ContainsKey(ColumnName))
                Columns.Add(ColumnName, Value);
            else
                Columns[ColumnName] = Value;
        }
        
        protected bool Getbool(String ColumnName)
        {
            if (Columns.ContainsKey(ColumnName))
            {
                return Convert.ToBoolean(Columns[ColumnName]);
            }
            else return false;
        }

        protected void Setbool(String ColumnName, bool Value)
        {
            if (!Columns.ContainsKey(ColumnName))
                Columns.Add(ColumnName, Value);
            else
                Columns[ColumnName] = Value;
        }

        protected float Getfloat(String ColumnName)
        {
            if (Columns.ContainsKey(ColumnName))
            {
                return Convert.ToSingle(Columns[ColumnName]);
            }
            else return 0;

        }

        protected void Setfloat(String ColumnName, float Value)
        {
            if (!Columns.ContainsKey(ColumnName))
                Columns.Add(ColumnName, Value);
            else
                Columns[ColumnName] = Value;
        }


        protected TimeSpan GetTimeSpan(String ColumnName)
        {
            if (Columns.ContainsKey(ColumnName))
            {
                return  TimeSpan.Parse(Columns[ColumnName].ToString());
            }
            else return new TimeSpan();

        }

        protected void SetTimeSpan(String ColumnName, TimeSpan Value)
        {
            if (!Columns.ContainsKey(ColumnName))
                Columns.Add(ColumnName, Value);
            else
                Columns[ColumnName] = Value;
        }


        protected decimal Getdecimal(String ColumnName)
        {
            if (Columns.ContainsKey(ColumnName))
            {
                return decimal.Parse(Columns[ColumnName].ToString());
            }
            else return new decimal();

        }

        protected void Setdecimal(String ColumnName, decimal Value)
        {
            if (!Columns.ContainsKey(ColumnName))
                Columns.Add(ColumnName, Value);
            else
                Columns[ColumnName] = Value;
        }


        /* nullables */


        protected sbyte? Getnullablesbyte(String ColumnName)
        {
            if (Columns.ContainsKey(ColumnName))
            {
                if (Columns[ColumnName] == null)
                    return null;
                return Convert.ToSByte(Columns[ColumnName]);
            }
            else return null;
        }

        protected void Setnullablesbyte(String ColumnName, sbyte? Value)
        {
            if (!Columns.ContainsKey(ColumnName))
                Columns.Add(ColumnName, Value);
            else
                Columns[ColumnName] = Value;
        }

        protected int? Getnullableint(String ColumnName)
        {
            if (Columns.ContainsKey(ColumnName))
            {
                if (Columns[ColumnName] == null)
                    return null;
                return Convert.ToInt32(Columns[ColumnName]);
            }
            else return null;
        }

        protected void Setnullableint(String ColumnName, int? Value)
        {
            if (!Columns.ContainsKey(ColumnName))
                Columns.Add(ColumnName, Value);
            else
                Columns[ColumnName] = Value;
        }

        protected float? Getnullablefloat(String ColumnName)
        {
            if (Columns.ContainsKey(ColumnName))
            {
                if (Columns[ColumnName] == null)
                    return null;

                return Convert.ToSingle(Columns[ColumnName]);
            }
            else return null;

        }

        protected void Setnullablefloat(String ColumnName, float? Value)
        {
            if (!Columns.ContainsKey(ColumnName))
                Columns.Add(ColumnName, Value);
            else
                Columns[ColumnName] = Value;
        }

        protected decimal? Getnullabledecimal(String ColumnName)
        {
            if (Columns.ContainsKey(ColumnName))
            {
                if (Columns[ColumnName] == null)
                    return null;

                return Convert.ToDecimal(Columns[ColumnName]);
            }
            else return null;

        }

        protected void Setnullabledecimal(String ColumnName, decimal? Value)
        {
            if (!Columns.ContainsKey(ColumnName))
                Columns.Add(ColumnName, Value);
            else
                Columns[ColumnName] = Value;
        }

        protected short? Getnullableshort(String ColumnName)
        {
            if (Columns.ContainsKey(ColumnName))
            {
                if (Columns[ColumnName] == null)
                    return null;

                return Convert.ToInt16(Columns[ColumnName]);
            }
            else return null;

        }

        protected void Setnullableshort(String ColumnName, short? Value)
        {
            if (!Columns.ContainsKey(ColumnName))
                Columns.Add(ColumnName, Value);
            else
                Columns[ColumnName] = Value;
        }
    }
}
