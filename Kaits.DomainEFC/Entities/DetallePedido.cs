namespace Kaits.Domain.Entities
{
	public class DetallePedido
	{
		public int Id { get; set; }
		public int PedidoId { get; set; }
		public Pedido Pedido { get; set; } = null!;
		public string ProductoCodigo { get; set; } = null!;
		public string ProductoDescripcion { get; set; } = null!;
		public int Cantidad { get; set; }
		public decimal PrecioUnitario { get; set; }
		public decimal Subtotal { get; set; }
	}
}