using BlockchainApi.Api.Application.Common;
using BlockchainApi.Api.Domain;
using BlockchainApi.Api.Domain.Models;
using MediatR;

namespace BlockchainApi.Api.Application.Commands;

public record GetBlockCypherCommand(string Coin) : IRequest<Result<BlockcypherSnapshotDto>>;

public class GetBlockCypherCommandHandler : IRequestHandler<GetBlockCypherCommand, Result<BlockcypherSnapshotDto>>
{
    private IBlockCypherRepository _repository;
    
    public GetBlockCypherCommandHandler(IBlockCypherRepository repository)
    {
        _repository = repository;
    }

    public Task<Result<BlockcypherSnapshotDto>> Handle(GetBlockCypherCommand request, CancellationToken cancellationToken)
    {

        if (!Constants.ValidCoins.Contains(request.Coin))
            return Task.FromResult(
                Result<BlockcypherSnapshotDto>.Failure(
                    $"Invalid coin: {request.Coin}. Valid coins are: {string.Join(", ", Constants.ValidCoins)}",
                    StatusCodes.Status400BadRequest)
                );

        using var http = new HttpClient();

        var response = http.GetAsync($"https://api.blockcypher.com/v1/{request.Coin}/main").Result;
        var result = response.Content.ReadAsStringAsync().Result;
        if (!response.IsSuccessStatusCode)
            return Task.FromResult(
                Result<BlockcypherSnapshotDto>.Failure($"Error fetching data from BlockCypher API: {result}",
                (int)response.StatusCode)
            );

        var record = BlockCypher.FromJson(request.Coin, result);
        var dto = BlockcypherSnapshotDto.FromRecord(record);

        _repository.Save(record);
        return Task.FromResult(Result<BlockcypherSnapshotDto>.Success(dto));
    }
}
