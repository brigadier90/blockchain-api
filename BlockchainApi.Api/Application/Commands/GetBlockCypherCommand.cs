using BlockchainApi.Api.Application.Common;
using BlockchainApi.Api.Domain;
using BlockchainApi.Api.Domain.Models;
using MediatR;

namespace BlockchainApi.Api.Application.Commands;

public record GetBlockCypherCommand(string Coin) : IRequest<Result<BlockCypher>>;

public class GetBlockCypherCommandHandler : IRequestHandler<GetBlockCypherCommand, Result<BlockCypher>>
{
    private IBlockCypherRepository _repository;
    private static readonly HashSet<string> ValidCoins = new(StringComparer.OrdinalIgnoreCase)
    {
        "btc", "eth", "ltc", "dash"
    };

    public GetBlockCypherCommandHandler(IBlockCypherRepository repository)
    {
        _repository = repository;
    }

    public Task<Result<BlockCypher>> Handle(GetBlockCypherCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!ValidCoins.Contains(request.Coin))
                return Task.FromResult(
                    Result<BlockCypher>.Failure(
                        $"Invalid coin: {request.Coin}. Valid coins are: {string.Join(", ", ValidCoins)}", 
                        StatusCodes.Status400BadRequest)
                    );

            using var http = new HttpClient();

            var response = http.GetAsync($"https://api.blockcypher.com/v1/{request.Coin}/main").Result;
            var result = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
                return Task.FromResult(Result<BlockCypher>.Failure($"Error fetching data from BlockCypher API: {result}", (int)response.StatusCode));

            var record = BlockCypher.FromJson(request.Coin, result);

            _repository.Save(record);
            return Task.FromResult(Result<BlockCypher>.Success(record));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<BlockCypher>.Failure($"Error fetching data from BlockCypher API: {ex.Message}", StatusCodes.Status500InternalServerError));
        }
     }
}
