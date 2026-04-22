using System.IO;
using System.Security.Cryptography;
using System.Text;
using EventHarbor.Data;

namespace EventHarbor.Services;

/// <summary>
/// Persists the last logged user id via Windows DPAPI (CurrentUser scope).
/// The token is readable only by the same Windows user on the same machine -
/// so opting into "Zustat prihlaseny" is equivalent to trusting this OS session.
/// </summary>
public class RememberMeService
{
    private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("EventHarbor.RememberMe.v1");
    private static string FilePath => Path.Combine(AppPaths.LocalAppFolder, "session.bin");

    public void Save(int userId)
    {
        try
        {
            Directory.CreateDirectory(AppPaths.LocalAppFolder);
            var plain = BitConverter.GetBytes(userId);
            var encrypted = ProtectedData.Protect(plain, Entropy, DataProtectionScope.CurrentUser);
            File.WriteAllBytes(FilePath, encrypted);
        }
        catch
        {
            // best-effort; remember-me is convenience, not load-bearing
        }
    }

    public int? TryLoad()
    {
        try
        {
            if (!File.Exists(FilePath)) return null;
            var encrypted = File.ReadAllBytes(FilePath);
            var plain = ProtectedData.Unprotect(encrypted, Entropy, DataProtectionScope.CurrentUser);
            if (plain.Length != sizeof(int)) return null;
            return BitConverter.ToInt32(plain, 0);
        }
        catch
        {
            // Corrupted / moved machine / different user - force re-login.
            Clear();
            return null;
        }
    }

    public void Clear()
    {
        try
        {
            if (File.Exists(FilePath)) File.Delete(FilePath);
        }
        catch
        {
        }
    }
}
