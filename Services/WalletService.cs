using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWalletApi.Data;
using TestWalletApi.Domain.Converting;
using TestWalletApi.DTO;
using TestWalletApi.Models;

namespace TestWalletApi.Services
{
    public class WalletService : IWalletService
    {
        private  ApplicationDbContext _db;
        private readonly ILogger<WalletService> _logger;
        private readonly ICurrencyRatesGetter _getter;

        public WalletService(ApplicationDbContext db, ILogger<WalletService> logger,  ICurrencyRatesGetter currencyRatesGetter)
        {
            _db = db;
            _logger = logger;
            _getter = currencyRatesGetter;
        }
        

        public async Task<Wallet> GetWallet(long walletId)
        {
            return await _db.Wallets.Include(w => w.CurrencyTabs).FirstOrDefaultAsync(w => w.Id == walletId);;
        }

        public async Task<Wallet> CreateWallet(long userId)
        {
            var newWallet = new Wallet {UserId = userId};
            
            try
            {
                await _db.Wallets.AddAsync(newWallet);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to create wallet for user: {userId}");
                throw new ApplicationException("Failed to create wallet");
            }
            return newWallet;
        }

        public async Task<CurrencyTab> AddMoney(ChangeMoneyDto dto)
        {
            var wallet = await GetWallet(dto.WalletId);
            if (wallet == null)
                throw new Exception(nameof(dto.WalletId)); // ObjectNotFoundException //todo

            ///тут бы добавил проверку на то что, кошелек принадлежит текущему пользователю

            var tab = wallet.CurrencyTabs.FirstOrDefault(t => t.Сurrency == dto.Currency);
            if (tab == null)
            {
                if (dto.Amount < 0)
                    throw new ArgumentException($"Can't create new currency tab with negative amount");

                tab = new CurrencyTab
                {
                    WalletId = dto.WalletId,
                    Сurrency = dto.Currency,
                    Amount = dto.Amount
                };

                await _db.CurrencyTabs.AddAsync(tab);
            }
            else
            {
                if (!tab.ChangeVersion.SequenceEqual(dto.ChangeVersion))
                    throw new ArgumentException(nameof(dto.ChangeVersion));

                if (tab.Amount + dto.Amount < 0)
                    throw new ArgumentException(nameof(dto.Amount));

                tab.Amount += dto.Amount;
            }

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to adding money with dto: {dto}");
                throw new ApplicationException("Failed to adding money");
            }
            
            return tab;
        }
        
        public async Task<CurrencyTab[]> TransferMoney(ConvertMoneyDto dto)
        {
            var wallet = await GetWallet(dto.WalletId);
            if (wallet == null)
                throw new Exception(nameof(dto.WalletId)); // ObjectNotFoundException //todo

            ///тут бы добавил проверку на то что, кошелек принадлежит текущему пользователю

            var tabFrom = wallet.CurrencyTabs.FirstOrDefault(t => t.Сurrency == dto.FromCurrency);
            if (tabFrom == null)
            {
                throw new Exception(nameof(dto.FromCurrency)); // ObjectNotFoundException //todo
            }

            var rates = await  _getter.GetRates();
            var converter = new CurrencyConverter(rates, CurrencyRatesFromEuroBankGetter.MAIN_CURRENCY);
            var convertedAmount = converter.Convert(dto);

            var dtoFrom = new ChangeMoneyDto
            {
                WalletId = dto.WalletId,
                Currency = dto.FromCurrency,
                ChangeVersion = dto.ChangeVersionFrom,
                Amount = -dto.Amount
            };
            var dtoTo = new ChangeMoneyDto
            {
                WalletId = dto.WalletId,
                Currency = dto.ToCurrency,
                ChangeVersion = dto.ChangeVersionTo,
                Amount = convertedAmount
            };

            await using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    tabFrom = await AddMoney(dtoFrom);
                    var tabTo = await AddMoney(dtoTo);
                    await transaction.CommitAsync();

                    return new[] { tabFrom, tabTo };
                }
                catch (ApplicationException)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
                catch (ArgumentException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to make transaction with dto: {dto}");
                    await transaction.RollbackAsync();
                    throw new ApplicationException("Failed to make transaction");
                }
            }
            
        }
    }
}
