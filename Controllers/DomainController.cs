using Desafio.Umbler.Services;
using Desafio.Umbler.Validators;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Desafio.Umbler.Controllers
{
    [Route("api/domain")]
    [ApiController]
    public class DomainController : ControllerBase
    {
        private readonly IDomainService _service;

        public DomainController(IDomainService service)
        {
            _service = service;
        }

        [HttpGet("{domainName}")]
        public async Task<IActionResult> Get(string domainName)
        {
            if (!DomainValidator.IsValid(domainName))
            {
                return BadRequest(new { error = "Domínio inválido. Por favor, informe um domínio válido (ex: exemplo.com)" });
            }

            try
            {
                var domain = await _service.GetDomainAsync(domainName);
                
                if (domain == null)
                {
                    return NotFound(new { error = "Domínio não encontrado." });
                }

                return Ok(domain);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Erro interno do servidor ao processar a requisição." });
            }
        }
    }
}
