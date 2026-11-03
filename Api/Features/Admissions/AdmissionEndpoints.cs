namespace Api.Features.Admissions;

using Api.Common.Extensions;
using Application.Abstractions.Messaging;
using Application.Features.Admissions.CreateAdmission;
using Application.Features.Admissions.GetAdmissionById;
using Application.Features.Admissions.GetAllAdmissions;
using Domain.Common;
using Microsoft.AspNetCore.Mvc;

public static class AdmissionEndpoints
{
    private const string GetAllRouteName = "GetAllAdmissions";
    private const string GetByIdRouteName = "GetAdmissionById";

    public static void MapAdmissionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admissions")
            .WithTags("Admissions");

        // POST: /api/admissions
        group.MapPost("/", Create)
            .WithName("CreateAdmission")
            .WithSummary("Create a new admission with courses");

        // GET: /api/admissions
        group.MapGet("/", GetAll)
            .WithName(GetAllRouteName)
            .WithSummary("Get all admissions");

        // GET: /api/admissions/{id}
        group.MapGet("/{id:guid}", GetById)
            .WithName(GetByIdRouteName)
            .WithSummary("Get admission by ID with courses");
    }

    private static async Task<IResult> Create(
        CreateAdmissionCommand command,
        [FromServices]
        ICommandHandler<CreateAdmissionCommand, Result<CreateAdmissionResponse>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            command,
            cancellationToken);

        return result.IsSuccess
            ? TypedResults.CreatedAtRoute(
                result.Value,
                GetByIdRouteName,
                new { id = result.Value!.Id })
            : result.ToProblemDetails();
    }

    private static async Task<IResult> GetAll(
        [FromServices]
        IQueryHandler<GetAllAdmissionsQuery, Result<GetAllAdmissionsResponse>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetAllAdmissionsQuery();

        var result = await handler.HandleAsync(
            query,
            cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> GetById(
        Guid id,
        [FromServices]
        IQueryHandler<GetAdmissionByIdQuery, Result<GetAdmissionByIdResponse>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetAdmissionByIdQuery(id);

        var result = await handler.HandleAsync(
            query,
            cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.ToProblemDetails();
    }
}

