namespace TestWalletApi.Models
{
    //Предпологается, что в этой таблице данные живут сутки, а потом удаляются. Удаление не реализовал.
    public class IdempotentCache
    {
        public string IdempotentKey { get; set; }

        public string ParametersHash { get; set; }

        public long UserId { get; set; }
    }
}
