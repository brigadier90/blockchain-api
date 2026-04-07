using BlockchainApi.Api.Application.Common;
using BlockchainApi.Api.Domain.Repositories;
using BlockchainApi.Api.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlockchainApi.Api.Application.Queries;

public record GetBlockCypherHistoryQuery(string Coin) : IRequest<Result<List<BlockcypherSnapshotDto>>>;

public class GetBlockCypherHistoryQueryHandler : IRequestHandler<GetBlockCypherHistoryQuery, Result<List<BlockcypherSnapshotDto>>>
{
    private readonly IBlockCypherRepository _repository;
    private readonly ILogger<GetBlockCypherHistoryQueryHandler> _logger;

    public GetBlockCypherHistoryQueryHandler(IBlockCypherRepository repository, ILogger<GetBlockCypherHistoryQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public Task<Result<List<BlockcypherSnapshotDto>>> Handle(GetBlockCypherHistoryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetBlockCypherHistoryQuery for coin {Coin}", request.Coin);
        if (!Constants.ValidCoins.Contains(request.Coin))
        {
            _logger.LogWarning("Invalid history request for coin {Coin}", request.Coin);
            return Task.FromResult(
                Result<List<BlockcypherSnapshotDto>>.Failure(
                    $"Invalid coin: {request.Coin}. Valid coins are: {string.Join(", ", Constants.ValidCoins)}",
                    StatusCodes.Status400BadRequest)
                );
        }

        if (!_repository.TryGetAllFor(request.Coin, out var history) || history.Count == 0)
        {
            _logger.LogWarning("No history found for coin {Coin}", request.Coin);
            return Task.FromResult(
                Result<List<BlockcypherSnapshotDto>>.Failure(
                    $"No history found for coin: {request.Coin}",
                    StatusCodes.Status404NotFound)
                );
        }

        _logger.LogInformation("Retrieved {Count} history records for coin {Coin}", history.Count, request.Coin);
        var snapshotDtos = history
            .OrderByDescending(snapshot => snapshot.CreatedAt)
            .Select(BlockcypherSnapshotDto.FromRecord)
            .ToList();

        return Task.FromResult(Result<List<BlockcypherSnapshotDto>>.Success(snapshotDtos));
    }
}