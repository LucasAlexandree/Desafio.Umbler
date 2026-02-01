using System;
using System.ComponentModel.DataAnnotations;

namespace Desafio.Umbler.Models
{
    public class Domain
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Ip { get; set; }
        public string WhoIs { get; set; }
        public string HostedAt { get; set; }
        public int Ttl { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}