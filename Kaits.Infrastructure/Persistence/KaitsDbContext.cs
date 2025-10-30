using Microsoft.EntityFrameworkCore;
using Kaits.Domain.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Kaits.Infrastructure.Persistence
{
	public class KaitsDbContext : DbContext
	{
		public KaitsDbContext(DbContextOptions<KaitsDbContext> options) : base(options) { }

		public DbSet<Pedido> Pedidos { get; set; }
		public DbSet<DetallePedido> DetallePedidos { get; set; }
		public DbSet<Cliente> Clientes { get; set; }
		public DbSet<Producto> Productos { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Cliente>()
				.HasIndex(c => c.Codigo)
				.IsUnique();

			modelBuilder.Entity<Producto>()
				.HasIndex(p => p.Codigo)
				.IsUnique();

			modelBuilder.Entity<Pedido>()
				.HasMany(p => p.Detalles)
				.WithOne(d => d.Pedido)
				.HasForeignKey(d => d.PedidoId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Producto>().Property(p => p.PrecioUnitario).HasColumnType("decimal(18,2)");
			modelBuilder.Entity<DetallePedido>().Property(d => d.PrecioUnitario).HasColumnType("decimal(18,2)");
			modelBuilder.Entity<DetallePedido>().Property(d => d.Subtotal).HasColumnType("decimal(18,2)");
			modelBuilder.Entity<Pedido>().Property(p => p.PrecioTotal).HasColumnType("decimal(18,2)");

			base.OnModelCreating(modelBuilder);
		}
	}
}