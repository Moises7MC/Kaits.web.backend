using MediatR;
using Microsoft.AspNetCore.Mvc;
using Kaits.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Kaits.Domain.Entities;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : Controller
{
	private readonly IMediator _mediator;
	private readonly ILogger<ProductosController> _logger;

	public ProductosController(IMediator mediator, ILogger<ProductosController> logger)
	{
		_mediator = mediator;
		_logger = logger;
	}

	[HttpGet]
	public async Task<IActionResult> GetProductos([FromServices] KaitsDbContext db)
	{
		var productos = await db.Productos
			.Select(p => new
			{
				p.Codigo,
				p.Descripcion
			})
			.ToListAsync();

		return Ok(productos);
	}

	[HttpPost]
	public async Task<IActionResult> CreateProducto([FromBody] CreateProductoCommand command, [FromServices] KaitsDbContext db)
	{
		if (string.IsNullOrWhiteSpace(command.Codigo) || string.IsNullOrWhiteSpace(command.Descripcion))
			return BadRequest(new { message = "El código y la descripción son obligatorios." });

		var existe = await db.Productos.AnyAsync(p => p.Codigo == command.Codigo);
		if (existe)
			return Conflict(new { message = $"Ya existe un producto con el código {command.Codigo}." });

		var producto = new Producto
		{
			Codigo = command.Codigo,
			Descripcion = command.Descripcion,
			PrecioUnitario = command.PrecioUnitario
		};

		db.Productos.Add(producto);
		await db.SaveChangesAsync();

		return CreatedAtAction(nameof(GetProductos), new { codigo = producto.Codigo }, producto);
	}

	[HttpPut("{codigo}")]
	public async Task<IActionResult> UpdateProducto(
	string codigo,
	[FromBody] UpdateProductoCommand command,
	[FromServices] KaitsDbContext db)
	{
		var producto = await db.Productos.FirstOrDefaultAsync(p => p.Codigo == codigo);
		if (producto == null)
			return NotFound(new { message = $"No se encontró el producto con código {codigo}." });

		// 🔍 Validar si el nuevo código ya existe en otro producto
		if (!string.Equals(command.Codigo, codigo, StringComparison.OrdinalIgnoreCase))
		{
			bool codigoExiste = await db.Productos.AnyAsync(p => p.Codigo == command.Codigo);
			if (codigoExiste)
				return Conflict(new { message = $"Ya existe un producto con el código {command.Codigo}." });
		}

		// Actualizar datos
		producto.Codigo = command.Codigo;
		producto.Descripcion = command.Descripcion;
		producto.PrecioUnitario = command.PrecioUnitario;

		await db.SaveChangesAsync();

		return Ok(new
		{
			message = "Producto actualizado correctamente.",
			producto = new
			{
				producto.Codigo,
				producto.Descripcion,
				producto.PrecioUnitario
			}
		});
	}

	[HttpDelete("{codigo}")]
	public async Task<IActionResult> DeleteProducto(string codigo, [FromServices] KaitsDbContext db)
	{
		var producto = await db.Productos.FirstOrDefaultAsync(p => p.Codigo == codigo);
		if (producto == null)
			return NotFound(new { message = $"No se encontró el producto con código {codigo}." });

		db.Productos.Remove(producto);
		await db.SaveChangesAsync();

		return Ok(new
		{
			message = $"Producto con código {codigo} eliminado correctamente."
		});
	}
}