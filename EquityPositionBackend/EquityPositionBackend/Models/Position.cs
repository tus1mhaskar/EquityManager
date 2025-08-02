namespace EquityPositionBackend.Models
{
    public class Position
    {
        public int PositionID { get; set; }
        public string SecurityCode { get; set; }
        public int Quantity { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
