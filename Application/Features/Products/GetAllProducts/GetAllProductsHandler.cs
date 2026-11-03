using System;
using System.Collections.Generic;
using System.Text;
namespace Application.Features.Products.GetAllProducts;

using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

public sealed class GetAllProductsHandler
    : IQueryHandler<GetAllProductsQuery, Result<List<GetAllProductsResponse>>>
{
    private readonly IAppDbContext _dbContext;

    public GetAllProductsHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<GetAllProductsResponse>>> HandleAsync(
        GetAllProductsQuery query,
        CancellationToken cancellationToken = default)
    {
        var products = await _dbContext.Products
            .Select(product => new GetAllProductsResponse(
                product.Id,
                product.Name,
                product.Sku,
                product.Description,
                product.Price,
                product.StockQuantity,
                product.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return Result<List<GetAllProductsResponse>>.Success(products);
    }
}