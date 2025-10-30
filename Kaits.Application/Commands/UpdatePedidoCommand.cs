using MediatR;

public class UpdatePedidoCommand : IRequest<UpdatePedidoResult>
{
	public int PedidoId { get; set; }
	public string ClienteCodigo { get; set; } = string.Empty;
	public List<UpdatePedidoItemDto> Items { get; set; } = new();
}

public class UpdatePedidoItemDto
{
	public string ProductoCodigo { get; set; } = string.Empty;
	public int Cantidad { get; set; }
}

public class UpdatePedidoResult
{
	public int PedidoId { get; set; }
	public decimal Total { get; set; }
	public DateTime FechaActualizacion { get; set; }
}