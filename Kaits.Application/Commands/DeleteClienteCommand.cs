using MediatR;

public class DeleteClienteCommand : IRequest<Unit>
{
	public int Id { get; set; }
}