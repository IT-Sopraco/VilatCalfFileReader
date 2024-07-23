using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.DB.DataTypes;
using System.Data;

namespace VSM.RUMA.CORE.DB.MYSQL.DBSQ
{
    public class DBSQResponders
    {
        private DBSelectQueries Parent;

        public DBSQResponders(DBSelectQueries parent)
        {
            this.Parent = parent;
        }

        public List<TRANSMIT> GetTransmitsByUbnId(int ubnId)
        {
            string sql = string.Format("SELECT t.* FROM TRANSMIT t WHERE t.UbnId = {0}", ubnId);
            DataTable tbl = Parent.QueryData(sql);
            return Parent.getList<TRANSMIT>(tbl);
        }


        public List<TRANSMIT> GetTransmitsByFarmId(int pFarmId,int pUbnID)
        {
            StringBuilder lQuery = new StringBuilder();

            lQuery.Append(" SELECT TRANSMIT.* FROM TRANSMIT ");
            lQuery.AppendFormat(" WHERE (TRANSMIT.farmid ={0} OR TRANSMIT.UbnID ={1}) AND (TRANSMIT.Koppelnr>0 OR TRANSMIT.ProcesComputerId>0) ", pFarmId, pUbnID);
            DataTable tbl = Parent.QueryData(lQuery.ToString());
            List<TRANSMIT> lResultList = new List<TRANSMIT>();
            foreach (DataRow drtrns in tbl.Rows)
            {
                TRANSMIT lTransprt = new TRANSMIT();
                if (Parent.FillObject(lTransprt, drtrns))
                {
                    lResultList.Add(lTransprt);
                }
            }
            return lResultList;
        }


        public TRANSMIT GetTransmitter(int ubnid, int procescomputerid, int aniid, string transmitterNumber)
        {
            StringBuilder lQuery = new StringBuilder();
            lQuery.Append(" SELECT TRANSMIT.* FROM TRANSMIT ");
            lQuery.AppendFormat(" WHERE TRANSMIT.UbnID ={0} AND TRANSMIT.ProcesComputerId = {1} AND (TRANSMIT.aniid ={2} OR TRANSMIT.transmitterNumber ={3})", ubnid, procescomputerid, aniid, transmitterNumber);
            DataTable tbl = Parent.QueryData(lQuery.ToString());
            TRANSMIT lTransprt = new TRANSMIT();
            foreach (DataRow drtrns in tbl.Rows)
            {
                if (Parent.FillObject(lTransprt, drtrns))
                {
                    return lTransprt;
                }
            }
            return lTransprt;

        }


        public DataTable getProcessCompIds(int pFarmId, int pUBNId, int pProgId)
        {
            StringBuilder lQuery = new StringBuilder();
            int pProgramId = 0;
            DataTable tblprogramid = Parent.QueryData(String.Format(" SELECT ProgramId FROM agrofactuur.BEDRIJF WHERE FarmId={0} ", pFarmId));
            if (tblprogramid.Rows.Count > 0)
            {
                if (tblprogramid.Rows[0][0]!=DBNull.Value)
                {
                    int.TryParse(tblprogramid.Rows[0][0].ToString(), out pProgramId);
                }
            }
            int lMSOptimabox = (int)CORE.utils.procesComputerReferenceNumber.MSOptimabox;
            lQuery.Append(" SELECT DISTINCT(Koppelnr) AS Nummer,l.LabLabel AS Naam,'' AS TransmitterNumber, WorkNumber  FROM TRANSMIT ");
            lQuery.Append(" LEFT OUTER JOIN agrofactuur.LABELS l ON(Koppelnr=LabID AND LabCountry=528 AND LabKind=40) ");
            lQuery.AppendFormat(" WHERE farmid = {0} AND (Koppelnr>0 OR ProcesComputerId>0) ", pFarmId);
            if (pProgramId == 16000)
            {
                lQuery.Append(" AND SUBSTRING(ProcesComputerId,1,4)='" + lMSOptimabox.ToString() + "' ");
            }
            lQuery.Append(" UNION ");
            lQuery.Append(" SELECT  agrofactuur.PSC_PROCESCOMPUTER.pspcmachineId AS Nummer,  ");
            lQuery.Append(" agrofactuur.PSC_PROCESCOMPUTER.pspcname AS Naam ,'' AS TransmitterNumber , '' AS WorkNumber ");
            lQuery.Append(" FROM agrofactuur.PSC_PROCESCOMPUTER ");
            lQuery.Append(" JOIN agrofactuur.PSC_SETTINGS ON agrofactuur.PSC_SETTINGS.psId=agrofactuur.PSC_PROCESCOMPUTER.psId ");
            lQuery.AppendFormat(" WHERE agrofactuur.PSC_SETTINGS.UbnId = {0} AND agrofactuur.PSC_SETTINGS.psProgId={1} ", pUBNId, pProgId);
            if (pProgramId == 16000)
            {
                lQuery.Append(" AND SUBSTRING(agrofactuur.PSC_PROCESCOMPUTER.pspcmachineId,1,4)='" + lMSOptimabox.ToString() + "' ");
            }

            DataTable tbl = Parent.QueryData(lQuery.ToString(), MissingSchemaAction.Add);
            foreach (DataRow rw in tbl.Rows)
            {
                if (rw["Nummer"] != DBNull.Value)
                {
                    if (rw["Naam"] == DBNull.Value || rw["Naam"].ToString() == "")
                    {
                        rw["Naam"] = CORE.utils.getTRANSMITKoppelingText(int.Parse(rw["Nummer"].ToString()));
                    }

                }
            }
            return tbl;
        }

