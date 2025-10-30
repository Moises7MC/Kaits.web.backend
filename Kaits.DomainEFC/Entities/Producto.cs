namespace Kaits.Domain.Entities
{
	public class Producto
	{
		public int Id { get; set; }
		public string Codigo { get; set; } = null!;
		public string Descripcion { get; set; } = null!;
		public decimal PrecioUnitario { get; set; }
	}
}