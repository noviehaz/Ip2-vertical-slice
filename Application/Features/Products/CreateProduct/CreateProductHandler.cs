namespace Application.Features.Products.CreateProduct;

using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class CreateProductCommandHandler
    : ICommandHandler<CreateProductCommand, Result<CreateProductResponse>>
{
    private readonly IAppDbContext _dbContext;

    public CreateProductCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<CreateProductResponse>> HandleAsync(
        CreateProductCommand command,
        CancellationToken cancellationToken = default)
    {
        // Check if SKU already exists
        var skuExists = await _dbContext.Products
            .AnyAsync(p => p.Sku == command.Sku, cancellationToken);

        if (skuExists)
        {
            return Result<CreateProductResponse>.Failure(
                Error.Conflict(
                    "Product.SkuConflict",
                    "A product with the same SKU already exists."
                ));
        }

        // Create product
        var product = new Product
        {
            Name = command.Name,
            Sku = command.Sku,
            Description = command.Description,
            Price = command.Price,
            StockQuantity = command.StockQuantity,
            CreatedAt = DateTime.UtcNow
        };

        // Add product
        _dbContext.Products.Add(product);

        // Save changes
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Create response
        var response = new CreateProductResponse(
            product.Id,
            product.Name,
            product.Sku,
            product.Description,
            product.Price,
            product.StockQuantity,
            product.CreatedAt
        );

        return Result<CreateProductResponse>.Success(response);
    }
}