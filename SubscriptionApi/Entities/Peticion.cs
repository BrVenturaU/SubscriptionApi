namespace SubscriptionApi.Entities
{
    public class Peticion
    {
        public int Id { get; set; }
        public int LlaveId { get; set; }
        public string Ruta { get; set; }
        public DateTime FechaPeticion { get; set; }
        public LlaveApi Llave { get; set; }
    }
}
