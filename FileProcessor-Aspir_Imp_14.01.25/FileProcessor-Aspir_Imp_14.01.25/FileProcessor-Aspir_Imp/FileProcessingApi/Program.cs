using Commerce_Connector_Data.CommerceConnectorServices.IService;
using Commerce_Connector_Data.CommerceConnectorServices.Service;
using Commerce_Connector_Data.FileProcessingEntities;
using Commerce_Connector_Factory.Factories;
using CsvHelper;
using FileProcessingApi.AutoMapperConfig;
using FileProcessingApi.Consumers;
using FileProcessingApi.Controllers;

using FileProcessingApi.Models;
using FileProcessingApi.ZnodeImport;
using Hangfire;
using MassTransit;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Configure CORS to allow any origin, method, and header
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add AutoMapper and register the FileProcessingProfile
builder.Services.AddAutoMapper(typeof(FileProcessingProfile));

// Add other necessary services
builder.Services.AddControllers();
builder.Services.AddScoped<FileProcessingController>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<FileProcessingConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var host = configuration.GetConnectionString("RabbitMQ");
        cfg.Host(host);
        cfg.ReceiveEndpoint("file-processing-queue", e =>
        {
            e.ConfigureConsumer<FileProcessingConsumer>(context);
        });
    });
});

builder.Services.AddHttpClient(); // Register HttpClientFactory
 // Update with your implementation
builder.Services.AddSingleton<IFileLocationHandlerFactory, FileLocationHandlerFactory>(); // Update with your implementation
builder.Services.AddScoped<IZnodeImport, ZnodeImport>();
builder.Services.AddControllers(); builder.Services.AddScoped<IFileDetailsService, FileDetailsService>();

builder.Services.AddHangfire((sp, config) =>
{
    var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("DbConnection");
    config.UseSqlServerStorage(connectionString);
});
builder.Services.AddHangfireServer();

//Register your DbContext using the connection string
builder.Services.AddDbContext<FileProcessing_Entities>(options =>
   options.UseSqlServer("Server = ACL - 228\\SQL2022; Initial Catalog = FileProcessing_Database; Persist Security Info=False; User ID = sa; Password = Commerce#765;MultipleActiveResultSets=False;Encrypt=false;TrustServerCertificate=True;Connection Timeout=30;"));

// Add services for Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



app.UseHangfireDashboard("/hangfire");
//using (var scope = app.Services.CreateScope())
//{
//    var controller = scope.ServiceProvider.GetRequiredService<FileProcessingController>();
//    // Triggering CreateRecurringJob method from controller
//    controller.CreateRecurringJob();
//}

// Use CORS policy
app.UseCors("AllowAll");

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
