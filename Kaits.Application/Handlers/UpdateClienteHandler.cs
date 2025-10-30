using Kaits.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class UpdateClienteHandler : IRequestHandler<UpdateClienteCommand, Unit>
{
	private readonly KaitsDbContext _db;

	public UpdateClienteHandler(KaitsDbContext db)
	{
		_db = db;
	}

	public async Task<Unit> Handle(UpdateClienteCommand request, CancellationToken cancellationToken)
	{
		var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

		if (cliente == null)
			throw new KeyNotFoundException($"No se encontró el cliente con Id {request.Id}.");

		var codigoDuplicado = await _db.Clientes
			.AnyAsync(c => c.Codigo == request.Codigo && c.Id != request.Id, cancellationToken);
		if (codigoDuplicado)
			throw new InvalidOperationException($"El código '{request.Codigo}' ya está registrado por otro cliente.");

		var dniDuplicado = await _db.Clientes
			.AnyAsync(c => c.DNI == request.DNI && c.Id != request.Id, cancellationToken);
		if (dniDuplicado)
			throw new InvalidOperationException($"El DNI '{request.DNI}' ya está registrado por otro cliente.");

		cliente.Codigo = request.Codigo;
		cliente.Nombre = request.Nombre;
		cliente.DNI = request.DNI;

		await _db.SaveChangesAsync(cancellationToken);

		return Unit.Value;
	}
}