using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TestWalletApi.DTO
{
    public class CreatingWallet
    {
        [Required]
        [Range(1, long.MaxValue)]
        [JsonProperty("userId")]
        public long UserId { get; set; }
    }
}