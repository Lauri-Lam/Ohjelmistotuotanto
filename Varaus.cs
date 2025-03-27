namespace VillageNewbiesReservationSystem.Models
{
    public class Varaus
    {
        public int VarausId { get; set; }
        public int AsiakasId { get; set; }
        public int MokkiId { get; set; }
        public DateTime Alkupvm { get; set; }
        public DateTime Loppupvm { get; set; }
        public Asiakas Asiakas { get; set; }
        public Mokki Mokki { get; set; }
    }
}
