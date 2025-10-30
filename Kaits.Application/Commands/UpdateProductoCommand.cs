public class UpdateProductoCommand
{
	public string Codigo { get; set; } = string.Empty;
	public string Descripcion { get; set; } = string.Empty;
	public decimal PrecioUnitario { get; set; }
}
