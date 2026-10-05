//using Commerce_Connector_Data.CommerceConnectorServices.IService;
//using Commerce_Connector_Data.CommerceConnectorServices.Service;

using Commerce_Connector_BAL.Agent;
using Commerce_Connector_BAL.IAgent;
using Commerce_Connector_Factory.Factories;
using TransformTableData;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
//builder.Services.AddScoped<IFileDetailsService, FileDetailsService>();


// Add services to the container.
builder.Services.AddControllersWithViews();
// Register HttpClient
builder.Services.AddHttpClient("APIHandler", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://localhost:7185/");
}); // This line registers HttpClient for dependency injection
builder.Services.AddScoped<IFileProcessingAgent, FileProcessingAgent>();
builder.Services.AddScoped<IMappingAgent, MappingAgent>();
builder.Services.AddScoped<IJobsAgent, JobsAgent>();
builder.Services.AddScoped<ITemplateAgent, TemplateAgent>();
builder.Services.AddSingleton<IFileProcessorFactory, FileProcessorFactory>(); // Update with your implementation
builder.Services.AddSingleton<IFileLocationHandlerFactory, FileLocationHandlerFactory>(); // Update with your implementation
builder.Services.AddSingleton<IExpressionEvaluator, ExpressionEvaluator>();

var app = builder.Build();

app.MapDefaultEndpoints();

//app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
