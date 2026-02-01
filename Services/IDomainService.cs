using System.Threading.Tasks;
using Desafio.Umbler.Dtos;

namespace Desafio.Umbler.Services
{
    public interface IDomainService
    {
        Task<DomainDto> GetDomainAsync(string domainName);
    }
}
