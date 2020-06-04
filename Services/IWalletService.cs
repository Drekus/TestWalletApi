using System.Threading.Tasks;
using TestWalletApi.DTO;
using TestWalletApi.Models;

namespace TestWalletApi.Services
{
    public interface IWalletService
    {
        Task<Wallet> GetWallet(long walletId);

        Task<Wallet> CreateWallet(long userId);

        Task<CurrencyTab> AddMoney(ChangeMoneyDto dto);
        Task<CurrencyTab[]> TransferMoney(ConvertMoneyDto dto);
    }
}
