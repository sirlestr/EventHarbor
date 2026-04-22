using EventHarbor.Services;

namespace EventHarbor.Tests;

public class UserServiceTests
{
    private static (UserService svc, SessionState session, TestDbFactory factory) CreateService()
    {
        var factory = new TestDbFactory();
        var session = new SessionState();
        var svc = new UserService(factory, session);
        return (svc, session, factory);
    }

    [Fact]
    public async Task Register_creates_user_and_second_duplicate_is_rejected()
    {
        var (svc, _, factory) = CreateService();
        using var _f = factory;

        var ok = await svc.RegisterAsync("jana.nova", "heslo123", "Město narození", "Praha");
        Assert.True(ok);

        var dup = await svc.RegisterAsync("jana.nova", "other", "Město narození", "Brno");
        Assert.False(dup);
    }

    [Fact]
    public async Task Login_returns_Success_for_correct_credentials()
    {
        var (svc, session, factory) = CreateService();
        using var _f = factory;

        await svc.RegisterAsync("ada", "pwd", "Město narození", "Praha");

        var result = await svc.LoginAsync("ada", "pwd");

        Assert.Equal(LoginResult.Success, result);
        Assert.True(session.IsLoggedIn);
        Assert.Equal("ada", session.UserName);
    }

    [Fact]
    public async Task Login_returns_WrongPassword_for_bad_credentials()
    {
        var (svc, _, factory) = CreateService();
        using var _f = factory;

        await svc.RegisterAsync("ada", "right", "Město narození", "Praha");

        var result = await svc.LoginAsync("ada", "wrong");
        Assert.Equal(LoginResult.WrongPassword, result);
    }

    [Fact]
    public async Task Login_returns_UserNotFound_for_missing_user()
    {
        var (svc, _, factory) = CreateService();
        using var _f = factory;

        var result = await svc.LoginAsync("ghost", "anything");
        Assert.Equal(LoginResult.UserNotFound, result);
    }

    [Fact]
    public async Task VerifySecurityAnswer_is_case_insensitive_and_trimmed()
    {
        var (svc, _, factory) = CreateService();
        using var _f = factory;

        await svc.RegisterAsync("ada", "pwd", "Město narození", "Praha");

        Assert.True(await svc.VerifySecurityAnswerAsync("ada", "PRAHA"));
        Assert.True(await svc.VerifySecurityAnswerAsync("ada", "  praha  "));
        Assert.False(await svc.VerifySecurityAnswerAsync("ada", "Brno"));
    }

    [Fact]
    public async Task ResetPassword_allows_login_with_new_credentials()
    {
        var (svc, _, factory) = CreateService();
        using var _f = factory;

        await svc.RegisterAsync("ada", "old", "Město narození", "Praha");
        var ok = await svc.ResetPasswordAsync("ada", "new");
        Assert.True(ok);

        Assert.Equal(LoginResult.WrongPassword, await svc.LoginAsync("ada", "old"));
        Assert.Equal(LoginResult.Success, await svc.LoginAsync("ada", "new"));
    }

    [Fact]
    public async Task ResetPassword_returns_false_for_unknown_user()
    {
        var (svc, _, factory) = CreateService();
        using var _f = factory;

        Assert.False(await svc.ResetPasswordAsync("ghost", "anything"));
    }

    [Fact]
    public async Task GetSecurityQuestion_returns_null_for_missing_user()
    {
        var (svc, _, factory) = CreateService();
        using var _f = factory;

        Assert.Null(await svc.GetSecurityQuestionAsync("ghost"));
    }

    [Fact]
    public async Task PasswordHash_is_not_plaintext()
    {
        var (svc, _, factory) = CreateService();
        using var _f = factory;

        await svc.RegisterAsync("ada", "plainSecret123", "Město narození", "Praha");
        var user = await svc.GetByUserNameAsync("ada");

        Assert.NotNull(user);
        Assert.NotEqual("plainSecret123", user!.PasswordHash);
        Assert.StartsWith("$2", user.PasswordHash);   // BCrypt prefix
    }
}
