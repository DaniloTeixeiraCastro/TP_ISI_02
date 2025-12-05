using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ISI_02.Domain.Models;

namespace TP_ISI_02.Domain.Interfaces
{
    /// <summary>
    /// Interface que define o contrato para o repositório de Imóveis.
    /// Permite a abstração do acesso a dados.
    /// </summary>
    public interface IImovelRepository
    {
        /// <summary>
        /// Obtém todos os imóveis registados na base de dados.
        /// </summary>
        /// <returns>Uma coleção de imóveis.</returns>
        Task<IEnumerable<Imovel>> GetAllAsync();

        /// <summary>
        /// Obtém um imóvel específico pelo seu identificador único.
        /// </summary>
        /// <param name="id">O identificador do imóvel.</param>
        /// <returns>O imóvel encontrado ou null se não existir.</returns>
        Task<Imovel> GetByIdAsync(int id);

        /// <summary>
        /// Adiciona um novo imóvel à base de dados.
        /// </summary>
        /// <param name="imovel">O objeto imóvel a ser criado.</param>
        /// <returns>O imóvel criado com o ID atribuído.</returns>
        Task<Imovel> AddAsync(Imovel imovel);

        /// <summary>
        /// Atualiza os dados de um imóvel existente.
        /// </summary>
        /// <param name="imovel">O objeto imóvel com os dados atualizados.</param>
        /// <returns>Verdadeiro se a atualização for bem-sucedida, falso caso contrário.</returns>
        Task<bool> UpdateAsync(Imovel imovel);

        /// <summary>
        /// Remove um imóvel da base de dados.
        /// </summary>
        /// <param name="id">O identificador do imóvel a remover.</param>
        /// <returns>Verdadeiro se a remoção for bem-sucedida, falso caso contrário.</returns>
        Task<bool> DeleteAsync(int id);
    }
}
