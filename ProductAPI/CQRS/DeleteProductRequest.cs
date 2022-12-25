using MediatR;

namespace ProductAPI.CQRS
{
    public class DeleteProductRequest : IRequest
    {
        public int Id { get; set; }
    }
}
