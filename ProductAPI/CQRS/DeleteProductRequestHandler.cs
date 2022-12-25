using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProductAPI.Data;
using StackExchange.Redis;

namespace ProductAPI.CQRS
{
    public class DeleteProductRequestHandler : IRequestHandler<DeleteProductRequest>
    {
        private readonly ProductDbContext _dbContext;
        private readonly IMemoryCache _memoryCache;
        private readonly IConnectionMultiplexer _redis;

        public DeleteProductRequestHandler(ProductDbContext dbContext, IMemoryCache memoryCache, IConnectionMultiplexer redis)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
            _redis = redis;
        }

        public async Task<Unit> Handle(DeleteProductRequest request, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products
                .Where(p => p.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken);

            if (product == null)
            {
                return Unit.Value;
            }

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _memoryCache.Remove(product.Id);

            var redisCache = _redis.GetDatabase();
            await redisCache.KeyDeleteAsync(product.Id.ToString());

            return Unit.Value;
        }
    }
}