        public List<TRANSMSTOCK> GetTransmitterVoorraad(int pUbniId)
        {
            StringBuilder lQuery = new StringBuilder();

            lQuery.Append(" SELECT TRANSMSTOCK.* FROM TRANSMSTOCK ");
            lQuery.AppendFormat(" WHERE TRANSMSTOCK.UbnID ={0} AND (TRANSMSTOCK.Koppelingnr>0 OR TRANSMSTOCK.ProcesComputerId>0) ORDER BY TRANSMSTOCK.RespSort ", pUbniId);

            DataTable tbl = Parent.QueryData(lQuery.ToString());
            List<TRANSMSTOCK> lResultList = new List<TRANSMSTOCK>();
            foreach (DataRow drtrns in tbl.Rows)
            {
                TRANSMSTOCK lTransms = new TRANSMSTOCK();
                if (Parent.FillObject(lTransms, drtrns))
                {
                    lResultList.Add(lTransms);
                }
            }
            return lResultList;
        }

        /// <summary>
        /// Haalt ook stock records op zondder koppelingsnr of procescomptuerId
        /// </summary>
        /// <param name="pUbniId"></param>
        /// <returns></returns>
        public List<TRANSMSTOCK> GetTransmitterVoorraad2(int pUbniId)
        {
            string sql = string.Format("SELECT t.* FROM TRANSMSTOCK t WHERE t.UbnId = {0}", pUbniId);
            DataTable tbl = Parent.QueryData(sql);
            return Parent.getList<TRANSMSTOCK>(tbl);
        }

        public List<TRANSMSTOCK> GetTransmitterVoorraadByProcesComputerId(int pUbniId, int pProcesComputerId)
        {
            StringBuilder lQuery = new StringBuilder();

            lQuery.Append(" SELECT TRANSMSTOCK.* FROM TRANSMSTOCK ");
            lQuery.AppendFormat(" WHERE TRANSMSTOCK.UbnID ={0} AND (TRANSMSTOCK.Koppelingnr>0 OR TRANSMSTOCK.ProcesComputerId={1}) ORDER BY TRANSMSTOCK.RespSort ", pUbniId, pProcesComputerId);

            DataTable tbl = Parent.QueryData(lQuery.ToString());
            List<TRANSMSTOCK> lResultList = new List<TRANSMSTOCK>();
            foreach (DataRow drtrns in tbl.Rows)
            {
                TRANSMSTOCK lTransms = new TRANSMSTOCK();
                if (Parent.FillObject(lTransms, drtrns))
                {
                    lResultList.Add(lTransms);
                }
            }
            return lResultList;
        }
    }
}
