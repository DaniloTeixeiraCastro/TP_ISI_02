using System;

namespace TP_ISI_02.Domain.Models
{
    /// <summary>
    /// Representa um evento (visita, reunião) associado a um imóvel.
    /// </summary>
    public class Evento
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public int ImovelId { get; set; }
        public virtual Imovel Imovel { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataAtualizacao { get; set; }
    }
}
