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

var coins = new List<string> { "btc", "eth", "ltc", "dash" };

var history = new Dictionary<string, List<string>>{};

app.MapGet("/api/blockcypher/v1/{coin}/main", (string coin) =>
{
    HttpClient _http = new HttpClient();

    var response = _http.GetAsync($"https://api.blockcypher.com/v1/{coin}/main").Result;
    var result = response.Content.ReadAsStringAsync().Result;

    if (!history.ContainsKey(coin))
        history[coin] = new List<string>();
    
    history[coin].Add(result);
    
    return response.Content.ReadAsStringAsync().Result;
})
.WithName("GetCoinBlockCypher")
.WithDescription("Returns the latest block information from BlockCypher API for the specified coin.");

app.MapGet("/api/blockcypher/v1/{coin}/history", (string coin) =>
{
    return string.Empty;
})
.WithName("GetCoinBlockCypherHistory")
.WithDescription("Returns the history of block information requested for the specified coin.");


app.Run();
