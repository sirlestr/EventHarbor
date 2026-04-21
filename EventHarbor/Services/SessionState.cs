using EventHarbor.Domain;

namespace EventHarbor.Services;

public class SessionState
{
    public User? LoggedUser { get; private set; }

    public int UserId => LoggedUser?.UserId ?? 0;
    public string UserName => LoggedUser?.UserName ?? string.Empty;
    public bool IsLoggedIn => LoggedUser is not null;

    public void SetLoggedUser(User user) => LoggedUser = user;
    public void Clear() => LoggedUser = null;
}
