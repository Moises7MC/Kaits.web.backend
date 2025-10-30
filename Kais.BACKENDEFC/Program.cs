using Kaits.Infrastructure.Persistence;
using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowReactApp", policy =>
	{
		policy.WithOrigins("http://localhost:5173")
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});

builder.Services.AddDbContext<KaitsDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("KaitsDatabase")));

builder.Services.AddMediatR(cfg =>
	cfg.RegisterServicesFromAssemblyContaining<CreatePedidoHandler>());

builder.Services.AddValidatorsFromAssemblyContaining<CreatePedidoValidator>();

// --- 🔹 Pipeline: ejecutar validaciones antes del handler ---
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();