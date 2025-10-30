using Kaits.Domain.Entities;
using Kaits.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class CreateClienteHandler : IRequestHandler<CreateClienteCommand, int>
{
	private readonly KaitsDbContext _db;

	public CreateClienteHandler(KaitsDbContext db)
	{
		_db = db;
	}

	public async Task<int> Handle(CreateClienteCommand request, CancellationToken cancellationToken)
	{
		var codigoExistente = await _db.Clientes
			.AnyAsync(c => c.Codigo == request.Codigo, cancellationToken);
		if (codigoExistente)
			throw new InvalidOperationException($"El código '{request.Codigo}' ya está registrado.");

		var dniExistente = await _db.Clientes
			.AnyAsync(c => c.DNI == request.DNI, cancellationToken);
		if (dniExistente)
			throw new InvalidOperationException($"El DNI '{request.DNI}' ya está registrado.");

		// Crear nuevo cliente
		var cliente = new Cliente
		{
			Codigo = request.Codigo,
			Nombre = request.Nombre,
			DNI = request.DNI
		};

		_db.Clientes.Add(cliente);
		await _db.SaveChangesAsync(cancellationToken);

		return cliente.Id; 
	}
}