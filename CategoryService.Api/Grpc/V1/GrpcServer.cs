using CategoryService.Api.Grpc.Mapping;
using CategoryService.Api.Grpc.V1.Protos;
using CategoryService.Application.Features.Commands.ActivateCategory;
using CategoryService.Application.Features.Commands.DeactivateCategory;
using CategoryService.Application.Features.Commands.DeleteCategory;
using CategoryService.Application.Features.Commands.MoveCategoryTo;
using CategoryService.Application.Features.Commands.ReorderCategory;
using CategoryService.Application.Features.Queries.GetAllCategories;
using CategoryService.Application.Features.Queries.GetAllCategoriesAdmin;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using migApp.Shared.Grpc;

namespace CategoryService.Api.Grpc.V1;

internal sealed class GrpcServer(IMediator mediator) : Protos.CategoryService.CategoryServiceBase
{
    public override async Task<Empty> CreateCategory(CreateCategoryRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(request.ToCreateCommand(), context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> UpdateCategoryInfo(UpdateCategoryInfoRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(request.ToUpdateInfoCommand(), context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> MoveCategoryTo(MoveCategoryToRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new MoveCategoryToCommand(
            Guid.Parse(request.CategoryId),
            request.HasNewParentCategoryId ?
                Guid.Parse(request.NewParentCategoryId) : null), context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> ReorderCategory(ReorderCategoryRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new ReorderCategoryCommand(
            Guid.Parse(request.CategoryId),
            request.NewPosition), context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> ActivateCategory(ActivateCategoryRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new ActivateCategoryCommand(Guid.Parse(request.CategoryId)), context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> DeactivateCategory(DeactivateCategoryRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new DeactivateCategoryCommand(Guid.Parse(request.CategoryId)), context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> DeleteCategory(DeleteCategoryRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new DeleteCategoryCommand(Guid.Parse(request.CategoryId)), context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<GetAllCategoriesResponse> GetAllCategories(GetAllCategoriesRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new GetAllCategoriesQuery(), context.CancellationToken);
        return new GetAllCategoriesResponse
        {
            Trees = { result.ThrowIfFailure().Select(t => t.ToGrpc()) }
        };
    }

    public override async Task<GetAllCategoriesAdminResponse> GetAllCategoriesAdmin(GetAllCategoriesAdminRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new GetAllCategoriesAdminQuery(), context.CancellationToken);
        return new GetAllCategoriesAdminResponse
        {
            Trees = { result.ThrowIfFailure().Select(t => t.ToGrpc()) }
        };
    }

    public override async Task<SearchCategoriesResponse> SearchCategories(SearchCategoriesRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(request.ToSearchCategoriesQuery(), context.CancellationToken);
        return new SearchCategoriesResponse
        {
            Categories = { result.ThrowIfFailure().Select(c => c.ToGrpc()) }
        };
    }
}