using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE.DB.DataTypes
{
    public class FTPINFO : DataObject
    {
        [Obsolete("DataObject zonder database tabel")]
        public FTPINFO()
            : base(Database.temptable)
        {
        }

        #region ColumnNames
        public class ColumnNamesFTPINFO
        {
            public const string FtpHostName = "FtpHostName";
            public const string UserName = "UserName";
            public const string Password = "Password";
            public const string PassiveMode = "PassiveMode";
            public const string DirectoryFrom = "DirectoryFrom";
            public const string DirectoryTo = "DirectoryTo";
            public const string Direction = "Direction";
            public const string UseExtention = "UseExtention";
            public const string AfterTransfer = "AfterTransfer";
        }
        #endregion

        #region Properties

        public string FtpHostName
        {
            get
            {
                return base.Getstring(ColumnNamesFTPINFO.FtpHostName);
            }
            set
            {
                base.Setstring(ColumnNamesFTPINFO.FtpHostName, value);
            }
        }

        public string UserName
        {
            get
            {
                return base.Getstring(ColumnNamesFTPINFO.UserName);
            }
            set
            {
                base.Setstring(ColumnNamesFTPINFO.UserName, value);
            }
        }

        public string Password
        {
            get
            {
                return base.Getstring(ColumnNamesFTPINFO.Password);
            }
            set
            {
                base.Setstring(ColumnNamesFTPINFO.Password, value);
            }
        }
        public sbyte PassiveMode
        {
            get
            {
                return base.Getsbyte(ColumnNamesFTPINFO.PassiveMode);
            }
            set
            {
                base.Setsbyte(ColumnNamesFTPINFO.PassiveMode, value);
            }
        }

        public string DirectoryFrom
        {
            get
            {
                return base.Getstring(ColumnNamesFTPINFO.DirectoryFrom);
            }
            set
            {
                base.Setstring(ColumnNamesFTPINFO.DirectoryFrom, value);
            }
        }

        public string DirectoryTo
        {
            get
            {
                return base.Getstring(ColumnNamesFTPINFO.DirectoryTo);
            }
            set
            {
                base.Setstring(ColumnNamesFTPINFO.DirectoryTo, value);
            }
        }

        public sbyte Direction
        {
            get
            {
                return base.Getsbyte(ColumnNamesFTPINFO.Direction);
            }
            set
            {
                base.Setsbyte(ColumnNamesFTPINFO.Direction, value);
            }
        }

        public string UseExtention
        {
            get
            {
                return base.Getstring(ColumnNamesFTPINFO.UseExtention);
            }
            set
            {
                base.Setstring(ColumnNamesFTPINFO.UseExtention, value);
            }
        }

        public sbyte AfterTransfer
        {
            get
            {
                return base.Getsbyte(ColumnNamesFTPINFO.AfterTransfer);
            }
            set
            {
                base.Setsbyte(ColumnNamesFTPINFO.AfterTransfer, value);
            }
        }

        #endregion
    }
}
