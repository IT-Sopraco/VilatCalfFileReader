using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using System.Text;

namespace VSM.RUMA.CORE.DB
{
    public class FileExport
    {

        public static void ExportToCSV(DataTable dt, string strFile)
        {
            var sw = new StreamWriter(strFile, false);

            int iColCount = dt.Columns.Count;
            for (int i = 0; i < iColCount; i++)
            {
                sw.Write(dt.Columns[i]);
                if (i < iColCount - 1) sw.Write(";");
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < iColCount; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        if (dr[i].ToString().StartsWith("0"))
                        {
                            sw.Write(@"=""" + dr[i] + @"""");
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }

                    if (i < iColCount - 1) sw.Write(";");
                }
                sw.Write(sw.NewLine);
            }

            sw.Close();
        }

        public static void ExportToCSV(DataTable dt, Stream OutputStream)
        {
            var sw = new StreamWriter(OutputStream);

            int iColCount = dt.Columns.Count;
            for (int i = 0; i < iColCount; i++)
            {
                sw.Write(dt.Columns[i]);
                if (i < iColCount - 1) sw.Write(";");
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < iColCount; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        //if (dr[i].ToString().StartsWith("0"))
                        //{
                        //    sw.Write(@"=""" + dr[i] + @"""");
                        //}
                        //else
                        //{
                        sw.Write(dr[i].ToString());
                        //}
                    }

                    if (i < iColCount - 1) sw.Write(";");
                }
                sw.Write(sw.NewLine);
            }

            sw.Flush();
        }
    }
}
