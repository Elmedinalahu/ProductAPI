using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using ProductAPI.Data;
using ProductAPI.Models;
using StackExchange.Redis;

namespace ProductAPI.CQRS
{
    public class UpdateProductRequestHandler : IRequestHandler<UpdateProductRequest, ProductResponse>
    {
        private readonly ProductDbContext _dbContext;
        private readonly IMemoryCache _memoryCache;
        private readonly IConnectionMultiplexer _redis;

        public UpdateProductRequestHandler(ProductDbContext dbContext, IMemoryCache memoryCache, IConnectionMultiplexer redis)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
            _redis = redis;
        }

        public async Task<ProductResponse> Handle(UpdateProductRequest request, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products
                .Where(p => p.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken);

            if (product == null)
            {
                return null;
            }

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;

            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var productResponse = new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price
            };

            _memoryCache.Set(productResponse.Id, productResponse, TimeSpan.FromMinutes(1));

            var redisCache = _redis.GetDatabase();
            await redisCache.StringSetAsync(productResponse.Id.ToString(), JsonConvert.SerializeObject(productResponse), TimeSpan.FromMinutes(1));

            return productResponse;
        }
    }
}
