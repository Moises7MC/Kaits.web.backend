using MediatR;
using Microsoft.EntityFrameworkCore;
using Kaits.Infrastructure.Persistence;
using Kaits.Domain.Entities;
using Microsoft.Extensions.Logging;
using Kaits.Application.Exceptions;

public class CreatePedidoHandler : IRequestHandler<CreatePedidoCommand, CreatePedidoResult>
{
	private readonly KaitsDbContext _db;
	private readonly ILogger<CreatePedidoHandler> _logger;

	public CreatePedidoHandler(KaitsDbContext db, ILogger<CreatePedidoHandler> logger)
	{
		_db = db;
		_logger = logger;
	}

	public async Task<CreatePedidoResult> Handle(CreatePedidoCommand request, CancellationToken cancellationToken)
	{
		var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.Codigo == request.ClienteCodigo, cancellationToken);
		if (cliente == null)
			throw new KeyNotFoundException($"Cliente con código '{request.ClienteCodigo}' no existe.");

		if (request.Items == null || !request.Items.Any())
			throw new BusinessValidationException("El pedido debe contener al menos un producto.");

		var productCodes = request.Items.Select(i => i.ProductoCodigo).Distinct().ToList();
		var productos = await _db.Productos
			.Where(p => productCodes.Contains(p.Codigo))
			.ToListAsync(cancellationToken);

		if (productos.Count != productCodes.Count)
		{
			var missing = productCodes.Except(productos.Select(p => p.Codigo));
			throw new KeyNotFoundException($"Productos no encontrados: {string.Join(',', missing)}");
		}

		var pedido = new Pedido
		{
			FechaOrden = DateTime.UtcNow,
			ClienteCodigo = cliente.Codigo,
			PrecioTotal = 0m
		};

		foreach (var item in request.Items)
		{
			if (item.Cantidad <= 0)
				throw new BusinessValidationException($"Cantidad para producto {item.ProductoCodigo} debe ser > 0.");

			var producto = productos.First(p => p.Codigo == item.ProductoCodigo);

			if (producto.PrecioUnitario <= 0)
				throw new BusinessValidationException($"Precio unitario inválido para producto {producto.Codigo}.");

			var subtotal = producto.PrecioUnitario * item.Cantidad;
			var detalle = new DetallePedido
			{
				ProductoCodigo = producto.Codigo,
				ProductoDescripcion = producto.Descripcion,
				Cantidad = item.Cantidad,
				PrecioUnitario = producto.PrecioUnitario,
				Subtotal = subtotal
			};
			pedido.Detalles.Add(detalle);
			pedido.PrecioTotal += subtotal;
		}

		using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
		try
		{
			_db.Pedidos.Add(pedido);
			await _db.SaveChangesAsync(cancellationToken);
			await tx.CommitAsync(cancellationToken);

			_logger.LogInformation("Pedido {PedidoId} creado para cliente {Cliente}", pedido.Id, cliente.Codigo);
			return new CreatePedidoResult(pedido.Id, pedido.PrecioTotal, pedido.FechaOrden);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error creando pedido");
			await tx.RollbackAsync(cancellationToken);
			throw;
		}
	}
}