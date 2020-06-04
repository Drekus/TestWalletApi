using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TestWalletApi.Models
{
    public class CurrencyTab
    {
        public CurrencyType Сurrency { get; set; }

        public decimal Amount { get; set; }

        public long WalletId { get; set; }
        
        [JsonIgnore]
        public Wallet Wallet { get; set; }

        [Timestamp]
        public byte[] ChangeVersion { get; set; }
    }
}
