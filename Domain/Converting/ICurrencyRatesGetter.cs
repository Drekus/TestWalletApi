using System.Collections.Generic;
using System.Threading.Tasks;
using TestWalletApi.Models;

namespace TestWalletApi.Domain.Converting
{
    public interface ICurrencyRatesGetter
    {
        Task<Dictionary<CurrencyType, decimal>> GetRates();
    }
}