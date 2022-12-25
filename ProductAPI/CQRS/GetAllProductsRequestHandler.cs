using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProductAPI.Data;
using ProductAPI.Models;

namespace ProductAPI.CQRS
{
    public class GetAllProductsRequestHandler : IRequestHandler<GetAllProductsRequest, List<ProductResponse>>
    {
        private readonly ProductDbContext _dbContext;
        private readonly IMemoryCache _memoryCache;

        public GetAllProductsRequestHandler(ProductDbContext dbContext, IMemoryCache memoryCache)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
        }

        public async Task<List<ProductResponse>> Handle(GetAllProductsRequest request, CancellationToken cancellationToken)
        {
            if (_memoryCache.TryGetValue("products", out List<ProductResponse> productResponses))
            {
                return productResponses;
            }

            var products = await _dbContext.Products.ToListAsync(cancellationToken);

            productResponses = products.Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price
            }).ToList();

            _memoryCache.Set("products", productResponses, TimeSpan.FromMinutes(1));

            return productResponses;
        }
    }
}
