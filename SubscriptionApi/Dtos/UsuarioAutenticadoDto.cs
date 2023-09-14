namespace SubscriptionApi.Dtos
{
    public class UsuarioAutenticadoDto
    {
        public string Token { get; set; }
        public DateTime Expiracion { get; set; }
    }
}
