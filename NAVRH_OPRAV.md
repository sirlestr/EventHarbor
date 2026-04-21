# NÁVRH OPRAV – EventHarbor

> Praktický průvodce k posunu projektu na profesionální úroveň.
> Opravy jsou seřazeny dle priority. Začni Prioritou 1 — nic jiného není tak důležité.

---

## Priorita 1 — Kritické (nesnesou odklad)

### ✅ P1-1: Nahradit SHA-256 bcryptem pro hashování hesel

**Proč:** SHA-256 bez soli je triviálně prolomitelné pomocí rainbow tables. Každé heslo musí mít jedinečnou sůl.

**Instalace balíčku:**
```bash
dotnet add package BCrypt.Net-Next
```

**Diff — `User.cs`:**
```csharp
// PŘED (nebezpečné)
using System.Security.Cryptography;
using System.Text;

public string getHashEncryption(string userPasswd)
{
    using (SHA256 sha256Hash = SHA256.Create())
    {
        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(userPasswd));
        return Convert.ToBase64String(bytes);
    }
}

// PO (bezpečné) — sůl je automaticky vložena do výsledného hashe
using BCrypt.Net;

public static string HashPassword(string password)
    => BCrypt.HashPassword(password, workFactor: 12);

public static bool VerifyPassword(string password, string hash)
    => BCrypt.Verify(password, hash);
```

**Diff — `UserManager.cs`:**
```csharp
// PŘED
User user = new User(name, passwd);  // hash v konstruktoru

// PO — hash mimo konstruktor, konstruktor je čistý
public bool AddUser(string name, string passwd)
{
    using var context = new DatabaseContextManager();
    var user = new User
    {
        UserName = name,
        UserHash = User.HashPassword(passwd)
    };
    context.Users.Add(user);
    context.SaveChanges();
    return true;
}

// PŘED – ověření
else if (user.getHashEncryption(passwd) != user.UserHash)

// PO
else if (!User.VerifyPassword(passwd, user.UserHash))
```

---

### ✅ P1-2: Přidat ověření identity při resetu hesla

**Proč:** Aktuálně stačí znát uživatelské jméno pro resetování hesla komukoliv.

**Možnost A (jednodušší) — vyžadovat staré heslo:**
```csharp
// ForgotScreen.xaml — přidat pole OldPasswordBox

// ForgotScreen.xaml.cs
private void ResetPwdBtn_Click(object sender, RoutedEventArgs e)
{
    UserManager userManager = new UserManager();
    // Ověř staré heslo před resetem
    if (userManager.IsRegistered(UserNameTextBox.Text, OldPwdBox.Password) != 1)
    {
        MessageBox.Show("Uživatel nenalezen nebo špatné staré heslo.");
        return;
    }
    if (userManager.ResetPassword(UserNameTextBox.Text, NewPwdBox.Password))
    {
        MessageBox.Show("Heslo bylo úspěšně změněno.");
        this.Close();
    }
}
```

**Možnost B (profesionálnější) — přejmenovat na "Změna hesla" a přesunout do profilu přihlášeného uživatele.** Funkce "forgot password" bez emailu nedává smysl pro desktopovou app.

---

### ✅ P1-3: Odstranit ResetDbButton z produkce

```xml
<!-- LoginScreen.xaml — smazat nebo podmínit DEBUG direktivou -->
<!-- Smazat nebo zakomentovat: -->
<!-- <Button x:Name="ResetDbButton" Content="Reset Db" Click="ResetDbButton_Click"/> -->
```

```csharp
// Pokud chceš ponechat pro vývoj, obal direktivou:
#if DEBUG
private void ResetDbButton_Click(object sender, RoutedEventArgs e) { ... }
#endif
```

---

### ✅ P1-4: Opravit bug — `MergeDataWithDb` vždy vrací `false`

```csharp
// DatabaseContextManager.cs — AKTUÁLNÍ (bug)
internal bool MergeDataWithDb(...)
{
    using (DatabaseContextManager context = new())
    {
        // ... logika ...
    }
    return false; // ← BUG: vždy false, i při úspěchu
}

// OPRAVA: přesuň return dovnitř using bloku
internal bool MergeDataWithDb(ObservableCollection<CultureAction> localCollection, int ownerId)
{
    using var context = new DatabaseContextManager();
    // ... stejná logika ...
    context.SaveChanges();
    return true; // ← vrátit true při úspěchu
}
```

