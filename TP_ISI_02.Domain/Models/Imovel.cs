using System;
using System.Collections.Generic;

namespace TP_ISI_02.Domain.Models
{
    /// <summary>
    /// Representa um imóvel no sistema de gestão imobiliária.
    /// </summary>
    public class Imovel
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string Localizacao { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataAtualizacao { get; set; }
        
        // Navigation property (optional in ADO.NET but good for domain modeling)
        public virtual ICollection<Evento> Eventos { get; set; } = new List<Evento>();
    }
}
