using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using ProductAPI.Data;
using ProductAPI.Models;
using StackExchange.Redis;

namespace ProductAPI.CQRS
{
    public class CreateProductRequestHandler : IRequestHandler<CreateProductRequest, ProductResponse>
    {
        private readonly ProductDbContext _dbContext;
        private readonly IMemoryCache _memoryCache;
        private readonly IConnectionMultiplexer _redis;

        public CreateProductRequestHandler(ProductDbContext dbContext, IMemoryCache memoryCache, IConnectionMultiplexer redis)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
            _redis = redis;
        }

        public async Task<ProductResponse> Handle(CreateProductRequest request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price
            };

            _dbContext.Products.Add(product);
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