---

### ✅ P1-5: Opravit načítání uživatelů (výkon + správnost)

```csharp
// PŘED — načte VŠECHNY uživatele do paměti
User user = context.Users.ToList().Find(u => u.UserName == name);

// PO — SQL WHERE dotaz, efektivní
User? user = context.Users.FirstOrDefault(u => u.UserName == name);
```

---

### ✅ P1-6: Opravit nesprávný `[Owned]` atribut na `User`

```csharp
// PŘED — User.cs
[Owned]          // ← špatně: [Owned] je pro Value Objects
public class User { ... }

// PO — odstranit [Owned], přidat Table atribut
[Table("Users")] // nebo bez atributu (EF Core to odvodí z názvu DbSet)
public class User { ... }
```

---

## Priorita 2 — Refaktorizace (lepší čitelnost a struktura)

### 🔧 P2-1: Nahradit magic numbers enumerací pro výsledky přihlášení

```csharp
// NOVÝ soubor: Class/LoginResult.cs
namespace EventHarbor.Class
{
    public enum LoginResult
    {
        UserNotFound = -1,
        WrongPassword = 0,
        Success = 1
    }
}

// UserManager.cs — změna návratového typu
public LoginResult IsRegistered(string name, string passwd)
{
    // ...
    if (user == null) return LoginResult.UserNotFound;
    if (!User.VerifyPassword(passwd, user.UserHash)) return LoginResult.WrongPassword;
    LoggedUserId = user.UserId;
    LoggedUserName = user.UserName;
    return LoginResult.Success;
}

// LoginScreen.xaml.cs — čitelný switch
switch (userManager.IsRegistered(UserNameTextBox.Text, PwdBox.Password))
{
    case LoginResult.UserNotFound:
        MessageBox.Show("Uživatel nenalezen.");
        break;
    case LoginResult.WrongPassword:
        MessageBox.Show("Špatné heslo, opakujte prosím.");
        break;
    case LoginResult.Success:
        new MainWindow(userManager).Show();
        this.Close();
        break;
}
```

---

### 🔧 P2-2: Odstranit DRY porušení — extrahovat `CopyProperties`

Kopírování 14 vlastností `CultureAction` se opakuje na 3 místech. Centralizuj to:

```csharp
// CultureAction.cs — přidat metodu
public void CopyFrom(CultureAction source)
{
    CultureActionName   = source.CultureActionName;
    ActionStartDate     = source.ActionStartDate;
    ActionEndDate       = source.ActionEndDate;
    NumberOfChildren    = source.NumberOfChildren;
    NumberOfAdults      = source.NumberOfAdults;
    NumberOfSeniors     = source.NumberOfSeniors;
    NumberOfDisabled    = source.NumberOfDisabled;
    CultureActionType   = source.CultureActionType;
    ExhibitionType      = source.ExhibitionType;
    ActionPrice         = source.ActionPrice;
    Organiser           = source.Organiser;
    CultureActionNotes  = source.CultureActionNotes;
    IsFree              = source.IsFree;
}

// CultureActionManager.EditAction — zkrácení ze 14 řádků na 1
public void EditAction(CultureAction selected, CultureAction edited, ObservableCollection<CultureAction> collection)
{
    var target = collection.FirstOrDefault(x => x.CultureActionId == selected.CultureActionId);
    target?.CopyFrom(edited);
}

// DatabaseContextManager.MergeDataWithDb — zkrácení ze 14 řádků na 1
if (dbItem != null && localItem != dbItem)
    dbItem.CopyFrom(localItem);
```

---

### 🔧 P2-3: Změnit `float` na `decimal` pro ceny

```csharp
// CultureAction.cs
// PŘED
public float ActionPrice { get; set; }

// PO
public decimal ActionPrice { get; set; }

// CultureActionDetail.xaml.cs — validace
// PŘED
int actionPrice = inputValidation.ValidateNumber(ActionPriceTextBox.Text, "Náklady na akci");

// PO — přidat ValidateDecimal do InputValidation
decimal actionPrice = inputValidation.ValidateDecimal(ActionPriceTextBox.Text, "Náklady na akci");
```

```csharp
// InputValidation.cs — nová metoda
public decimal ValidateDecimal(string input, string itemToValidate)
{
    if (string.IsNullOrWhiteSpace(input))
        throw new ArgumentException($"{itemToValidate} nemůže být prázdný.");
    if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.CurrentCulture, out decimal result))
        return result;
    throw new ArgumentException($"{itemToValidate} není platné číslo.");
}
```

