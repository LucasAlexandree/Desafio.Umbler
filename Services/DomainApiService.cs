using Desafio.Umbler.Dtos;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Desafio.Umbler.Services
{
    public interface IDomainApiService
    {
        Task<DomainDto> GetDomainAsync(string domainName);
    }

    public class DomainApiService : IDomainApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DomainApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<DomainDto> GetDomainAsync(string domainName)
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            var baseUrl = request != null 
                ? $"{request.Scheme}://{request.Host}"
                : "https://localhost:5001";
            
            var uri = new Uri($"{baseUrl}/api/domain/{Uri.EscapeDataString(domainName)}");
            var response = await _httpClient.GetAsync(uri);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<DomainDto>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                throw new DomainNotFoundException(error?.Error ?? "Domínio não encontrado.");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                throw new DomainValidationException(error?.Error ?? "Domínio inválido.");
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                throw new DomainServiceException(error?.Error ?? "Erro ao consultar informações do domínio.");
            }
        }
    }

    public class ErrorResponse
    {
        public string Error { get; set; }
    }

    public class DomainNotFoundException : Exception
    {
        public DomainNotFoundException(string message) : base(message) { }
    }

    public class DomainValidationException : Exception
    {
        public DomainValidationException(string message) : base(message) { }
    }

    public class DomainServiceException : Exception
    {
        public DomainServiceException(string message) : base(message) { }
    }
}
