using MediatR;
using ProductAPI.Models;

namespace ProductAPI.CQRS
{
    public class GetProductRequest : IRequest<ProductResponse>
    {
        public int Id { get; set; }
    }
}
