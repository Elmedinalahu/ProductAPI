using MediatR;
using ProductAPI.Models;

namespace ProductAPI.CQRS
{
    public class GetAllProductsRequest : IRequest<List<ProductResponse>>
    {
        public int Id { get; set; }
    }
}
