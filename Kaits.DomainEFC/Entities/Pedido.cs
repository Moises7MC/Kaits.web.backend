namespace Kaits.Domain.Entities
{
	public class Pedido
	{
		public int Id { get; set; }
		public DateTime FechaOrden { get; set; } = DateTime.UtcNow;
		public string ClienteCodigo { get; set; } = null!;
		public decimal PrecioTotal { get; set; }
		public List<DetallePedido> Detalles { get; set; } = new();
	}
}