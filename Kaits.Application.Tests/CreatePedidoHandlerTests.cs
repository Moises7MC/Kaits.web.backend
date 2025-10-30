namespace Kaits.Application.Tests;

using Kaits.Domain.Entities;
using Kaits.Infrastructure.Persistence;
using Kaits.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class CreatePedidoHandlerTests
{
	[Fact]
	public async Task Handle_When_ItemQuantityIsZero_ThrowsValidationException()
	{
		var options = new DbContextOptionsBuilder<KaitsDbContext>()
			.UseInMemoryDatabase(databaseName: "TestDb")
			.Options;

		using var db = new KaitsDbContext(options);
		db.Clientes.Add(new Cliente { Codigo = "C001", Nombre = "X", DNI = "11111111" });
		db.Productos.Add(new Producto { Codigo = "P001", Descripcion = "X", PrecioUnitario = 10m });
		await db.SaveChangesAsync();

		var handler = new CreatePedidoHandler(db, null);
		var command = new CreatePedidoCommand(
			"C001",
			new List<OrderItemDto> { new OrderItemDto("P001", 0) }
		);

		await Assert.ThrowsAsync<BusinessValidationException>(() =>
			handler.Handle(command, CancellationToken.None));
	}
}