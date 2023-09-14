using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SubscriptionApi.Entities;

namespace SubscriptionApi.Dtos
{
    public class LlaveDto
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public bool Activa { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TipoLlave TipoLlave { get; set; }

        public List<RestriccionDominioDto> RestriccionesDominio { get; set; }
        public List<RestriccionIpDto> RestriccionesIp { get; set; }
    }
}
