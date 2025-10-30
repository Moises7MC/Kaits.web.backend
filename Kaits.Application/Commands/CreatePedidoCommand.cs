using MediatR;
public record CreatePedidoCommand(string ClienteCodigo, List<OrderItemDto> Items) : IRequest<CreatePedidoResult>;