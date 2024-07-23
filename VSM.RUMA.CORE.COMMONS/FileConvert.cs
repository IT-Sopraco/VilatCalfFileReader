using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.OleDb;
using System.Text;

namespace VSM.RUMA.CORE.COMMONS
{
    public class FileConvert
    {
        public static bool xls2csv(String FileNameIn, String FileNameOut)
        {
            StreamWriter wr2 = null;
            OleDbConnection conn = null;
            try
            {
                wr2 = new StreamWriter(FileNameOut, false);

                DataTable dtWerkbladen;
                string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;" + @"Data Source=" + FileNameIn + ";" + @"Extended Properties=""Excel 8.0;HDR=Yes;IMEX=1""";
                conn = new OleDbConnection(connectionString);

                conn.Open();
                dtWerkbladen = conn.GetSchema("Tables");
                StringBuilder sbCSVWerkblad;
                foreach (DataRow drWerkblad in dtWerkbladen.Rows)
                {
                    sbCSVWerkblad = new StringBuilder();
                    // For Sheets: 0=Table_Catalog,1=Table_Schema,2=Table_Name,3=Table_Type
                    // For Columns: 0=Table_Name, 1=Column_Name, 2=Ordinal_Position
                    string SheetName = (string)drWerkblad[2];
                    OleDbCommand command = new OleDbCommand(@"SELECT * FROM [" + SheetName + "]", conn);
                    OleDbDataAdapter oleAdapter = new OleDbDataAdapter();
                    oleAdapter.SelectCommand = command;
                    DataTable dtTabel = new DataTable();
                    oleAdapter.FillSchema(dtTabel, SchemaType.Source);
                    oleAdapter.Fill(dtTabel);
                    StringBuilder sbCSVLine = new StringBuilder();
                    foreach (DataColumn Colum in dtTabel.Columns)
                    {
                        sbCSVLine.AppendFormat("\"{0}\";", Colum.ColumnName);
                    }
                    sbCSVWerkblad.AppendLine(sbCSVLine.ToString());

                    foreach (DataRow drRegel in dtTabel.Rows)
                    {
                        sbCSVLine = new StringBuilder();
                        foreach (DataColumn Colum in dtTabel.Columns)
                        {
                            sbCSVLine.AppendFormat("\"{0}\";", drRegel[Colum]);
                        }
                        sbCSVWerkblad.AppendLine(sbCSVLine.ToString());
                    }
                    wr2.Write(sbCSVWerkblad.ToString());
                    wr2.Flush();
                }
                return true;
            }
            catch (Exception ex)
            {
                unLogger.WriteError("Error Converting xls to csv \r\n" + ex.Message, ex);
                return false;
            }
            finally {
                try
                {
                    if (wr2 !=null) wr2.Close();
                    if (conn != null) conn.Close();
                }
                catch (Exception ex2)
                {
                    unLogger.WriteError("Error Closing file \r\n" + ex2.Message, ex2);
                }    
            }
        }



        public static bool xlsx2csv(String FileNameIn, String FileNameOut)
        {
            try
            {
                StreamWriter wr2 = new StreamWriter(FileNameOut, false);

                DataTable dtWerkbladen;
                string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" + @"Data Source=" + FileNameIn + ";" + @"Extended Properties=""Excel 12.0;HDR=YES""";
                OleDbConnection conn = new OleDbConnection(connectionString);

                conn.Open();
                dtWerkbladen = conn.GetSchema("Tables");
                StringBuilder sbCSVWerkblad;
                foreach (DataRow drWerkblad in dtWerkbladen.Rows)
                {
                    sbCSVWerkblad = new StringBuilder();
                    // For Sheets: 0=Table_Catalog,1=Table_Schema,2=Table_Name,3=Table_Type
                    // For Columns: 0=Table_Name, 1=Column_Name, 2=Ordinal_Position
                    string SheetName = (string)drWerkblad[2];
                    OleDbCommand command = new OleDbCommand(@"SELECT * FROM [" + SheetName + "]", conn);
                    OleDbDataAdapter oleAdapter = new OleDbDataAdapter();
                    oleAdapter.SelectCommand = command;
                    DataTable dtTabel = new DataTable();
                    oleAdapter.FillSchema(dtTabel, SchemaType.Source);
                    oleAdapter.Fill(dtTabel);
                    StringBuilder sbCSVLine = new StringBuilder();
                    foreach (DataColumn Colum in dtTabel.Columns)
                    {
                        sbCSVLine.AppendFormat("\"{0}\";", Colum.ColumnName);
                    }
                    sbCSVWerkblad.AppendLine(sbCSVLine.ToString());

                    foreach (DataRow drRegel in dtTabel.Rows)
                    {
                        sbCSVLine = new StringBuilder();
                        foreach (DataColumn Colum in dtTabel.Columns)
                        {
                            sbCSVLine.AppendFormat("\"{0}\";", drRegel[Colum]);
                        }
                        sbCSVWerkblad.AppendLine(sbCSVLine.ToString());
                    }
                    wr2.Write(sbCSVWerkblad.ToString());
                }
                wr2.Close();
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                unLogger.WriteError(ex.Message, ex);
                return false;
            }
        }

    }
}
