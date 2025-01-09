using MoExpenseTracker.Core;

using CategoryModel = MoExpenseTracker.Models.Category;

namespace MoExpenseTracker.Features.V0.Category;

static class RequestHandlers
{
    public static async Task<IResult> CreateCategory(
        CategoryDao dao,
        HttpContext httpContext,
        CreateCategoryDto dto)
    {
        // Check if the user is authenticated
        if (!(httpContext.User.Identity?.IsAuthenticated ?? false))
        {
            // Return Unauthorized if user is not authenticated
            return Results.Unauthorized();
        }

        // Find the claims for id and email
        var userId = int.Parse(httpContext.User.FindFirst("id")?.Value!);

        var category = await dao.ReadCategoryByName(userId, dto.Name);
        if (category is not null)
        {
            return Results.BadRequest<FailureResponse>(new("Category name already exists"));
        }

        var newCategory = new CategoryModel()
        {
            UserId = userId,
            Name = dto.Name,
            Description = dto.Description,
        };

        var response = await dao.CreateCategory(newCategory);
        if (response is null)
        {
            return Results.BadRequest<FailureResponse>(new("Coulkd not create category"));
        }

        return Results.Ok<SuccessResponseWithData<CategoryModel>>(new(response));
    }

    public static async Task<IResult> ListCategories(
        CategoryDao dao,
        HttpContext httpContext)
    {
        // Check if the user is authenticated
        if (!(httpContext.User.Identity?.IsAuthenticated ?? false))
        {
            // Return Unauthorized if user is not authenticated
            return Results.Unauthorized();
        }

        // Find the claims for id and email
        var userId = int.Parse(httpContext.User.FindFirst("id")?.Value!);

        var categories = await dao.ListCategoriesByUserId(userId);

        return Results.Ok<SuccessResponseWithData<List<CategoryModel>>>(new(categories));
    }

    public static async Task<IResult> ReadCategory(
        CategoryDao dao,
        HttpContext httpContext,
        int categoryId)
    {
        // Check if the user is authenticated
        if (!(httpContext.User.Identity?.IsAuthenticated ?? false))
        {
            // Return Unauthorized if user is not authenticated
            return Results.Unauthorized();
        }

        // Find the claims for id and email
        var userId = int.Parse(httpContext.User.FindFirst("id")?.Value!);

        var category = await dao.ReadCategoryById(userId, categoryId);
        if (category is null)
        {
            return Results.Ok<FailureResponse>(new("Category not found"));
        }

        return Results.Ok<SuccessResponseWithData<CategoryModel>>(new(category));
    }

    public static async Task<IResult> UpdateCategory(
        CategoryDao dao,
        HttpContext httpContext,
        int categoryId,
        UpdateCategoryDto dto)
    {
        // Check if the user is authenticated
        if (!(httpContext.User.Identity?.IsAuthenticated ?? false))
        {
            // Return Unauthorized if user is not authenticated
            return Results.Unauthorized();
        }

        // Find the claims for id and email
        var userId = int.Parse(httpContext.User.FindFirst("id")?.Value!);

        var category = await dao.ReadCategoryById(userId, categoryId);
        if (category is null)
        {
            return Results.Ok<FailureResponse>(new("Category not found"));
        }

        category.Name = dto.Name ?? category.Name;
        category.Description = dto.Description ?? category.Description;
        category.UpdatedAt = DateTime.UtcNow;

        var updatedCategory = await dao.UpdateCategory(category);
        if (updatedCategory is null)
        {
            return Results.Ok<FailureResponse>(new("Could update category, please try again"));
        }

        return Results.Ok<SuccessResponseWithData<CategoryModel>>(new(updatedCategory));
    }
}