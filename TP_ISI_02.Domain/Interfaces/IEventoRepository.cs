using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ISI_02.Domain.Models;

namespace TP_ISI_02.Domain.Interfaces
{
    public interface IEventoRepository
    {
        Task<IEnumerable<Evento>> GetAllAsync();
        Task<Evento> GetByIdAsync(int id);
        Task<IEnumerable<Evento>> GetByImovelIdAsync(int imovelId);
        Task<Evento> AddAsync(Evento evento);
        Task<bool> UpdateAsync(Evento evento);
        Task<bool> DeleteAsync(int id);
    }
}
