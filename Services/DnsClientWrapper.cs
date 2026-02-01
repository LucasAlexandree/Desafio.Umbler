using System.Threading;
using System.Threading.Tasks;
using DnsClient;

namespace Desafio.Umbler.Services
{
    public class DnsClientWrapper : IDnsClient
    {
        private readonly LookupClient _lookupClient;

        public DnsClientWrapper()
        {
            _lookupClient = new LookupClient();
        }

        public async Task<IDnsQueryResponse> QueryAsync(string query, QueryType queryType, QueryClass queryClass = QueryClass.IN, CancellationToken cancellationToken = default)
        {
            return await _lookupClient.QueryAsync(query, queryType, queryClass, cancellationToken);
        }
    }
}
