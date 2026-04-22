namespace EventHarbor.Services;

public enum LoginResult
{
    UserNotFound,
    WrongPassword,
    Success,
    Locked,
}
