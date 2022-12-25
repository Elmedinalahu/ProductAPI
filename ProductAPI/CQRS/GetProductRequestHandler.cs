using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProductAPI.Data;
using ProductAPI.Models;

namespace ProductAPI.CQRS
{
    public class GetProductRequestHandler : IRequestHandler<GetProductRequest, ProductResponse>
    {
        private readonly ProductDbContext _dbContext;
        private readonly IMemoryCache _memoryCache;

        public GetProductRequestHandler(ProductDbContext dbContext, IMemoryCache memoryCache)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
        }

        public async Task<ProductResponse> Handle(GetProductRequest request, CancellationToken cancellationToken)
        {
            if (_memoryCache.TryGetValue(request.Id, out ProductResponse productResponse))
            {
                return productResponse;
            }

            var product = await _dbContext.Products
                .Where(p => p.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken);

            if (product == null)
            {
                return null;
            }

            productResponse = new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price
            };

            _memoryCache.Set(productResponse.Id, productResponse, TimeSpan.FromMinutes(1));

            return productResponse;
        }
    }
}
