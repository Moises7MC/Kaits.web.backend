using Kaits.Domain.Entities;
using Kaits.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class UpdatePedidoHandler : IRequestHandler<UpdatePedidoCommand, UpdatePedidoResult>
{
	private readonly KaitsDbContext _db;

	public UpdatePedidoHandler(KaitsDbContext db)
	{
		_db = db;
	}

	public async Task<UpdatePedidoResult> Handle(UpdatePedidoCommand request, CancellationToken cancellationToken)
	{
		var pedido = await _db.Pedidos
			.Include(p => p.Detalles)
			.FirstOrDefaultAsync(p => p.Id == request.PedidoId, cancellationToken);

		if (pedido == null)
			throw new KeyNotFoundException($"No se encontró el pedido con ID {request.PedidoId}");

		// Validar cliente
		var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.Codigo == request.ClienteCodigo, cancellationToken);
		if (cliente == null)
			throw new KeyNotFoundException($"El cliente con código {request.ClienteCodigo} no existe.");

		_db.DetallePedidos.RemoveRange(pedido.Detalles);

		decimal total = 0;

		foreach (var item in request.Items)
		{
			var producto = await _db.Productos.FirstOrDefaultAsync(p => p.Codigo == item.ProductoCodigo, cancellationToken);
			if (producto == null)
				throw new KeyNotFoundException($"El producto con código {item.ProductoCodigo} no existe.");

			var subtotal = producto.PrecioUnitario * item.Cantidad;
			total += subtotal;

			_db.DetallePedidos.Add(new DetallePedido
			{
				PedidoId = pedido.Id,
				ProductoCodigo = producto.Codigo,
				ProductoDescripcion = producto.Descripcion,
				Cantidad = item.Cantidad,
				PrecioUnitario = producto.PrecioUnitario,
				Subtotal = subtotal
			});
		}

		pedido.ClienteCodigo = request.ClienteCodigo;
		pedido.PrecioTotal = total;
		pedido.FechaOrden = DateTime.UtcNow;

		await _db.SaveChangesAsync(cancellationToken);

		return new UpdatePedidoResult
		{
			PedidoId = pedido.Id,
			Total = total,
			FechaActualizacion = pedido.FechaOrden
		};
	}
}