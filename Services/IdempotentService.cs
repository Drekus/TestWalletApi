using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TestWalletApi.Data;
using TestWalletApi.DTO;
using TestWalletApi.Models;

namespace TestWalletApi.Services
{
    public class IdempotentService: IIdempotentService
    {
        private  ApplicationDbContext _db;

        public IdempotentService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool?> CheckIdempotent(string idempotentKey, BaseMoneyDto parameters)
        {
            var userId = await GetUserId(parameters.WalletId);
            if (userId == null)
                return null;
            
            var hash = GetHashDto(parameters);

            var row = await _db.IdempotentCaches.FindAsync(userId, idempotentKey, hash);
            if (row != null) 
                return false;
            
            await _db.IdempotentCaches.AddAsync(new IdempotentCache
            {
                UserId = userId.Value,
                IdempotentKey = idempotentKey,
                ParametersHash = hash
            });
            await _db.SaveChangesAsync();
            return true;
        }
        
        private async Task<long?> GetUserId(long walletId)
        {
            // UserId хотел брать у контекстного пользователя
            // Но т.к. аутентификация по Т.З. не нужна 
            // не стал тратить время на реализацию.
            
            var wallet = await _db.Wallets.FindAsync(walletId);
            if (wallet == null)
                return null;

            return wallet.UserId;
        }

        private static string GetHashDto(BaseMoneyDto parameters)
        {
            //Т.к. быстро-быстро не нашел готовый nuget-packet
            // сделал простейшую реализацию сам
            // для прода делал бы по-другому, но для тестового ограничен по времени
            
            var encoding = new ASCIIEncoding();
            var key = encoding.GetBytes("072e77e426f92738a72fe23c4d1953b4");
            var hmac = new HMACSHA1(key);
            var paramStr = GetXmlFroDto(parameters);
            var bytes = hmac.ComputeHash(encoding.GetBytes(paramStr));
            return Convert.ToBase64String(bytes);
        }

        private static string GetXmlFroDto(BaseMoneyDto parameters)
        {
            var strWriter = new StringWriter();
            XmlTextWriter textWriter = null;
            try
            {
                var serializer = new XmlSerializer(parameters.GetType());
                textWriter = new XmlTextWriter(strWriter);
                serializer.Serialize(textWriter, parameters);
            }
            catch (Exception ex)
            {
                throw new AggregateException("Failed to serialize parameters"); 
            }
            finally
            {
                strWriter.Close();
                textWriter?.Close();
            }
            return strWriter.ToString();
        }
    }
}