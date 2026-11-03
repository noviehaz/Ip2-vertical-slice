using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Products.GetAllProducts;

public sealed record GetAllProductsResponse(
    Guid Id,
    string Name,
    string Sku,
    string? Description,
    decimal Price,
    int StockQuantity,
    DateTime CreatedAt
);