using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductAPI.CQRS;
using ProductAPI.Data;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mp = ConnectionMultiplexer.Connect("localhost");
builder.Services.AddSingleton<IConnectionMultiplexer>(mp);

builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlite("Data Source=ProductAPITest.sqlite3"));

//builder.Services.AddDbContext<ProductDbContext>(options =>
//    options.UseInMemoryDatabase(databaseName: "InMemoryProduct"));

builder.Services.AddMemoryCache();

builder.Services.AddMediatR(typeof(Program), typeof(UpdateProductRequestHandler), typeof(DeleteProductRequestHandler), typeof(GetProductRequestHandler), typeof(GetProductRequestHandler), typeof(CreateProductRequestHandler));


var app = builder.Build();

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetService<ProductDbContext>();
dbContext.Database.MigrateAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
