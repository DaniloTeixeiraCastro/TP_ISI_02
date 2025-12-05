using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TP_ISI_02.Domain.Interfaces;
using TP_ISI_02.Domain.Models;

namespace TP_ISI_02.API.Services.Soap
{
    /// <summary>
    /// Implementação do serviço SOAP para gestão imobiliária.
    /// Garante a interoperabilidade com sistemas baseados em XML.
    /// </summary>
    public class ImobiliariaSoapService : IImobiliariaSoapService
    {
        private readonly IImovelRepository _imovelRepository;

        /// <summary>
        /// Construtor do serviço SOAP.
        /// </summary>
        /// <param name="imovelRepository">Repositório de imóveis partilhado com a API REST.</param>
        public ImobiliariaSoapService(IImovelRepository imovelRepository)
        {
            _imovelRepository = imovelRepository;
        }

        /// <summary>
        /// Obtém a lista de todos os imóveis via SOAP.
        /// </summary>
        /// <returns>Lista de imóveis.</returns>
        public async Task<List<Imovel>> GetImoveis()
        {
            var imoveis = await _imovelRepository.GetAllAsync();
            return imoveis.ToList();
        }

        /// <summary>
        /// Obtém os detalhes de um imóvel específico via SOAP.
        /// </summary>
        /// <param name="id">Identificador do imóvel.</param>
        /// <returns>O imóvel solicitado.</returns>
        public async Task<Imovel> GetImovel(int id)
        {
            return await _imovelRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Cria um novo imóvel via SOAP.
        /// </summary>
        /// <param name="imovel">Dados do imóvel.</param>
        /// <returns>O imóvel criado.</returns>
        public async Task<Imovel> CreateImovel(Imovel imovel)
        {
            return await _imovelRepository.AddAsync(imovel);
        }
    }
}
