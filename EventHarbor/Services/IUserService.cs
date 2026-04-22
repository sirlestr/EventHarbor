using EventHarbor.Domain;

namespace EventHarbor.Services;

public interface IUserService
{
    Task<LoginResult> LoginAsync(string userName, string password, CancellationToken ct = default);
    Task<bool> RegisterAsync(string userName, string password, string securityQuestion, string securityAnswer, CancellationToken ct = default);
    Task<string?> GetSecurityQuestionAsync(string userName, CancellationToken ct = default);
    Task<bool> VerifySecurityAnswerAsync(string userName, string securityAnswer, CancellationToken ct = default);
    Task<bool> ResetPasswordAsync(string userName, string newPassword, CancellationToken ct = default);
    Task<User?> GetByUserNameAsync(string userName, CancellationToken ct = default);
}
