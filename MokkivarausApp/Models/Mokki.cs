namespace MokkivarausApp.Models
{
    public class Mokki
    {
        public uint MokkiId { get; set; }
        public uint AlueId { get; set; }
        public string Postinro { get; set; }
        public string Mokkinimi { get; set; }
        public string Katuosoite { get; set; }
        public double Hinta { get; set; }
        public string Kuvaus { get; set; }
        public int? Henkilomaara { get; set; }
        public string Varustelu { get; set; }

        public override string ToString() => Mokkinimi;
    }
}
