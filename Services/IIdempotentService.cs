using System.Threading.Tasks;
using TestWalletApi.DTO;

namespace TestWalletApi.Services
{
    public interface IIdempotentService
    {
        Task<bool> CheckIdempotent(string idempotentKey, BaseMoneyDto parameters);
    }
}