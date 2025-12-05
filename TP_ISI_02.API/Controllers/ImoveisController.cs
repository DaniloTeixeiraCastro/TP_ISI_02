using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP_ISI_02.Domain.Interfaces;
using TP_ISI_02.Domain.Models;

namespace TP_ISI_02.API.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão de imóveis via API REST.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ImoveisController : ControllerBase
    {
        private readonly IImovelRepository _repository;

        /// <summary>
        /// Construtor do controlador de imóveis.
        /// </summary>
        /// <param name="repository">Injeção de dependência do repositório de imóveis.</param>
        public ImoveisController(IImovelRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Obtém a lista completa de imóveis.
        /// </summary>
        /// <returns>Uma lista de imóveis.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Imovel>>> GetImoveis()
        {
            var imoveis = await _repository.GetAllAsync();
            return Ok(imoveis);
        }

        /// <summary>
        /// Obtém os detalhes de um imóvel específico.
        /// </summary>
        /// <param name="id">O identificador do imóvel.</param>
        /// <returns>O imóvel solicitado ou NotFound se não existir.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Imovel>> GetImovel(int id)
        {
            var imovel = await _repository.GetByIdAsync(id);
            if (imovel == null)
            {
                return NotFound();
            }
            return Ok(imovel);
        }

        /// <summary>
        /// Cria um novo imóvel. Requer autenticação.
        /// </summary>
        /// <param name="imovel">Os dados do novo imóvel.</param>
        /// <returns>O imóvel criado.</returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Imovel>> PostImovel(Imovel imovel)
        {
            var novoImovel = await _repository.AddAsync(imovel);
            return CreatedAtAction(nameof(GetImovel), new { id = novoImovel.Id }, novoImovel);
        }

        /// <summary>
        /// Atualiza um imóvel existente. Requer autenticação.
        /// </summary>
        /// <param name="id">O identificador do imóvel a atualizar.</param>
        /// <param name="imovel">Os novos dados do imóvel.</param>
        /// <returns>NoContent se bem-sucedido, ou erro correspondente.</returns>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutImovel(int id, Imovel imovel)
        {
            if (id != imovel.Id)
            {
                return BadRequest();
            }

            var updated = await _repository.UpdateAsync(imovel);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Remove um imóvel. Requer autenticação.
        /// </summary>
        /// <param name="id">O identificador do imóvel a remover.</param>
        /// <returns>NoContent se bem-sucedido.</returns>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImovel(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
