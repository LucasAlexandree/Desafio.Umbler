using System.Text.RegularExpressions;

namespace Desafio.Umbler.Validators
{
    public static class DomainValidator
    {
        private static readonly Regex DomainRegex = new Regex(
            @"^([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,}$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static bool IsValid(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
                return false;

            domain = domain.Trim();

            if (!domain.Contains("."))
                return false;

            return DomainRegex.IsMatch(domain);
        }

        public static string Normalize(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
                return string.Empty;

            return domain.Trim().ToLower();
        }
    }
}
