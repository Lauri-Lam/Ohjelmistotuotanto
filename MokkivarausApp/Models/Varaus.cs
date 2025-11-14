using System;

namespace MokkivarausApp.Models
{
    public class Varaus
    {
        public uint VarausId { get; set; }
        public uint AsiakasId { get; set; }
        public uint MokkiId { get; set; }

        public DateTime? VarattuPvm { get; set; }
        public DateTime? VahvistusPvm { get; set; }
        public DateTime? VarattuAlkuPvm { get; set; }
        public DateTime? VarattuLoppuPvm { get; set; }

        public string MokkiNimi { get; set; }
        public string AsiakasNimi { get; set; }

        public string DateRangeText
        {
            get
            {
                if (VarattuAlkuPvm.HasValue && VarattuLoppuPvm.HasValue)
                    return $"{VarattuAlkuPvm.Value:dd.MM.yyyy} - {VarattuLoppuPvm.Value:dd.MM.yyyy}";

                if (VarattuAlkuPvm.HasValue)
                    return VarattuAlkuPvm.Value.ToString("dd.MM.yyyy");

                return string.Empty;
            }
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(MokkiNimi) && !string.IsNullOrEmpty(DateRangeText))
                return $"{MokkiNimi} ({DateRangeText})";

            return base.ToString();
        }
    }
}
