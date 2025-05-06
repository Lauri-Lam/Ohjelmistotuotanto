public class Varaus
{
    public int Id { get; set; }
    public int AsiakasId { get; set; }
    public Asiakas Asiakas { get; set; }
    public int MokkiId { get; set; }
    public Mokki Mokki { get; set; }
    public DateTime Alku { get; set; }
    public DateTime Loppu { get; set; }
}