var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/api/blockcypher/v1/{coin}/main", (string coin) =>
{
    HttpClient _http = new HttpClient();

    var response = _http.GetAsync($"https://api.blockcypher.com/v1/{coin}/main").Result;
    return response.Content.ReadAsStringAsync().Result;
})
.WithName("GetCoinBlockCypher")
.WithDescription("Returns the latest block information from BlockCypher API for the specified coin.");

app.Run();
