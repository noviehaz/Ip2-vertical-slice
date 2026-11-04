using Api.Features.Admissions;
using Api.Features.Courses;
using Api.Features.Students;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Api.Features.Products;
using Application.Features.Admissions.CreateAdmission;
using Application.Features.Admissions.GetAdmissionById;
using Application.Features.Admissions.GetAllAdmissions;
using Application.Features.Courses.CreateCourse;
using Application.Features.Courses.DeleteCourse;
using Application.Features.Courses.GetAllCourses;
using Application.Features.Courses.GetCourseById;
using Application.Features.Courses.UpdateCourse;
using Application.Features.Products.CreateProduct;
using Application.Features.Products.DeleteProduct;
using Application.Features.Products.GetAllProducts;
using Application.Features.Products.GetProductById;
using Application.Features.Products.UpdateProduct;
using Application.Features.Students.CreateStudent;
using Application.Features.Students.GetAllStudents;
using Application.Features.Students.GetStudentById;
using Domain.Common;
using FluentValidation;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<IAppDbContext, AppDbContext>(options =>
 options.UseSqlite(builder.Configuration.GetConnectionString("Database")));

// Validators
builder.Services.AddValidatorsFromAssemblyContaining<CreateStudentValidator>();

// Handlers
builder.Services.AddScoped<ICommandHandler<CreateStudentCommand, Result<CreateStudentResponse>>, CreateStudentCommandHandler>();
builder.Services.AddScoped<IQueryHandler<GetAllStudentsQuery, Result<GetAllStudentsResponse>>, GetAllStudentsQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetStudentByIdQuery, Result<GetStudentByIdResponse>>, GetStudentByIdQueryHandler>();

builder.Services.AddScoped<ICommandHandler<CreateCourseCommand, Result<CreateCourseResponse>>, CreateCourseCommandHandler>();
builder.Services.AddScoped<IQueryHandler<GetAllCoursesQuery, Result<GetAllCoursesResponse>>, GetAllCoursesQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetCourseByIdQuery, Result<GetCourseByIdResponse>>, GetCourseByIdQueryHandler>();

builder.Services.AddScoped<ICommandHandler<UpdateCourseCommand, Result<UpdateCourseResponse>>, UpdateCourseCommandHandler>();

builder.Services.AddScoped<ICommandHandler<CreateAdmissionCommand, Result<CreateAdmissionResponse>>, CreateAdmissionCommandHandler>();

builder.Services.AddScoped<ICommandHandler<DeleteCourseCommand, Result>,
    DeleteCourseCommandHandler>();

builder.Services.AddScoped<
    ICommandHandler<CreateProductCommand, Result<CreateProductResponse>>,
    CreateProductCommandHandler>();

builder.Services.AddScoped<
    IQueryHandler<GetProductByIdQuery, Result<GetProductByIdResponse>>,
    GetProductByIdHandler>();

builder.Services.AddScoped<
    IQueryHandler<GetAllProductsQuery, Result<List<GetAllProductsResponse>>>,
    GetAllProductsHandler>();

builder.Services.AddScoped<
    ICommandHandler<UpdateProductCommand, Result<UpdateProductResponse>>,
    UpdateProductHandler>();

builder.Services.AddScoped<
    ICommandHandler<DeleteProductCommand, Result>,
    DeleteProductHandler>();



//builder.Services.AddScoped<IQueryHandler<GetAllAdmissionQuery, Result<GetAllAdmissionResponse>>, GetAllAdmissionQueryHandler>();

// API
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info.Title = "Vertical Slice API";
        document.Info.Description = "A simple API demonstrating Vertical Slice Architecture";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    dbContext.Students.AddRange(
        new Domain.Entities.Student
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            DateOfBirth = new DateTime(2000, 1, 15)
        },
        new Domain.Entities.Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            DateOfBirth = new DateTime(2001, 5, 20)
        }
    );

    dbContext.Courses.AddRange(
        new Domain.Entities.Course
        {
            Id = Guid.NewGuid(),
            Title = "Introduction to Programming",
            Code = "CS101",
            Credits = 3
        },
        new Domain.Entities.Course
        {
            Id = Guid.NewGuid(),
            Title = "Data Structures",
            Code = "CS201",
            Credits = 4
        },
        new Domain.Entities.Course
        {
            Id = Guid.NewGuid(),
            Title = "Database Systems",
            Code = "CS301",
            Credits = 3
        }
    );

    dbContext.Products.AddRange(
        new Domain.Entities.Product
        {
            Id = Guid.NewGuid(),
            Name = "Wireless Mouse",
            Sku = "SKU-MOU-001",
            Description = "Ergonomic 2.4GHz wireless mouse",
            Price = 799.00m,
            StockQuantity = 25,
            CreatedAt = DateTime.UtcNow
        },
        new Domain.Entities.Product
        {
            Id = Guid.NewGuid(),
            Name = "Mechanical Keyboard",
            Sku = "SKU-KEY-002",
            Description = null,
            Price = 3499.50m,
            StockQuantity = 10,
            CreatedAt = DateTime.UtcNow
        }
    );

    dbContext.SaveChanges();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Root endpoint
app.MapGet("/", () =>
    "Welcome to the Vertical Slice API! Go to /scalar/v1 for API documentation.");

// Map endpoints
app.MapStudentEndpoints();
app.MapCourseEndpoints();
app.MapAdmissionEndpoints();
app.MapProductEndpoints();

app.Run();