using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using TestWalletApi.Models;

namespace TestWalletApi.DTO
{
    public class ChangeMoneyDto: BaseMoneyDto
    {

        [Required]
        [JsonProperty("currency")]
        public CurrencyType Currency { get; set; }

        [Required]
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
        
        [Required]
        [JsonProperty("changeVersion")]
        public byte[] ChangeVersion { get; set; }

    }
}
