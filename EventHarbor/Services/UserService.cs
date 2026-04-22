using System.Collections.Concurrent;
using System.Diagnostics;
using EventHarbor.Data;
using EventHarbor.Domain;
using Microsoft.EntityFrameworkCore;

namespace EventHarbor.Services;

public class UserService : IUserService
{
    private const int BcryptWorkFactor = 12;

    // Login throttling:
    //  - 3 failed attempts -> lock bucket for 60s
    //  - lock is keyed by username (lowercased) and is process-local (in-memory only)
    //  - timing-safe: every LoginAsync pads to >= 100ms total to avoid user-enumeration via response time
    private const int MaxLoginAttempts = 3;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromSeconds(60);
    private const int MinLoginResponseMs = 100;

    private sealed class LoginBucket
    {
        public int FailCount;
        public DateTime? LockedUntilUtc;
    }

    private static readonly ConcurrentDictionary<string, LoginBucket> _attempts = new();

    public static int GetRemainingLockSeconds(string userName)
    {
        if (!_attempts.TryGetValue(Key(userName), out var b) || b.LockedUntilUtc is null)
            return 0;
        var remaining = b.LockedUntilUtc.Value - DateTime.UtcNow;
        return remaining > TimeSpan.Zero ? (int)Math.Ceiling(remaining.TotalSeconds) : 0;
    }

    /// <summary>Reset in-memory lockout state (test hook / administrative unlock).</summary>
    public static void ClearAllLoginAttempts() => _attempts.Clear();

    private readonly IDbContextFactory<EventHarborDbContext> _factory;
    private readonly SessionState _session;

    public UserService(IDbContextFactory<EventHarborDbContext> factory, SessionState session)
    {
        _factory = factory;
        _session = session;
    }

    public async Task<LoginResult> LoginAsync(string userName, string password, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var key = Key(userName);
            var bucket = _attempts.GetOrAdd(key, _ => new LoginBucket());

            if (bucket.LockedUntilUtc is { } until && until > DateTime.UtcNow)
                return LoginResult.Locked;

            // Expired lock - clear and continue
            if (bucket.LockedUntilUtc is not null && bucket.LockedUntilUtc <= DateTime.UtcNow)
            {
                bucket.LockedUntilUtc = null;
                bucket.FailCount = 0;
            }

            await using var db = await _factory.CreateDbContextAsync(ct);
            var user = await db.Users.FirstOrDefaultAsync(u => u.UserName == userName, ct);

            if (user is null)
            {
                RegisterFailure(bucket);
                return LoginResult.UserNotFound;
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                RegisterFailure(bucket);
                return LoginResult.WrongPassword;
            }

            // success: clear attempts
            _attempts.TryRemove(key, out _);
            _session.SetLoggedUser(user);
            return LoginResult.Success;
        }
        finally
        {
            var elapsed = (int)sw.ElapsedMilliseconds;
            if (elapsed < MinLoginResponseMs)
                await Task.Delay(MinLoginResponseMs - elapsed, CancellationToken.None);
        }
    }

    private static void RegisterFailure(LoginBucket bucket)
    {
        var count = Interlocked.Increment(ref bucket.FailCount);
        if (count >= MaxLoginAttempts)
            bucket.LockedUntilUtc = DateTime.UtcNow + LockoutDuration;
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
        // Successful reset also clears any lockout
        _attempts.TryRemove(Key(userName), out _);
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

    private static string Key(string userName) => (userName ?? string.Empty).Trim().ToLowerInvariant();
}
