using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TP_ISI_02.API.Services;
using TP_ISI_02.Domain.Models;

namespace TP_ISI_02.API.Controllers
{
    /// <summary>
    /// Controlador responsável pela autenticação e registo de utilizadores.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Construtor do serviço de autenticação.
        /// </summary>
        /// <param name="authService">Serviço de autenticação injetado.</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Realiza o login (autenticação) de um utilizador.
        /// </summary>
        /// <param name="request">Credenciais (username e password).</param>
        /// <returns>Token JWT se as credenciais forem válidas.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _authService.LoginAsync(request.Username, request.Password);
            if (token == null)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(new { token });
        }

        /// <summary>
        /// Regista um novo utilizador no sistema.
        /// </summary>
        /// <param name="request">Dados do novo utilizador.</param>
        /// <returns>Dados do utilizador criado.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var user = await _authService.RegisterAsync(request.Username, request.Password, request.Email);
                return Ok(user);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    /// <summary>
    /// Modelo para o pedido de login.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>Nome de utilizador.</summary>
        public string Username { get; set; }
        /// <summary>Palavra-passe.</summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// Modelo para o pedido de registo.
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>Nome de utilizador desejado.</summary>
        public string Username { get; set; }
        /// <summary>Palavra-passe.</summary>
        public string Password { get; set; }
        /// <summary>Email de contacto.</summary>
        public string Email { get; set; }
    }
}
