var builder = DistributedApplication.CreateBuilder(args);


var messaging = builder.AddRabbitMQ("RabbitMQ")
    .WithManagementPlugin();
var apiservice = builder.AddProject<Projects.FileProcessingApi>("fileprocessingapi")
    .WithReference(messaging);

builder.AddProject<Projects.CommerceConnector>("commerceconnector")
    .WithReference(apiservice);

builder.Build().Run();
