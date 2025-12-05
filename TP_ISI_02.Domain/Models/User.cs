using System;
using System;

namespace TP_ISI_02.Domain.Models
{
    /// <summary>
    /// Representa um utilizador para autenticação.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Identificador único do utilizador.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome de utilizador para login.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Hash da palavra-passe (nunca guardar em texto limpo).
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Endereço de email do utilizador.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Data de registo do utilizador.
        /// </summary>
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    }
}
