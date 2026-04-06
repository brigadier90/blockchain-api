using System.Reflection;
using BlockchainApi.Api.Application.Clients;
using BlockchainApi.Api.Domain;
using BlockchainApi.Api.Domain.Repositories;
using BlockchainApi.Api.Infrastructure.Clients;
using BlockchainApi.Api.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});
builder.Services.AddSingleton<IBlockCypherRepository, BlockCypherRepository>();
builder.Services.AddHttpClient<IBlockCypherClient, BlockCypherClient>()
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri("https://api.blockcypher.com/v1/");
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
