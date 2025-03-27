namespace VillageNewbiesReservationSystem.Models
{
    public class Lasku
    {
        public int LaskuId { get; set; }
        public int VarausId { get; set; }
        public decimal Summa { get; set; }
        public DateTime Luontipvm { get; set; }
        public Varaus Varaus { get; set; }
    }
}
