using FluentValidation;
using MediatR;

public class CreateClienteCommand : IRequest<int>
{
	public string Codigo { get; set; } = string.Empty;
	public string Nombre { get; set; } = string.Empty;
	public string DNI { get; set; } = string.Empty;
}

public class CreateClienteCommandValidator : AbstractValidator<CreateClienteCommand>
{
	public CreateClienteCommandValidator()
	{
		RuleFor(x => x.Codigo)
			.NotEmpty().WithMessage("El código del cliente es obligatorio.")
			.MaximumLength(20).WithMessage("El código no debe exceder los 20 caracteres.");

		RuleFor(x => x.Nombre)
			.NotEmpty().WithMessage("El nombre del cliente es obligatorio.");

		RuleFor(x => x.DNI)
			.NotEmpty().WithMessage("El DNI es obligatorio.")
			.Length(8).WithMessage("El DNI debe tener exactamente 8 dígitos.");
	}
}