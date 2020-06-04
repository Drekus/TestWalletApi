using System.Collections.Generic;

namespace TestWalletApi.Models
{
    public class Wallet
    {
        public long Id { get; set; }
        
        public long UserId { get; set; }

        public ICollection<CurrencyTab> CurrencyTabs { get; set; } = new List<CurrencyTab>();
    }
}
