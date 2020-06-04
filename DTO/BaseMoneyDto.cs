using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TestWalletApi.DTO
{
    public class BaseMoneyDto
    {
        [Required]
        [Range(1, long.MaxValue)]
        [JsonProperty("walletId")]
        public long WalletId { get; set; }
    }
}