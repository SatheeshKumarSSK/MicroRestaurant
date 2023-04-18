using Micro.Services.EmailAPI.Data;
using Micro.Services.EmailAPI.Extensions;
using Micro.Services.EmailAPI.Interfaces;
using Micro.Services.EmailAPI.Messaging;
using Micro.Services.EmailAPI.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DataContext>(options =>
              options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IEmailRepository, EmailRepository>();

var optionBuilder = new DbContextOptionsBuilder<DataContext>();
optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddSingleton(new EmailRepository(optionBuilder.Options));

builder.Services.AddSingleton<IAzureServiceBusConsumerEmail, AzureServiceBusConsumerEmail>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseAzureServiceBusConsumer();

app.Run();
