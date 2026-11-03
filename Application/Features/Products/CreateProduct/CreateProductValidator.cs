using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Products.CreateProduct;

using FluentValidation;

public sealed class CreateProductValidator
    : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Product name is required.")
            .MaximumLength(150)
            .WithMessage("Product name must not exceed 150 characters.");

        RuleFor(x => x.Sku)
            .NotEmpty()
            .WithMessage("SKU is required.")
            .MaximumLength(50)
            .WithMessage("SKU must not exceed 50 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Product description must not exceed 500 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock quantity cannot be negative.");
    }
}