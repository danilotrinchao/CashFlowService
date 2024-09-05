using CashierService.Application;
using CashierService.Application.CommandHandlers;
using CashierService.Core.Interfaces;
using CashierService.Infra.Repositories;
using System.Data;
using Npgsql;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddScoped<IDbConnection>(provider =>
    new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateTransactionCommandHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ConsolidateDailyBalanceCommandHandler).Assembly));
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IDailySummaryRepository, DailySummaryRepository>();

builder.Services.AddScoped<ICashFlowService, CashFlowService>();
builder.Services.AddMemoryCache();
builder.Services.AddOutputCache();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var defaultCulture = new CultureInfo("pt-BR");
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(defaultCulture),
    SupportedCultures = new List<CultureInfo> { defaultCulture },
    SupportedUICultures = new List<CultureInfo> { defaultCulture }
};
var app = builder.Build();

// Configuração do pipeline de requisição
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseOutputCache();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
