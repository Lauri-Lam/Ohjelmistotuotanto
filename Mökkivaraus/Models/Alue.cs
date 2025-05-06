public class Alue
{
    public int Id { get; set; }
    public string Nimi { get; set; }
    public ICollection<Mokki> Mokit { get; set; }
}