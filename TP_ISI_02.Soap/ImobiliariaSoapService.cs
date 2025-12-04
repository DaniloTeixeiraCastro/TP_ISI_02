using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TP_ISI_02.Domain.Interfaces;
using TP_ISI_02.Domain.Models;

namespace TP_ISI_02.Soap
{
    public class ImobiliariaSoapService : IImobiliariaSoapService
    {
        private readonly IImovelRepository _imovelRepository;

        public ImobiliariaSoapService(IImovelRepository imovelRepository)
        {
            _imovelRepository = imovelRepository;
        }

        public async Task<List<Imovel>> GetImoveis()
        {
            var imoveis = await _imovelRepository.GetAllAsync();
            return imoveis.ToList();
        }

        public async Task<Imovel> GetImovel(int id)
        {
            return await _imovelRepository.GetByIdAsync(id);
        }

        public async Task<Imovel> CreateImovel(Imovel imovel)
        {
            return await _imovelRepository.AddAsync(imovel);
        }
    }
}
