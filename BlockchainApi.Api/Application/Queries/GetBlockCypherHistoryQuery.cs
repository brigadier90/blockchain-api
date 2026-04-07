using BlockchainApi.Api.Application.Common;
using BlockchainApi.Api.Domain.Repositories;
using BlockchainApi.Api.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlockchainApi.Api.Application.Queries;

public record GetBlockCypherHistoryQuery(string Coin, int? Page = null, int? PageSize = null) : IRequest<Result<List<BlockcypherSnapshotDto>>>;

public class GetBlockCypherHistoryQueryHandler : IRequestHandler<GetBlockCypherHistoryQuery, Result<List<BlockcypherSnapshotDto>>>
{
    private readonly IBlockCypherRepository _repository;
    private readonly ILogger<GetBlockCypherHistoryQueryHandler> _logger;

    public GetBlockCypherHistoryQueryHandler(IBlockCypherRepository repository, ILogger<GetBlockCypherHistoryQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<List<BlockcypherSnapshotDto>>> Handle(GetBlockCypherHistoryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetBlockCypherHistoryQuery for coin {Coin}", request.Coin);
        if (!Constants.ValidCoins.Contains(request.Coin))
        {
            _logger.LogWarning("Invalid history request for coin {Coin}", request.Coin);
            return Result<List<BlockcypherSnapshotDto>>.Failure(
                $"Invalid coin: {request.Coin}. Valid coins are: {string.Join(", ", Constants.ValidCoins)}",
                StatusCodes.Status400BadRequest);
        }

        if ((request.Page.HasValue && request.Page < 1) || (request.PageSize.HasValue && request.PageSize < 1))
        {
            _logger.LogWarning("Invalid pagination parameters for coin {Coin}: page={Page}, pageSize={PageSize}", request.Coin, request.Page, request.PageSize);
            return Result<List<BlockcypherSnapshotDto>>.Failure(
                "Invalid pagination parameters. Page and pageSize must be greater than 0.",
                StatusCodes.Status400BadRequest);
        }

        var history = await _repository.GetAllAsync(request.Coin, request.Page, request.PageSize);
        _logger.LogInformation("Retrieved {Count} history records for coin {Coin} page {Page} pageSize {PageSize}", history.Count, request.Coin, request.Page, request.PageSize);

        var snapshotDtos = history
            .Select(BlockcypherSnapshotDto.FromRecord)
            .ToList();

        return Result<List<BlockcypherSnapshotDto>>.Success(snapshotDtos);
    }
}