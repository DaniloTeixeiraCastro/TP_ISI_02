using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;

namespace TP_ISI_02.Domain.Models
{
    /// <summary>
    /// Representa um imóvel no sistema de gestão imobiliária.
    /// </summary>
    public class Imovel
    {
        /// <summary>
        /// Identificador único do imóvel.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Título ou designação comercial do imóvel.
        /// </summary>
        public string Titulo { get; set; } = string.Empty;
        /// <summary>
        /// Descrição detalhada das características do imóvel.
        /// </summary>
        public string Descricao { get; set; } = string.Empty;
        /// <summary>
        /// Preço de venda ou arrendamento do imóvel.
        /// </summary>
        public decimal Preco { get; set; }
        /// <summary>
        /// Localização geográfica (Cidade/Região) do imóvel.
        /// </summary>
        public string Localizacao { get; set; } = string.Empty;
        /// <summary>
        /// Data em que o registo foi criado no sistema.
        /// </summary>
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataAtualizacao { get; set; }
        
        // Navigation property (optional in ADO.NET but good for domain modeling)
        public virtual ICollection<Evento> Eventos { get; set; } = new List<Evento>();
    }
}
