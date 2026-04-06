using BlockchainApi.Api.Application.Clients;
using BlockchainApi.Api.Application.Common;
using BlockchainApi.Api.Domain.Repositories;
using BlockchainApi.Api.Domain.Models;
using MediatR;

namespace BlockchainApi.Api.Application.Commands;

public record GetBlockCypherCommand(string Coin) : IRequest<Result<BlockcypherSnapshotDto>>;

public class GetBlockCypherCommandHandler : IRequestHandler<GetBlockCypherCommand, Result<BlockcypherSnapshotDto>>
{
    private readonly IBlockCypherClient _client;
    private readonly IBlockCypherRepository _repository;
    
    public GetBlockCypherCommandHandler(IBlockCypherClient client, IBlockCypherRepository repository)
    {
        _client = client;
        _repository = repository;
    }

    public async Task<Result<BlockcypherSnapshotDto>> Handle(GetBlockCypherCommand request, CancellationToken cancellationToken)
    {
        if (!Constants.ValidCoins.Contains(request.Coin))
            return Result<BlockcypherSnapshotDto>.Failure(
                $"Invalid coin: {request.Coin}. Valid coins are: {string.Join(", ", Constants.ValidCoins)}",
                StatusCodes.Status400BadRequest);

        var response = await _client.FetchAsync(request.Coin, cancellationToken);

        if (!response.IsSuccessStatusCode)
            return Result<BlockcypherSnapshotDto>.Failure(
                $"Error fetching data from BlockCypher API: {response.Content}",
                response.StatusCode);

        var record = BlockCypher.FromJson(request.Coin, response.Content);
        var dto = BlockcypherSnapshotDto.FromRecord(record);

        _repository.Save(record);
        return Result<BlockcypherSnapshotDto>.Success(dto);
    }
}
