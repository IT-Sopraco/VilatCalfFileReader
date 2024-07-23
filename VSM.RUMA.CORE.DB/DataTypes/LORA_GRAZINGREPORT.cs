using System;

namespace VSM.RUMA.CORE.DB.DataTypes
{
    public class LORA_GRAZINGREPORT
    {
        public int id;

        public string UBNNumber { get; set; }

        public string TankNumber { get; set; }

        public DateTime GrazingDate { get; set; }

        public int TotalNumberDairyCows { get; set; }

        public int TotalNumberQualifiedCows { get; set; }

        public double PercentageQualifiedForGrazing { get; set; }

        public bool GrazingDay { get; set; }

        public int FarmGrazingTime { get; set; }

        public double GrazingDeviation { get; set; }

        public long CumulatedGrazingTimeYear { get; set; }

        public int CumulatedGrazingDays { get; set; }

        public DateTime? ReportDateTime { get; set; }
    }
}
