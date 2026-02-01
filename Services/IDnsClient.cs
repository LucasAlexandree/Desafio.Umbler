using System.Threading;
using System.Threading.Tasks;
using DnsClient;

namespace Desafio.Umbler.Services
{
    public interface IDnsClient
    {
        Task<IDnsQueryResponse> QueryAsync(string query, QueryType queryType, QueryClass queryClass = QueryClass.IN, CancellationToken cancellationToken = default);
    }
}
