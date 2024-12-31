namespace MoExpenseTracker.Features.Category;

class CreateCategoryDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}

class UpdateCategoryDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}

