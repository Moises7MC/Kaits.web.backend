using FluentValidation;
using MediatR;

public class UpdateClienteCommand : IRequest<Unit>
{
	public int Id { get; set; }
	public string Codigo { get; set; } = string.Empty;
	public string Nombre { get; set; } = string.Empty;
	public string DNI { get; set; } = string.Empty;
}

public class UpdateClienteCommandValidator : AbstractValidator<UpdateClienteCommand>
{
	public UpdateClienteCommandValidator()
	{
		RuleFor(x => x.Codigo)
			.NotEmpty().WithMessage("El código del cliente es obligatorio.");

		RuleFor(x => x.Nombre)
			.NotEmpty().WithMessage("El nombre del cliente es obligatorio.");

		RuleFor(x => x.DNI)
			.NotEmpty().WithMessage("El DNI es obligatorio.")
			.Length(8).WithMessage("El DNI debe tener exactamente 8 dígitos.");
	}
}