---

### 🔧 P2-4: Přidat potvrzení před smazáním záznamu

```csharp
// MainWindow.xaml.cs
private void RemoveBtn_Click(object sender, RoutedEventArgs e)
{
    if (CultureActionDataGrid.SelectedItem is not CultureAction selected) return;

    var result = MessageBox.Show(
        $"Opravdu chcete smazat akci \"{selected.CultureActionName}\"?",
        "Potvrzení smazání",
        MessageBoxButton.YesNo,
        MessageBoxImage.Warning);

    if (result == MessageBoxResult.Yes)
        cultureActionManager.RemoveItemFromCollection(LocalCollection, selected);
}
```

---

### 🔧 P2-5: Přejmenovat a zpřehledNit identifikátory

| Aktuální | Navrhované | Soubor |
|---|---|---|
| `numberOfChildern` | `numberOfChildren` | CultureAction, CultureActionManager |
| `oraganiser` | `organiser` | CultureAction, CultureActionManager |
| `localColection` | `localCollection` | CultureActionManager.EditAction |
| `button2` | `RefreshBtn` | MainWindow.xaml |
| `getHashEncryption` | `HashPassword` (static) | User.cs |
| `UserManager.manager` | Smazat (never used) | UserManager.cs |

---

### 🔧 P2-6: Přidat databázové migrace

```bash
# Místo EnsureCreated() používat migrace
dotnet ef migrations add InitialCreate
dotnet ef database update
```

```csharp
// App.xaml.cs — místo EnsureCreated
using (var context = new DatabaseContextManager())
{
    context.Database.Migrate(); // aplikuje migrace, nevymaže data
}
```

---

### 🔧 P2-7: Nahradit `Debug.WriteLine` strukturovaným loggerem

```csharp
// CultureActionManager.cs, DatabaseContextManager.cs
// PŘED
Debug.WriteLine("Data updated");
MessageBox.Show("Added to Db"); // UI feedback z business logiky je špatná praxe

// PO — používej ILogger (např. Microsoft.Extensions.Logging)
// Nebo nejjednodušší oprava pro nyní: odstraň MessageBox z Manager tříd,
// nech UI vrstvu reagovat na návratové hodnoty
```

---

### 🔧 P2-8: Opravit překlepy v enum hodnotách

```csharp
// CultureAction.cs
// PŘED
[Description("Zámeká akce")] CastleEvent,       // chybí č
[Description("Porhlídka Zámku")] CastleTour,    // chybí h (Prohlídka)

// PO
[Description("Zámecká akce")] CastleEvent,
[Description("Prohlídka Zámku")] CastleTour,
```

---

### 🔧 P2-9: Opravit potenciální NullReferenceException v `FillFormData`

```csharp
// CultureActionDetail.xaml.cs
// PŘED — může vyhodit výjimku pokud ActionStartDate je null
StartDatePicker.SelectedDate = action.ActionStartDate.Value.ToDateTime(TimeOnly.MinValue);

// PO — bezpečný přístup
StartDatePicker.SelectedDate = action.ActionStartDate.HasValue
    ? action.ActionStartDate.Value.ToDateTime(TimeOnly.MinValue)
    : null;
EndDatePicker.SelectedDate = action.ActionEndDate.HasValue
    ? action.ActionEndDate.Value.ToDateTime(TimeOnly.MinValue)
    : null;
```

---

### 🔧 P2-10: Přidat přechodový konstruktor — oddělit DB přístup od `CultureAction` konstruktoru

```csharp
// CultureAction.cs — AKTUÁLNÍ (špatně: DB přístup v konstruktoru)
internal CultureAction(string actionName, ...)
{
    LastId = GetLastIdFromDb(); // DB call v konstruktoru!
    CultureActionId = LastId;
    ...
}

// PO — použít DB auto-increment (smazat [DatabaseGenerated(None)] a GetLastIdFromDb)
[Key]
// Smazat: [DatabaseGenerated(DatabaseGeneratedOption.None)]
public int CultureActionId { get; set; } // EF Core přiřadí ID automaticky

internal CultureAction(string actionName, ...)
{
    // Žádný DB přístup — čistý konstruktor
    CultureActionName = actionName;
    ...
}

// Smazat: GetLastIdFromDb(), LastId property
```

---

