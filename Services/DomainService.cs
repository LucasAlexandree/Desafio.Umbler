using Desafio.Umbler.Models;
using Desafio.Umbler.Dtos;
using Desafio.Umbler.Validators;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using DnsClient;

namespace Desafio.Umbler.Services
{
    public class DomainService : IDomainService
    {
        private readonly DatabaseContext _db;
        private readonly IWhoisClient _whoisClient;
        private readonly IDnsClient _dnsClient;

        public DomainService(
            DatabaseContext db,
            IWhoisClient whoisClient,
            IDnsClient dnsClient)
        {
            _db = db;
            _whoisClient = whoisClient;
            _dnsClient = dnsClient;
        }

        public async Task<DomainDto> GetDomainAsync(string domainName)
        {
            if (!DomainValidator.IsValid(domainName))
            {
                throw new ArgumentException("Domínio inválido.", nameof(domainName));
            }

            domainName = DomainValidator.Normalize(domainName);

            var domain = await _db.Domains.FirstOrDefaultAsync(d => d.Name == domainName);

            bool needsUpdate = domain == null || IsTtlExpired(domain);

            if (needsUpdate)
            {
                try
                {
                    domain = await UpdateDomainInfoAsync(domainName, domain);
                }
                catch (Exception ex)
                {
                    if (domain == null)
                        throw new InvalidOperationException($"Erro ao consultar informações do domínio: {ex.Message}", ex);
                }
            }

            return MapToDto(domain);
        }

        private bool IsTtlExpired(Domain domain)
        {
            if (domain.Ttl <= 0)
                return true;

            var expirationTime = domain.UpdatedAt.AddSeconds(domain.Ttl);
            return DateTime.Now > expirationTime;
        }

        private async Task<Domain> UpdateDomainInfoAsync(string domainName, Domain domain)
        {
            var whoisResponse = await _whoisClient.QueryAsync(domainName);
            
            var dnsResult = await _dnsClient.QueryAsync(domainName, QueryType.ANY);
            var aRecord = dnsResult.Answers.ARecords().FirstOrDefault();
            var ip = aRecord?.Address?.ToString();
            var ttl = aRecord?.TimeToLive ?? 0;

            var nameServers = ExtractNameServers(whoisResponse);

            string hostedAt = null;
            if (!string.IsNullOrEmpty(ip))
            {
                try
                {
                    var hostResponse = await _whoisClient.QueryAsync(ip);
                    hostedAt = hostResponse.OrganizationName;
                }
                catch
                {
                    
                }
            }

            if (domain == null)
            {
                domain = new Domain { Name = domainName };
                _db.Domains.Add(domain);
            }

            domain.Ip = ip;
            domain.WhoIs = whoisResponse.Raw;
            domain.HostedAt = hostedAt;
            domain.Ttl = ttl;
            domain.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return domain;
        }

        private string ExtractNameServers(Whois.NET.WhoisResponse whoisResponse)
        {
            if (whoisResponse == null || string.IsNullOrEmpty(whoisResponse.Raw))
                return null;

            var lines = whoisResponse.Raw.Split('\n');
            var nameServers = new System.Collections.Generic.List<string>();

            foreach (var line in lines)
            {
                var lowerLine = line.ToLower();
                if (lowerLine.Contains("name server:") || lowerLine.Contains("nserver:"))
                {
                    var parts = line.Split(':');
                    if (parts.Length > 1)
                    {
                        var ns = parts[1].Trim();
                        if (!string.IsNullOrEmpty(ns))
                            nameServers.Add(ns);
                    }
                }
            }

            return nameServers.Any() ? string.Join(", ", nameServers) : null;
        }

        private DomainDto MapToDto(Domain domain)
        {
            if (domain == null)
                return null;

            return new DomainDto
            {
                Name = domain.Name,
                Ip = domain.Ip,
                HostedAt = domain.HostedAt,
                NameServers = ExtractNameServersFromWhois(domain.WhoIs),
                WhoIs = domain.WhoIs
            };
        }

        private string ExtractNameServersFromWhois(string whoisRaw)
        {
            if (string.IsNullOrEmpty(whoisRaw))
                return null;

            var lines = whoisRaw.Split('\n');
            var nameServers = new System.Collections.Generic.List<string>();

            foreach (var line in lines)
            {
                var lowerLine = line.ToLower();
                if (lowerLine.Contains("name server:") || lowerLine.Contains("nserver:"))
                {
                    var parts = line.Split(':');
                    if (parts.Length > 1)
                    {
                        var ns = parts[1].Trim();
                        if (!string.IsNullOrEmpty(ns))
                            nameServers.Add(ns);
                    }
                }
            }

            return nameServers.Any() ? string.Join(", ", nameServers) : null;
        }
    }
}
