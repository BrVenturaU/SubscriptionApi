namespace SubscriptionApi.Entities
{
    public class RestriccionIp
    {
        public int Id { get; set; }
        public int LlaveId { get; set; }
        public string Ip { get; set; }
        public LlaveApi Llave { get; set; }
    }
}
