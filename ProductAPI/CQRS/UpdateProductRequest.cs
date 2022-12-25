using MediatR;
using ProductAPI.Models;

namespace ProductAPI.CQRS
{
    public class UpdateProductRequest : IRequest<ProductResponse>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
