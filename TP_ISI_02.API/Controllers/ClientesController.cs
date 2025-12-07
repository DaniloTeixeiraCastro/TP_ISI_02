using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP_ISI_02.Domain.Interfaces;
using TP_ISI_02.Domain.Models;

namespace TP_ISI_02.API.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão de clientes via API REST.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteRepository _repository;

        /// <summary>
        /// Construtor do controlador de clientes.
        /// </summary>
        /// <param name="repository">Repositório de clientes.</param>
        public ClientesController(IClienteRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Obtém a lista de todos os clientes.
        /// </summary>
        /// <returns>Lista de clientes.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            var clientes = await _repository.GetAllAsync();
            return Ok(clientes);
        }

        /// <summary>
        /// Obtém um cliente pelo seu ID.
        /// </summary>
        /// <param name="id">ID do cliente.</param>
        /// <returns>Dados do cliente.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            var cliente = await _repository.GetByIdAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return Ok(cliente);
        }

        /// <summary>
        /// Cria um novo cliente.
        /// </summary>
        /// <param name="cliente">Dados do novo cliente.</param>
        /// <returns>Cliente criado.</returns>
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
        {
            var novoCliente = await _repository.AddAsync(cliente);
            return CreatedAtAction(nameof(GetCliente), new { id = novoCliente.Id }, novoCliente);
        }

        /// <summary>
        /// Atualiza os dados de um cliente existente.
        /// </summary>
        /// <param name="id">ID do cliente.</param>
        /// <param name="cliente">Novos dados do cliente.</param>
        /// <returns>Sem conteúdo se bem sucedido.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return BadRequest();
            }

            var updated = await _repository.UpdateAsync(cliente);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Remove um cliente do sistema.
        /// </summary>
        /// <param name="id">ID do cliente.</param>
        /// <returns>Sem conteúdo se bem sucedido.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
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
