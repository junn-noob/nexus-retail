using Microsoft.EntityFrameworkCore;
using NexusRetail.Core.Api.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<NexusRetail.Core.Api.Services.Export.IExcelExportService, NexusRetail.Core.Api.Services.Export.ExcelExportService>();
builder.Services.AddScoped<NexusRetail.Core.Api.Services.Chat.IGeminiChatService, NexusRetail.Core.Api.Services.Chat.GeminiChatService>();
builder.Services.AddSingleton<NexusRetail.Core.Api.Services.Payment.IBankPaymentService, NexusRetail.Core.Api.Services.Payment.BankPaymentService>();
builder.Services.AddDbContext<RetailDbContext>(x =>
    x.UseSqlServer(builder.Configuration.GetConnectionString("RetailDb")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();
