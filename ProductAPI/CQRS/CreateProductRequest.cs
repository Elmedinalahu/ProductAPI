using MediatR;
using ProductAPI.Models;

namespace ProductAPI.CQRS
{
    public class CreateProductRequest : IRequest<ProductResponse>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
