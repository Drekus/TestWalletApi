using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using TestWalletApi.Models;

namespace TestWalletApi.DTO
{
    public class ConvertMoneyDto: BaseMoneyDto
    {
        [Required]
        [JsonProperty("fromCurrency")]
        public CurrencyType FromCurrency { get; set; }

        [Required]
        [JsonProperty("toCurrency")]
        public CurrencyType ToCurrency { get; set; }

        [Required]
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [Required]
        [JsonProperty("changeVersionFrom")]
        public byte[] ChangeVersionFrom { get; set; }


        [Required]
        [JsonProperty("changeVersionTo")]
        public byte[] ChangeVersionTo { get; set; }
    }
}
