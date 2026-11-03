using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Products.GetAllProducts;

using Application.Abstractions.Messaging;
using Domain.Common;

public sealed record GetAllProductsQuery
    : IQuery<Result<List<GetAllProductsResponse>>>;