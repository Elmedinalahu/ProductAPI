using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using ProductAPI.CQRS;
using ProductAPI.Data;
using ProductAPI.Models;
using StackExchange.Redis;
using System.Text;

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMemoryCache _memoryCache;
        private readonly IConnectionMultiplexer _redis;

        public ProductsController(IMediator mediator, IMemoryCache memoryCache, IConnectionMultiplexer redis)
        {
            _mediator = mediator;
            _memoryCache = memoryCache;
            _redis = redis;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = await _mediator.Send(new GetAllProductsRequest());
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var product = await _mediator.Send(new GetProductRequest { Id = id });
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateProductRequest request)
        {
            var product = await _mediator.Send(request);
            return Ok("Product created successfully");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] UpdateProductRequest request)
        {
            request.Id = id;
            var product = await _mediator.Send(request);
            return Ok("Product updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteProductRequest { Id = id });
            return Ok("Product deleted successfully");
        }
    }
}
