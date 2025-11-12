namespace MokkivarausApp.Models
{
    public class Asiakas
    {
        public uint AsiakasId { get; set; }
        public string Postinro { get; set; }
        public string Etunimi { get; set; }
        public string Sukunimi { get; set; }
        public string Lahiosoite { get; set; }
        public string Email { get; set; }
        public string Puhelinnro { get; set; }

        public string FullName => $"{Etunimi} {Sukunimi}".Trim();

        public override string ToString() => FullName;
    }
}
