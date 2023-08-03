namespace CaddyShackMVC.Models
{
    public class GolfBag
    {
        public int Id { get; set; }
        public string Player { get; set; }
        public int Capacity { get; set; }
        public List<Club> Clubs { get; set; } = new List<Club>();
    }
}
