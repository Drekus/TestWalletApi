using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using TestWalletApi.Models;

namespace TestWalletApi.Domain.Converting
{
    public class CurrencyRatesFromEuroBankGetter: ICurrencyRatesGetter
    {
        private readonly IHttpClientFactory _clientFactory;

        //Это можно вынести в конфиги
        public const CurrencyType BASE_CURRENCY = CurrencyType.EUR;
        private const string SOURCE_URL = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";

        public CurrencyRatesFromEuroBankGetter(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        
        public async Task<Dictionary<CurrencyType, decimal>> GetRates()
        {
            var client = _clientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get,SOURCE_URL);
            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Code: {response.StatusCode}. {response.ReasonPhrase}");
            }

            var currencyRates = new Dictionary<CurrencyType, decimal> {[BASE_CURRENCY] = 1};
            
            var responseStr = await response.Content.ReadAsStringAsync();
            dynamic xml = XDocument.Parse(responseStr).Root;
            dynamic item = xml.LastNode.FirstNode.FirstNode;

            do
            {
                var currencyTypeStr = item.FirstAttribute.Value;
                var rateStr = item.LastAttribute.Value;

                var isExist = Enum.TryParse(currencyTypeStr, out CurrencyType currencyType);
                if (!isExist)
                {
                    item = item.NextNode;
                    continue;
                }
                
                var rate = decimal.Parse(rateStr, CultureInfo.InvariantCulture);

                currencyRates[currencyType] = rate;
                item = item.NextNode;
            }
            while (item.NextNode != null);

            return currencyRates;
        }
    }
}