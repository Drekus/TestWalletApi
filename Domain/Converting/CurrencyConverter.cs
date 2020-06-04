using System;
using System.Collections.Generic;
using TestWalletApi.DTO;
using TestWalletApi.Models;

namespace TestWalletApi.Domain.Converting
{
    public class CurrencyConverter
    {
        private readonly IReadOnlyDictionary<CurrencyType, decimal> _currencyRates;

        public CurrencyType BaseCurrency { get; }

        public CurrencyConverter(IReadOnlyDictionary<CurrencyType, decimal> currencyRates, CurrencyType baseCurrency)
        {
            _currencyRates = currencyRates;
            BaseCurrency = baseCurrency;
        }

        public decimal Convert(ConvertMoneyDto dto)
        {
            if (dto.Amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(dto.Amount));
            }
            
            return dto.Amount / _currencyRates[dto.FromCurrency] * _currencyRates[dto.ToCurrency];
        }
    }
}
