using System.Threading.Tasks;
using Whois.NET;

namespace Desafio.Umbler.Services
{
    public class WhoisClientWrapper : IWhoisClient
    {
        public async Task<WhoisResponse> QueryAsync(string query)
        {
            return await WhoisClient.QueryAsync(query);
        }
    }
}
