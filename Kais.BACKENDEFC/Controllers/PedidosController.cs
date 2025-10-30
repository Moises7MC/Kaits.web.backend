using MediatR;
using Microsoft.AspNetCore.Mvc;
using Kaits.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
	private readonly IMediator _mediator;
	private readonly ILogger<PedidosController> _logger;

	public PedidosController(IMediator mediator, ILogger<PedidosController> logger)
	{
		_mediator = mediator;
		_logger = logger;
	}

	[HttpGet]
	[Route("", Name = "GetAllPedidos")]
	public async Task<IActionResult> GetAll([FromServices] KaitsDbContext db)
	{
		var pedidos = await db.Pedidos
			.Include(p => p.Detalles)
			.ToListAsync();

		if (pedidos.Count == 0)
			return Ok(new { message = "No hay pedidos registrados." });

		var codigosClientes = pedidos.Select(p => p.ClienteCodigo).Distinct().ToList();
		var clientes = await db.Clientes
			.Where(c => codigosClientes.Contains(c.Codigo))
			.ToListAsync();

		var result = pedidos.Select(p =>
		{
			var cliente = clientes.FirstOrDefault(c => c.Codigo == p.ClienteCodigo);

			return new
			{
				p.Id,
				p.FechaOrden,
				Cliente = new
				{
					cliente?.Codigo,
					cliente?.Nombre,
					cliente?.DNI
				},
				Total = p.PrecioTotal,
				Detalles = p.Detalles.Select(d => new
				{
					d.ProductoCodigo,
					d.ProductoDescripcion,
					d.Cantidad,
					d.PrecioUnitario,
					d.Subtotal
				})
			};
		});

		return Ok(result);
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] CreatePedidoCommand command)
	{
		try
		{
			var result = await _mediator.Send(command);
			return CreatedAtAction(nameof(GetById), new { id = result.PedidoId }, result);
		}
		catch (FluentValidation.ValidationException fv)
		{
			return BadRequest(new { errors = fv.Errors.Select(e => e.ErrorMessage) });
		}
		catch (KeyNotFoundException knf)
		{
			return NotFound(new { message = knf.Message });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error en Create Pedido");
			return StatusCode(500, new { message = "Ocurrió un error inesperado." });
		}
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetById(int id, [FromServices] KaitsDbContext db)
	{
		var pedido = await db.Pedidos
			.Include(p => p.Detalles)
			.FirstOrDefaultAsync(p => p.Id == id);

		if (pedido == null)
			return NotFound(new { message = $"No se encontró el pedido con Id {id}" });

		var cliente = await db.Clientes
			.FirstOrDefaultAsync(c => c.Codigo == pedido.ClienteCodigo);

		var result = new
		{
			pedido.Id,
			pedido.FechaOrden,
			Cliente = new
			{
				cliente?.Codigo,
				cliente?.Nombre,
				cliente?.DNI
			},
			Detalles = pedido.Detalles.Select(d => new
			{
				d.ProductoCodigo,
				d.ProductoDescripcion,
				d.Cantidad,
				d.PrecioUnitario,
				d.Subtotal
			}),
			Total = pedido.PrecioTotal
		};

		return Ok(result);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(int id, [FromBody] UpdatePedidoCommand command)
	{
		if (id != command.PedidoId)
			return BadRequest(new { message = "El ID del pedido en la URL no coincide con el del cuerpo de la solicitud." });

		try
		{
			var result = await _mediator.Send(command);
			return Ok(result);
		}
		catch (FluentValidation.ValidationException fv)
		{
			return BadRequest(new { errors = fv.Errors.Select(e => e.ErrorMessage) });
		}
		catch (KeyNotFoundException knf)
		{
			return NotFound(new { message = knf.Message });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error al actualizar pedido");
			return StatusCode(500, new { message = "Ocurrió un error inesperado." });
		}
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(int id, [FromServices] KaitsDbContext db)
	{
		try
		{
			var pedido = await db.Pedidos
				.Include(p => p.Detalles)
				.FirstOrDefaultAsync(p => p.Id == id);

			if (pedido == null)
				return NotFound(new { message = $"No se encontró el pedido con Id {id}" });

			db.DetallePedidos.RemoveRange(pedido.Detalles);

			db.Pedidos.Remove(pedido);

			await db.SaveChangesAsync();

			return NoContent();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error eliminando pedido con Id {PedidoId}", id);
			return StatusCode(500, new { message = "Ocurrió un error al eliminar el pedido." });
		}
	}
}