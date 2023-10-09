using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using Test_WeatherApp_Server.Context;
using Test_WeatherApp_Server.Interfaces;
using Test_WeatherApp_Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add database connection

builder.Services
    .AddDbContext<WeatherForecastDbContext>(options => 
        options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));

// Add services to the container.

builder.Services.AddTransient<IWeatherForecastFileHandler, WeatherForecastFileHandler>();
builder.Services.AddTransient<IWeatherForecastService, WeatherForecastService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => options.AddPolicy(name: "Frontend",
    policy =>
    {
        policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
    }
    ));

var app = builder.Build();

// This part of code is used for automatic migrations and creation of DataBase 
// Usualy it will be better to do it with commands
using (var scope = app.Services.CreateScope())
{
    using var context = scope.ServiceProvider.GetService<WeatherForecastDbContext>();
    context?.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.MapControllers();

app.Run();
