var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.AspireSample_ApiService>("apiservice");

var daRestAPI = builder.AddProject<Projects.DA_RestAPI>("DA_RestAPI");
var daGrpcService = builder.AddProject<Projects.DA_GrpcService>("DA_GrpcService");


builder.AddProject<Projects.AspireSample_Web>("webfrontend")
    .WithReference(cache)
    .WithReference(apiService);




builder.Build().Run();
