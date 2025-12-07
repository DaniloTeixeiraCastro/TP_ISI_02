using System.Threading.Tasks;
using TP_ISI_02.Domain.Models;

namespace TP_ISI_02.Domain.Interfaces
{
    public interface IGoogleCalendarService
    {
        Task<string> CreateEventAsync(Evento evento);
    }
}
