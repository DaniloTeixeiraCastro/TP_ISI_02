using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP_ISI_02.Domain.Interfaces;
using TP_ISI_02.Domain.Models;

namespace TP_ISI_02.API.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão de eventos (visitas, reuniões) via API REST.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventosController : ControllerBase
    {
        private readonly IEventoRepository _repository;
        private readonly IGoogleCalendarService _calendarService;

        /// <summary>
        /// Construtor do controlador de eventos.
        /// </summary>
        /// <param name="repository">Repositório de eventos.</param>
        /// <param name="calendarService">Serviço de integração com Google Calendar.</param>
        public EventosController(IEventoRepository repository, IGoogleCalendarService calendarService)
        {
            _repository = repository;
            _calendarService = calendarService;
        }

        /// <summary>
        /// Obtém a lista de todos os eventos.
        /// </summary>
        /// <returns>Lista de eventos.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Evento>>> GetEventos()
        {
            var eventos = await _repository.GetAllAsync();
            return Ok(eventos);
        }

        /// <summary>
        /// Obtém um evento específico pelo seu ID.
        /// </summary>
        /// <param name="id">ID do evento.</param>
        /// <returns>Detalhes do evento.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Evento>> GetEvento(int id)
        {
            var evento = await _repository.GetByIdAsync(id);
            if (evento == null)
            {
                return NotFound();
            }
            return Ok(evento);
        }

        /// <summary>
        /// Obtém todos os eventos associados a um imóvel específico.
        /// </summary>
        /// <param name="imovelId">ID do imóvel.</param>
        /// <returns>Lista de eventos relacionados com o imóvel.</returns>
        [HttpGet("imovel/{imovelId}")]
        public async Task<ActionResult<IEnumerable<Evento>>> GetEventosByImovel(int imovelId)
        {
            var eventos = await _repository.GetByImovelIdAsync(imovelId);
            return Ok(eventos);
        }

        /// <summary>
        /// Cria um novo evento e agenda no Google Calendar.
        /// </summary>
        /// <param name="evento">Dados do novo evento.</param>
        /// <returns>O evento criado.</returns>
        [HttpPost]
        public async Task<ActionResult<Evento>> PostEvento(Evento evento)
        {
            var novoEvento = await _repository.AddAsync(evento);
            
            // Integrate with Google Calendar (Fire and forget, or wait? Let's wait to return result)
            var calendarResult = await _calendarService.CreateEventAsync(novoEvento);
            
            // Optionally, we could append the calendar result to the response or log it.
            // For now, we just proceed.
            
            return CreatedAtAction(nameof(GetEvento), new { id = novoEvento.Id }, novoEvento);
        }

        /// <summary>
        /// Atualiza um evento existente.
        /// </summary>
        /// <param name="id">ID do evento.</param>
        /// <param name="evento">Novos dados do evento.</param>
        /// <returns>Sem conteúdo se sucesso.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvento(int id, Evento evento)
        {
            if (id != evento.Id)
            {
                return BadRequest();
            }

            var updated = await _repository.UpdateAsync(evento);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Remove um evento do sistema.
        /// </summary>
        /// <param name="id">ID do evento.</param>
        /// <returns>Sem conteúdo se sucesso.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvento(int id)
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
