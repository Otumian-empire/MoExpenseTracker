using MoExpenseTracker.Core;
using MoExpenseTracker.Features.V0.Auth;

namespace MoExpenseTracker.Features.V0.Account;

static class RequestHandlers
{
    public static async Task<IResult> ReadProfile(
        AccountDataAccess accountDao,
        ICurrentUser currentUser)
    {
        var userId = currentUser.UserId();

        var user = await accountDao.ReadProfile(userId);
        if (user == null)
        {
            return Results.Unauthorized();
        }

        var profile = new UserProfileDto(user);

        // Return user profile data
        return Results.Ok<SuccessResponseWithData<UserProfileDto>>(new(profile));
    }

    public static async Task<IResult> UpdateProfile(
        AccountDataAccess accountDao,
        AuthDataAccess authDao,
        ICurrentUser currentUser,
        UpdateProfileDto dto)
    {
        var userId = currentUser.UserId();

        var user = await accountDao.ReadProfile(userId);
        if (user == null)
        {
            return Results.Unauthorized();
        }

        dto.Email = dto.Email.ToLower();

        var emailUser = await authDao.GetUserByEmail(dto.Email);
        if (emailUser is not null && emailUser.Id != user.Id)
        {
            return Results.Ok<FailureResponse>(new("Email already taken"));
        }

        user.Name = dto.Name;
        user.Email = dto.Email;
        user.UpdatedAt = DateTime.UtcNow;

        var updatedUser = await accountDao.UpdateProfile(user);
        var profile = new UserProfileDto(updatedUser!);

        // Return user profile data
        return Results.Ok<SuccessResponseWithData<UserProfileDto>>(new(profile));
    }
}