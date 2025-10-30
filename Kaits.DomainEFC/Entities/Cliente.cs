namespace Kaits.Domain.Entities
{
	public class Cliente
	{
		public int Id { get; set; }
		public string Codigo { get; set; } = null!;
		public string Nombre { get; set; } = null!;
		public string DNI { get; set; } = null!;
	}
}