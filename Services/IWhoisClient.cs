using System.Threading.Tasks;
using Whois.NET;

namespace Desafio.Umbler.Services
{
    public interface IWhoisClient
    {
        Task<WhoisResponse> QueryAsync(string query);
    }
}
