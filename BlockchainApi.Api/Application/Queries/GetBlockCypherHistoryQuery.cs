using BlockchainApi.Api.Application.Common;
using BlockchainApi.Api.Domain.Repositories;
using BlockchainApi.Api.Dtos;
using MediatR;

namespace BlockchainApi.Api.Application.Queries;

public record GetBlockCypherHistoryQuery(string Coin) : IRequest<Result<List<BlockcypherSnapshotDto>>>;

public class GetBlockCypherHistoryQueryHandler : IRequestHandler<GetBlockCypherHistoryQuery, Result<List<BlockcypherSnapshotDto>>>
{
    private IBlockCypherRepository _repository;

    public GetBlockCypherHistoryQueryHandler(IBlockCypherRepository repository)
    {
        _repository = repository;
    }

    public Task<Result<List<BlockcypherSnapshotDto>>> Handle(GetBlockCypherHistoryQuery request, CancellationToken cancellationToken)
    {
        if (!Constants.ValidCoins.Contains(request.Coin))
            return Task.FromResult(
                Result<List<BlockcypherSnapshotDto>>.Failure(
                    $"Invalid coin: {request.Coin}. Valid coins are: {string.Join(", ", Constants.ValidCoins)}",
                    StatusCodes.Status400BadRequest)
                );

        if (!_repository.TryGetAllFor(request.Coin, out var history) || history.Count == 0)
            return Task.FromResult(
                Result<List<BlockcypherSnapshotDto>>.Failure(
                    $"No history found for coin: {request.Coin}",
                    StatusCodes.Status404NotFound)
                );

        var snapshotDtos = history
            .OrderByDescending(snapshot => snapshot.CreatedAt)
            .Select(BlockcypherSnapshotDto.FromRecord)
            .ToList();

        return Task.FromResult(Result<List<BlockcypherSnapshotDto>>.Success(snapshotDtos));
    }
}