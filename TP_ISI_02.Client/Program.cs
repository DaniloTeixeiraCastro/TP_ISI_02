using System;
using System.Threading.Tasks;

namespace TP_ISI_02.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=============================================");
            Console.WriteLine("   TP_ISI_02 - Cliente Imobiliária (Cloud)   ");
            Console.WriteLine("=============================================");

            var client = new ApiClient();
            bool exit = false;
            bool isAuthenticated = false;

            while (!exit)
            {
                if (!isAuthenticated)
                {
                    Console.WriteLine("\n--- LOGIN ---");
                    Console.Write("Username: ");
                    string username = Console.ReadLine();
                    Console.Write("Password: ");
                    string password = Console.ReadLine();

                    Console.WriteLine("A autenticar...");
                    isAuthenticated = await client.LoginAsync(username, password);

                    if (isAuthenticated)
                    {
                        Console.WriteLine("Login com Sucesso! 🔓");
                    }
                    else
                    {
                        Console.WriteLine("Login Falhou. Tente novamente.");
                    }
                }
                else
                {
                    Console.WriteLine("\n--- MENU ---");
                    Console.WriteLine("1. Listar Imóveis");
                    Console.WriteLine("2. Ver Meteorologia 🌦️");
                    Console.WriteLine("0. Sair");
                    Console.Write("Opção: ");
                    var option = Console.ReadLine();

                    switch (option)
                    {
                        case "1":
                            await ListarImoveis(client);
                            break;
                        case "2":
                            await VerMeteorologia(client);
                            break;
                        case "0":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Opção inválida.");
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Obtém e exibe a lista de imóveis da API.
        /// </summary>
        /// <param name="client">Instância do cliente API autenticado.</param>
        static async Task ListarImoveis(ApiClient client)
        {
            Console.WriteLine("\n--- Lista de Imóveis ---");
            var imoveis = await client.GetImoveisAsync();

            if (imoveis != null && imoveis.Count > 0)
            {
                foreach (var imovel in imoveis)
                {
                    Console.WriteLine($"[{imovel.Id}] {imovel.Titulo} - {imovel.Preco:C} ({imovel.Localizacao})");
                }
            }
            else
            {
                Console.WriteLine("Nenhum imóvel encontrado.");
            }
        }

        /// <summary>
        /// Obtém e exibe a meteorologia para uma cidade.
        /// </summary>
        /// <param name="client">Instância do cliente API autenticado.</param>
        static async Task VerMeteorologia(ApiClient client)
        {
            Console.Write("\nCidade: ");
            string cidade = Console.ReadLine();

            Console.WriteLine("A obter dados...");
            var weather = await client.GetWeatherAsync(cidade);

            if (weather != null)
            {
                Console.WriteLine($"\n--- Meteorologia em {weather.City} ---");
                Console.WriteLine($"🌡️ Temperatura: {weather.Temperature}°C");
                Console.WriteLine($"☁️ Estado: {weather.Description}");
                Console.WriteLine($"💧 Humidade: {weather.Humidity}%");
            }
        }
    }
}
