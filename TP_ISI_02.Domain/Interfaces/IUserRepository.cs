using System.Threading.Tasks;
using TP_ISI_02.Domain.Models;

namespace TP_ISI_02.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByUsernameAsync(string username);
        Task<User> AddAsync(User user);
    }
}
