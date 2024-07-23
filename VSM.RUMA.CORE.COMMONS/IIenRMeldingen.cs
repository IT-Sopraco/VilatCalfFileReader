using System;
using VSM.RUMA.CORE.DB;
using VSM.RUMA.CORE.DB.DataTypes;
namespace VSM.RUMA.CORE
{
    public interface IIenRMeldingen
    {
        System.Collections.Generic.List<MUTATION> ListIenRMeldingen(String pUBNid, DBConnectionToken mToken);
        System.Collections.Generic.List<MUTALOG> ListVorigeIenR(String pUBNid, DBConnectionToken mToken);
        String getMutSoort(int CodeMutation);
        String getMutGeslacht(int Sex);
        String getMutHaarKleur(int Haircolor, string pAniHairColor_Memo);
        String getMutResultaat(int Returnresult);
        String getDHZSoort(int InsInfo);
        SOAPLOG MeldIR(MUTATION pRecord, int pUBNid, int pProgId, int pProgramid, DBConnectionToken mToken, FTPUSER pLNV2IRCredidentials, LABELSConst.ChangedBy changed = LABELSConst.ChangedBy.UNKNOWN, int sourceId = 0);
        SOAPLOG LNV2MeldingIntrekken(MUTALOG pRecord, int pUBNid, int pProgId, int pProgramid, DBConnectionToken mToken);
        void MeldIDRnaarGD(System.Collections.Generic.List<MUTATION> pRecords, int pUBNid, int pProgId, int pProgramid, DBConnectionToken mToken);
        void MeldStamboekIDRnaarGD(System.Collections.Generic.List<MUTATION> pRecords, string StamBoeknr, int pProgId, DBConnectionToken mToken,out int iOk, out int iError);
        String Plugin(); 
    }
}
