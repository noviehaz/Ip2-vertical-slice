using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Products.DeleteProduct;

using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

public sealed class DeleteProductHandler
    : ICommandHandler<DeleteProductCommand, Result>
{
    private readonly IAppDbContext _dbContext;

    public DeleteProductHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> HandleAsync(
        DeleteProductCommand command,
        CancellationToken cancellationToken = default)
    {
        // Check if product exists
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(
                p => p.Id == command.Id,
                cancellationToken);

        if (product is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Product.NotFound",
                    "Product was not found."
                ));
        }

        // Delete product
        _dbContext.Products.Remove(product);

        // Save changes
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Successful deletion
        return Result.Success();
    }
}