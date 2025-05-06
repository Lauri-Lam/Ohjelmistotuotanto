public class Mokki
{
    public int Id { get; set; }
    public string Nimi { get; set; }
    public int AlueId { get; set; }
    public Alue Alue { get; set; }
}