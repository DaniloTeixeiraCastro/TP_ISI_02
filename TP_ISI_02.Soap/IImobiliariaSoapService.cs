using System.Collections.Generic;
using System.Threading.Tasks;
using CoreWCF;
using TP_ISI_02.Domain.Models;

namespace TP_ISI_02.Soap
{
    [ServiceContract]
    public interface IImobiliariaSoapService
    {
        [OperationContract]
        Task<List<Imovel>> GetImoveis();

        [OperationContract]
        Task<Imovel> GetImovel(int id);

        [OperationContract]
        Task<Imovel> CreateImovel(Imovel imovel);
    }
}
