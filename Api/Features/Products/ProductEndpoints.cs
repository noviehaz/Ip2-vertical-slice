namespace Api.Features.Products;

using Api.Common.Extensions;
using Application.Abstractions.Messaging;
using Application.Features.Products.CreateProduct;
using Application.Features.Products.GetAllProducts;
using Application.Features.Products.GetProductById;
using Application.Features.Products.UpdateProduct;
using Application.Features.Products.DeleteProduct;
using Domain.Common;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
            .WithTags("Products");

        // POST /api/products
        group.MapPost("/", async (
            CreateProductCommand command,
            ICommandHandler<CreateProductCommand, Result<CreateProductResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(command, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Created(
                    $"/api/products/{result.Value!.Id}",
                    result.Value);
            }

            return result.ToProblemDetails();
        })
        .WithName("CreateProduct")
        .WithSummary("Creates a new product.");

        // GET /api/products
        group.MapGet("/", async (
            IQueryHandler<GetAllProductsQuery, Result<List<GetAllProductsResponse>>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllProductsQuery();

            var result = await handler.HandleAsync(query, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }

            return result.ToProblemDetails();
        })
        .WithName("GetAllProducts")
        .WithSummary("Gets all products.");

        // GET /api/products/{id}
        group.MapGet("/{id:guid}", async (
            Guid id,
            IQueryHandler<GetProductByIdQuery, Result<GetProductByIdResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetProductByIdQuery(id);

            var result = await handler.HandleAsync(query, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }

            return result.ToProblemDetails();
        })
        .WithName("GetProductById")
        .WithSummary("Gets a product by its ID.");

        // PUT /api/products/{id}
        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateProductRequest request,
            ICommandHandler<UpdateProductCommand, Result<UpdateProductResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateProductCommand(
                id,
                request.Name,
                request.Sku,
                request.Description,
                request.Price,
                request.StockQuantity
            );

            var result = await handler.HandleAsync(
                command,
                cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }

            return result.ToProblemDetails();
        })
        .WithName("UpdateProduct")
        .WithSummary("Updates an existing product.");

        // DELETE /api/products/{id}
        group.MapDelete("/{id:guid}", async (
            Guid id,
            ICommandHandler<DeleteProductCommand, Result> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteProductCommand(id);

            var result = await handler.HandleAsync(
                command,
                cancellationToken);

            if (result.IsSuccess)
            {
                return Results.NoContent();
            }

            return result.ToProblemDetails();
        })
        .WithName("DeleteProduct")
        .WithSummary("Deletes a product by its ID.");
    }

    // Request body for PUT.
    // Id comes only from the URL.
    public sealed record UpdateProductRequest(
        string Name,
        string Sku,
        string? Description,
        decimal Price,
        int StockQuantity
    );
}