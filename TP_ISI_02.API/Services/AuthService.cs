using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TP_ISI_02.Domain.Interfaces;
using TP_ISI_02.Domain.Models;

namespace TP_ISI_02.API.Services
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string username, string password);
        Task<User> RegisterAsync(string username, string password, string email);
    }

    /// <summary>
    /// Serviço responsável pela autenticação e registo de utilizadores.
    /// Implementa a lógica de hashing de passwords e geração de tokens JWT.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Construtor do serviço de autenticação.
        /// </summary>
        /// <param name="userRepository">Repositório de utilizadores.</param>
        /// <param name="configuration">Configuração da aplicação (para acesso a chaves secretas).</param>
        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        /// <summary>
        /// Valida as credenciais de um utilizador e gera um token JWT se forem válidas.
        /// </summary>
        /// <param name="username">Nome de utilizador.</param>
        /// <param name="password">Palavra-passe em texto limpo.</param>
        /// <returns>Token JWT se o login for bem-sucedido, ou null caso contrário.</returns>
        public async Task<string> LoginAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null) return null;

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return null;

            return GenerateToken(user);
        }

        /// <summary>
        /// Regista um novo utilizador no sistema.
        /// </summary>
        /// <param name="username">Nome de utilizador desejado.</param>
        /// <param name="password">Palavra-passe (será guardada como hash).</param>
        /// <param name="email">Endereço de email.</param>
        /// <returns>O utilizador criado.</returns>
        public async Task<User> RegisterAsync(string username, string password, string email)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(username);
            if (existingUser != null) throw new Exception("Username already exists");

            var user = new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Email = email,
                DataCriacao = DateTime.UtcNow
            };

            return await _userRepository.AddAsync(user);
        }

        private string GenerateToken(User user)
        {
            var secretKey = _configuration["Jwt:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
