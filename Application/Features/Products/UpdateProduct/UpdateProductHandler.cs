using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Products.UpdateProduct;

using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

public sealed class UpdateProductHandler
    : ICommandHandler<UpdateProductCommand, Result<UpdateProductResponse>>
{
    private readonly IAppDbContext _dbContext;

    public UpdateProductHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<UpdateProductResponse>> HandleAsync(
        UpdateProductCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Check if product exists
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(
                p => p.Id == command.Id,
                cancellationToken);

        if (product is null)
        {
            return Result<UpdateProductResponse>.Failure(
                Error.NotFound(
                    "Product.NotFound",
                    "Product was not found."
                ));
        }

        // 2. Check if the new SKU belongs to another product
        var skuExists = await _dbContext.Products
            .AnyAsync(
                p => p.Sku == command.Sku && p.Id != command.Id,
                cancellationToken);

        if (skuExists)
        {
            return Result<UpdateProductResponse>.Failure(
                Error.Conflict(
                    "Product.SkuConflict",
                    "A product with the same SKU already exists."
                ));
        }

        // 3. Update product fields
        product.Name = command.Name;
        product.Sku = command.Sku;
        product.Description = command.Description;
        product.Price = command.Price;
        product.StockQuantity = command.StockQuantity;

        // CreatedAt is intentionally NOT changed

        // 4. Save changes
        await _dbContext.SaveChangesAsync(cancellationToken);

        // 5. Create response
        var response = new UpdateProductResponse(
            product.Id,
            product.Name,
            product.Sku,
            product.Description,
            product.Price,
            product.StockQuantity,
            product.CreatedAt
        );

        return Result<UpdateProductResponse>.Success(response);
    }
}