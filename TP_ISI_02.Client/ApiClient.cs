using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TP_ISI_02.Client
{
    /// <summary>
    /// Cliente HTTP responsável pela comunicação com a API REST na Cloud.
    /// </summary>
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private string _authToken;

        // URL da API na Cloud
        private const string BaseUrl = "https://tpisi0220251130195049-crfgeafnevdufhas.francecentral-01.azurewebsites.net/";

        /// <summary>
        /// Inicializa uma nova instância do cliente API.
        /// </summary>
        public ApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);
        }

        /// <summary>
        /// Realiza o login na API e guarda o token de autenticação.
        /// </summary>
        /// <param name="username">Nome de utilizador.</param>
        /// <param name="password">Palavra-passe.</param>
        /// <returns>Verdadeiro se o login for bem-sucedido.</returns>
        public async Task<bool> LoginAsync(string username, string password)
        {
            var loginRequest = new LoginRequest { Username = username, Password = password };
            var json = JsonConvert.SerializeObject(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("api/Auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseString);
                    
                    _authToken = loginResponse.Token;
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                    return true;
                }
                else
                {
                    Console.WriteLine($"Erro no Login: {response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro de Ligação: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Imovel>> GetImoveisAsync()
        {
            if (string.IsNullOrEmpty(_authToken))
            {
                Console.WriteLine("Erro: Não autenticado.");
                return null;
            }

            try
            {
                var response = await _httpClient.GetAsync("api/Imoveis");

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Imovel>>(responseString);
                }
                else
                {
                    Console.WriteLine($"Erro ao obter imóveis: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro de Ligação: {ex.Message}");
                return null;
            }
        }

        public async Task<WeatherDto> GetWeatherAsync(string city)
        {
            if (string.IsNullOrEmpty(_authToken))
            {
                Console.WriteLine("Erro: Não autenticado.");
                return null;
            }

            try
            {
                var response = await _httpClient.GetAsync($"api/Weather/{city}");

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<WeatherDto>(responseString);
                }
                else
                {
                    Console.WriteLine($"Erro ao obter meteorologia: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro de Ligação: {ex.Message}");
                return null;
            }
        }
    }
}
