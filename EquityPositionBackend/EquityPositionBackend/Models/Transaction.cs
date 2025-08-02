using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EquityPositionBackend.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // This ensures auto-generation
        public int TransactionID { get; set; }

        public int TradeID { get; set; }
        public int Version { get; set; }
        public string SecurityCode { get; set; }
        public int Quantity { get; set; }
        public string ActionType { get; set; }
        public string TradeType { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }


  
}
