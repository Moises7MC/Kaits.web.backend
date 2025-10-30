using Kaits.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class DeleteClienteHandler : IRequestHandler<DeleteClienteCommand, Unit>
{
	private readonly KaitsDbContext _db;

	public DeleteClienteHandler(KaitsDbContext db)
	{
		_db = db;
	}

	public async Task<Unit> Handle(DeleteClienteCommand request, CancellationToken cancellationToken)
	{
		var cliente = await _db.Clientes
			.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

		if (cliente == null)
			throw new KeyNotFoundException($"No se encontró el cliente con Id {request.Id}.");

		bool tienePedidos = await _db.Pedidos
			.AnyAsync(p => p.ClienteCodigo == cliente.Codigo, cancellationToken);

		if (tienePedidos)
			throw new InvalidOperationException($"No se puede eliminar el cliente '{cliente.Nombre}' porque tiene pedidos registrados.");

		_db.Clientes.Remove(cliente);
		await _db.SaveChangesAsync(cancellationToken);

		return Unit.Value;
	}
}