using EventHarbor.Data;
using EventHarbor.Domain;
using Microsoft.EntityFrameworkCore;

namespace EventHarbor.Services;

public class UserService : IUserService
{
    private const int BcryptWorkFactor = 12;

    private readonly IDbContextFactory<EventHarborDbContext> _factory;
    private readonly SessionState _session;

    public UserService(IDbContextFactory<EventHarborDbContext> factory, SessionState session)
    {
        _factory = factory;
        _session = session;
    }

    public async Task<LoginResult> LoginAsync(string userName, string password, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        var user = await db.Users.FirstOrDefaultAsync(u => u.UserName == userName, ct);

        if (user is null) return LoginResult.UserNotFound;
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return LoginResult.WrongPassword;

        _session.SetLoggedUser(user);
        return LoginResult.Success;
    }

    public async Task<bool> RegisterAsync(string userName, string password, string securityQuestion, string securityAnswer, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);

        if (await db.Users.AnyAsync(u => u.UserName == userName, ct))
            return false;

        var user = new User
        {
            UserName = userName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, BcryptWorkFactor),
            SecurityQuestion = securityQuestion,
            SecurityAnswerHash = HashSecurityAnswer(securityAnswer),
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<string?> GetSecurityQuestionAsync(string userName, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        return await db.Users
            .Where(u => u.UserName == userName)
            .Select(u => u.SecurityQuestion)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<bool> VerifySecurityAnswerAsync(string userName, string securityAnswer, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        var user = await db.Users.FirstOrDefaultAsync(u => u.UserName == userName, ct);
        if (user is null) return false;

        var normalized = NormalizeAnswer(securityAnswer);
        return BCrypt.Net.BCrypt.Verify(normalized, user.SecurityAnswerHash);
    }

    public async Task<bool> ResetPasswordAsync(string userName, string newPassword, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        var user = await db.Users.FirstOrDefaultAsync(u => u.UserName == userName, ct);
        if (user is null) return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword, BcryptWorkFactor);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<User?> GetByUserNameAsync(string userName, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        return await db.Users.FirstOrDefaultAsync(u => u.UserName == userName, ct);
    }

    private static string HashSecurityAnswer(string answer)
        => BCrypt.Net.BCrypt.HashPassword(NormalizeAnswer(answer), BcryptWorkFactor);

    private static string NormalizeAnswer(string answer)
        => (answer ?? string.Empty).Trim().ToLowerInvariant();
}
