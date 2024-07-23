using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class CsvFunctions
    {
        /*
         * CSV functies uit de Delphi algemeen.pas unit
         */

        public static int fAantalCSVvelden(string line, string csvchar)
        {
            if (csvchar == "")
                csvchar = ";";

            int count = 0;
            int i = 0;

            while ((i = line.IndexOf(csvchar, i)) != -1)
            {
                i += csvchar.Length;
                count++;
            }

            return count + 1;
        }

        public static string leesCSVveld(string line, int index, string csvchar)
        {
            string retString = "";

            //default
            if (csvchar == "")
                csvchar = ";";

            int nextPos = 0;
            int startPos = 0;

            int idx = 1;

            while ((nextPos = line.IndexOf(csvchar, nextPos)) != -1)
            {
                if (idx == index)
                {
                    int length = nextPos - startPos;

                    if (length > 0)
                    {
                        if (line.Length > (startPos + length))
                            retString = line.Substring(startPos, length);

                        return retString;
                    }
                }

                nextPos += csvchar.Length;
                startPos = nextPos;

                idx++;
            }

            if ((idx == index) && (startPos < line.Length))
                retString = line.Substring(startPos, (line.Length - startPos));

            return retString;
        }
        
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

        public static void ExportToCSV(DataTable dt, ref System.IO.Stream OutputStream)
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
    }
}
