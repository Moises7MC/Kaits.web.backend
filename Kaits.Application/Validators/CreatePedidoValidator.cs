using FluentValidation;

public class CreatePedidoValidator : AbstractValidator<CreatePedidoCommand>
{
	public CreatePedidoValidator()
	{
		RuleFor(x => x.ClienteCodigo).NotEmpty().WithMessage("ClienteCodigo es requerido.");
		RuleFor(x => x.Items)
			.NotNull().WithMessage("Items no puede ser nulo.")
			.Must(i => i.Any()).WithMessage("Debe haber al menos un item en el pedido.");

		RuleForEach(x => x.Items).SetValidator(new OrderItemDtoValidator());
	}
}

public class OrderItemDtoValidator : AbstractValidator<OrderItemDto>
{
	public OrderItemDtoValidator()
	{
		RuleFor(x => x.ProductoCodigo).NotEmpty().WithMessage("ProductoCodigo es requerido.");
		RuleFor(x => x.Cantidad)
			.GreaterThan(0).WithMessage("Cantidad debe ser mayor a 0.");
	}
}
