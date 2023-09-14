using Microsoft.AspNetCore.Identity;

namespace SubscriptionApi.Entities
{
    public class LlaveApi
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public TipoLlave TipoLlave { get; set; }
        public bool Activa { get; set; } = true;
        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public List<RestriccionDominio> RestriccionesDominio { get; set; }
        public List<RestriccionIp> RestriccionesIp { get; set; }
    }

    public enum TipoLlave
    {
        GRATUITA = 1,
        PROFESIONAL
    }
}
