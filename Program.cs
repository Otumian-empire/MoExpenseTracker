using System.Text;

using FluentValidation;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using MoExpenseTracker.Core;
using MoExpenseTracker.Data;
using MoExpenseTracker.Features;
using MoExpenseTracker.Features.V0.Account;
using MoExpenseTracker.Features.V0.Auth;
using MoExpenseTracker.Features.V0.Category;
using MoExpenseTracker.Features.V0.Expense;

namespace MoExpenseTracker;

class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // gloabal exeption handler
        builder.Services.AddExceptionHandler<AppExceptionHandler>(); // for errors that we throw
        builder.Services.AddExceptionHandler<UnHandledExceptionHandler>(); // for errors that we could not handle

        // database connection
        var connectionString = builder.Configuration.GetConnectionString("MoExpenseTrackerDb");
        builder.Services.AddDbContext<DatabaseContext>(
            options => options.UseNpgsql(connectionString));

        // authentication and authorization
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer((options) =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer"),
                ValidAudience = builder.Configuration.GetValue<string>("Jwt:Audience"),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Jwt:Key")!)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };
            options.RequireHttpsMetadata = false;
        });

        builder.Services.AddAuthorization();

        // register depencies
        builder.Services.AddScoped<AuthDao>();
        builder.Services.AddScoped<AuthUtil>();
        builder.Services.AddScoped<IValidator<SignupDto>, AuthSignupValidation>();
        builder.Services.AddScoped<IValidator<LoginDto>, AuthLoginValidation>();

        builder.Services.AddScoped<AccountDao>();
        builder.Services.AddScoped<IValidator<UpdateProfileDto>, AccountUpdateValidation>();

        builder.Services.AddScoped<CategoryDao>();
        builder.Services.AddScoped<IValidator<CreateCategoryDto>, CategoryCreationValidation>();
        builder.Services.AddScoped<IValidator<UpdateCategoryDto>, CategoryUpdateValidation>();

        builder.Services.AddScoped<ExpenseDao>();
        builder.Services.AddScoped<IValidator<CreateExpenseDto>, ExpenseCreationValidation>();
        builder.Services.AddScoped<IValidator<UpdateExpenseDto>, ExpenseUpdateValidation>();

        var app = builder.Build();

        app.UseExceptionHandler(_ => { });
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.AddVersionEndpoints();

        app.Run();
    }

}