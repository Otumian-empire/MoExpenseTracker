using MoExpenseTracker.Core;

using CategoryModel = MoExpenseTracker.Models.Category;

namespace MoExpenseTracker.Features.V0.Category;

class CategoryController(CategoryDao dao)
{
    public async Task<IResult> CreateCategory(HttpContext httpContext, CreateCategoryDto dto)
    {
        // Check if the user is authenticated
        if (!(httpContext.User.Identity?.IsAuthenticated ?? false))
        {
            // Return Unauthorized if user is not authenticated
            return Results.Unauthorized();
        }

        // Find the claims for id and email
        var userId = int.Parse(httpContext.User.FindFirst("id")?.Value!);
        // email has been removed from the User claims
        // var userEmail = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;

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

    public async Task<IResult> ListCategories(HttpContext httpContext)
    {
        // Check if the user is authenticated
        if (!(httpContext.User.Identity?.IsAuthenticated ?? false))
        {
            // Return Unauthorized if user is not authenticated
            return Results.Unauthorized();
        }

        // Find the claims for id and email
        var userId = int.Parse(httpContext.User.FindFirst("id")?.Value!);
        // email has been removed from the User claims
        // var userEmail = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;

        var categories = await dao.ListCategoriesByUserId(userId);


        return Results.Ok<SuccessResponseWithData<List<CategoryModel>>>(new(categories));
    }

    public async Task<IResult> ReadCategory(HttpContext httpContext, int categoryId)
    {
        // Check if the user is authenticated
        if (!(httpContext.User.Identity?.IsAuthenticated ?? false))
        {
            // Return Unauthorized if user is not authenticated
            return Results.Unauthorized();
        }

        // Find the claims for id and email
        var userId = int.Parse(httpContext.User.FindFirst("id")?.Value!);
        // email has been removed from the User claims
        // var userEmail = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;

        var category = await dao.ReadCategoryById(userId, categoryId);
        if (category is null)
        {
            return Results.Ok<FailureResponse>(new("Category not found"));
        }

        return Results.Ok<SuccessResponseWithData<CategoryModel>>(new(category));
    }

    public async Task<IResult> UpdateCategory(HttpContext httpContext, int categoryId, UpdateCategoryDto dto)
    {
        // Check if the user is authenticated
        if (!(httpContext.User.Identity?.IsAuthenticated ?? false))
        {
            // Return Unauthorized if user is not authenticated
            return Results.Unauthorized();
        }

        // Find the claims for id and email
        var userId = int.Parse(httpContext.User.FindFirst("id")?.Value!);
        // email has been removed from the User claims
        // var userEmail = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;

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