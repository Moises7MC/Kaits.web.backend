using MediatR;
using Microsoft.AspNetCore.Mvc;
using Kaits.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : Controller
{
	private readonly IMediator _mediator;
	private readonly ILogger<ClientesController> _logger;

	public ClientesController(IMediator mediator, ILogger<ClientesController> logger)
	{
		_mediator = mediator;
		_logger = logger;
	}

	[HttpGet]
	public async Task<IActionResult> GetClientes([FromServices] KaitsDbContext db)
	{
		var clientes = await db.Clientes
			.Select(c => new
			{
				c.Id,        
				c.Codigo,
				c.Nombre,
				c.DNI
			})
			.ToListAsync();

		return Ok(clientes);
	}

	[HttpGet("{codigo}")]
	public async Task<IActionResult> GetClienteByCodigo(string codigo, [FromServices] KaitsDbContext db)
	{
		var cliente = await db.Clientes.FirstOrDefaultAsync(c => c.Codigo == codigo);

		if (cliente == null)
			return NotFound(new { message = $"No se encontró el cliente con código {codigo}." });

		return Ok(cliente);
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] CreateClienteCommand command)
	{
		try
		{
			var nuevoId = await _mediator.Send(command);

			return CreatedAtAction(nameof(GetClientes), new { id = nuevoId }, new
			{
				message = "Cliente creado correctamente.",
				clienteId = nuevoId
			});
		}
		catch (FluentValidation.ValidationException fv)
		{
			return BadRequest(new { errors = fv.Errors.Select(e => e.ErrorMessage) });
		}
		catch (InvalidOperationException ioe)
		{
			return Conflict(new { message = ioe.Message }); // 409
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error al crear cliente");
			return StatusCode(500, new { message = "Ocurrió un error inesperado." });
		}
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(int id, [FromBody] UpdateClienteCommand command)
	{
		try
		{
			command.Id = id;

			await _mediator.Send(command);

			return Ok(new { message = "Cliente actualizado correctamente." });
		}
		catch (FluentValidation.ValidationException fv)
		{
			return BadRequest(new { errors = fv.Errors.Select(e => e.ErrorMessage) });
		}
		catch (KeyNotFoundException knf)
		{
			return NotFound(new { message = knf.Message });
		}
		catch (InvalidOperationException ioe)
		{
			return Conflict(new { message = ioe.Message });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error al actualizar cliente");
			return StatusCode(500, new { message = "Ocurrió un error inesperado." });
		}
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(int id)
	{
		try
		{
			await _mediator.Send(new DeleteClienteCommand { Id = id });
			return Ok(new { message = "Cliente eliminado correctamente." });
		}
		catch (KeyNotFoundException knf)
		{
			return NotFound(new { message = knf.Message });
		}
		catch (InvalidOperationException ioe)
		{
			return Conflict(new { message = ioe.Message });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error al eliminar cliente");
			return StatusCode(500, new { message = "Ocurrió un error inesperado." });
		}
	}
}