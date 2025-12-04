using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ISI_02.Domain.Models;

namespace TP_ISI_02.Domain.Interfaces
{
    public interface IImovelRepository
    {
        Task<IEnumerable<Imovel>> GetAllAsync();
        Task<Imovel> GetByIdAsync(int id);
        Task<Imovel> AddAsync(Imovel imovel);
        Task<bool> UpdateAsync(Imovel imovel);
        Task<bool> DeleteAsync(int id);
    }
}
