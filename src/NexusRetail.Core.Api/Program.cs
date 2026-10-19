using Microsoft.EntityFrameworkCore;
using NexusRetail.Core.Api.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<RetailDbContext>(x =>
    x.UseSqlServer(builder.Configuration.GetConnectionString("RetailDb")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapControllers(); 

app.Run();
