using FluentValidation;

public class UpdatePedidoCommandValidator : AbstractValidator<UpdatePedidoCommand>
{
	public UpdatePedidoCommandValidator()
	{
		RuleFor(x => x.PedidoId)
			.GreaterThan(0)
			.WithMessage("El ID del pedido debe ser mayor que cero.");

		RuleFor(x => x.ClienteCodigo)
			.NotEmpty()
			.WithMessage("El código del cliente es obligatorio.")
			.MaximumLength(10)
			.WithMessage("El código del cliente no puede tener más de 10 caracteres.");

		RuleFor(x => x.Items)
			.NotEmpty()
			.WithMessage("Debe incluir al menos un producto en el pedido.");

		RuleForEach(x => x.Items).ChildRules(item =>
		{
			item.RuleFor(i => i.ProductoCodigo)
				.NotEmpty()
				.WithMessage("El código del producto es obligatorio.")
				.MaximumLength(10)
				.WithMessage("El código del producto no puede tener más de 10 caracteres.");

			item.RuleFor(i => i.Cantidad)
				.GreaterThan(0)
				.WithMessage("La cantidad debe ser mayor que cero.");
		});
	}
}