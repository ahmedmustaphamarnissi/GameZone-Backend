using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Data.enums;

namespace Models.DTO
{
    public class CardDTO
    {
        public int cardId { get; set; } 
        public string cardHolder { get; set; } = null!;
        public string cardNumber { get; set; } = null!;
        public DateOnly expiryDate { get; set; }
        public enCardType cardType { get; set; }
        public bool isDeafult { get; set; }
    }
}
