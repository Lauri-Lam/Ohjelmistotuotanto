namespace VillageNewbiesReservationSystem.Models
{
    public class Mokki
    {
        public int MokkiId { get; set; }
        public int AlueId { get; set; }
        public string Nimi { get; set; }
        public string Kuvaus { get; set; }
        public decimal Hinta { get; set; }
        public Alue Alue { get; set; }
    }
}