## Priorita 3 — Dlouhodobé (architektura a nové paradigmata)

### 🏗️ P3-1: Implementovat MVVM vzor

Přechod na MVVM je největší refaktorizace, ale přinese největší hodnotu. Doporučuji **CommunityToolkit.Mvvm** (Microsoft).

```bash
dotnet add package CommunityToolkit.Mvvm
```

**Struktura po refaktorizaci:**
```
EventHarbor/
├── Models/
│   ├── User.cs
│   └── CultureAction.cs
├── ViewModels/
│   ├── LoginViewModel.cs
│   ├── MainViewModel.cs
│   └── CultureActionDetailViewModel.cs
├── Views/  (přejmenovat Screen/ → Views/)
│   ├── LoginScreen.xaml
│   ├── MainWindow.xaml
│   └── CultureActionDetail.xaml
├── Services/
│   ├── IUserService.cs / UserService.cs
│   └── ICultureActionService.cs / CultureActionService.cs
└── Data/
    └── AppDbContext.cs
```

**Příklad ViewModel s CommunityToolkit.Mvvm:**
```csharp
// ViewModels/LoginViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class LoginViewModel : ObservableObject
{
    private readonly IUserService _userService;

    [ObservableProperty] private string _userName = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;

    public LoginViewModel(IUserService userService) => _userService = userService;

    [RelayCommand]
    private async Task LoginAsync(string password)
    {
        var result = await _userService.LoginAsync(UserName, password);
        ErrorMessage = result switch
        {
            LoginResult.UserNotFound  => "Uživatel nenalezen.",
            LoginResult.WrongPassword => "Špatné heslo.",
            _                        => string.Empty
        };
    }
}
```

---

### 🏗️ P3-2: Zavést Dependency Injection

```csharp
// App.xaml.cs
using Microsoft.Extensions.DependencyInjection;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();
        services.AddDbContext<AppDbContext>(o =>
            o.UseSqlite($"Data Source={GetDbPath()}"));
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICultureActionService, CultureActionService>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<MainViewModel>();
        Services = services.BuildServiceProvider();

        var loginWindow = new LoginScreen();
        loginWindow.DataContext = Services.GetRequiredService<LoginViewModel>();
        loginWindow.Show();
    }
}
```

---

### 🏗️ P3-3: Přidat unit testy

```bash
dotnet new xunit -n EventHarbor.Tests
dotnet add EventHarbor.Tests package Moq
```

```csharp
// EventHarbor.Tests/UserServiceTests.cs
public class UserServiceTests
{
    [Fact]
    public void HashPassword_ShouldNotEqualPlainText()
    {
        var hash = User.HashPassword("tajneHeslo123");
        Assert.NotEqual("tajneHeslo123", hash);
    }

    [Fact]
    public void VerifyPassword_ShouldReturnTrue_ForCorrectPassword()
    {
        var hash = User.HashPassword("tajneHeslo123");
        Assert.True(User.VerifyPassword("tajneHeslo123", hash));
    }

    [Fact]
    public void VerifyPassword_ShouldReturnFalse_ForWrongPassword()
    {
        var hash = User.HashPassword("tajneHeslo123");
        Assert.False(User.VerifyPassword("spatneHeslo", hash));
    }
}
```

---

### 🏗️ P3-4: Přidat asynchronní volání DB

```csharp
// Všechna DB volání blokují UI vlákno. Přejdi na async/await:
public async Task<List<CultureAction>> GetActionsAsync(int ownerId)
{
    await using var context = new AppDbContext();
    return await context.CultureActions
        .Where(x => x.OwnerId == ownerId)
        .ToListAsync();
}
```

---

## Doporučené pořadí implementace

```
Týden 1:  P1-1 (bcrypt) → P1-2 (ověření reset) → P1-3 (ResetDb) → P1-4 (bug fix)
Týden 2:  P2-1 (enum) → P2-2 (DRY) → P2-3 (decimal) → P2-5 (překlepy)
Týden 3:  P2-4 (potvrzení smazání) → P2-6 (migrace) → P2-8 (typos) → P2-9 (null check)
Dlouhodobě: P3-1 (MVVM) → P3-2 (DI) → P3-3 (testy) → P3-4 (async)
```

> 💡 **Tip pro začátek:** Nejcennější věc, kterou teď můžeš udělat, je **P1-1 + P1-2**. Ostatní jsou vylepšení; toto je bezpečnost.
