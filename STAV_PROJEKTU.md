# STAV PROJEKTU – EventHarbor

> Audit provedl: GitHub Copilot (Quality Auditor mode) | Datum: 2026-02-25

---

## Executive Summary

EventHarbor je desktopová WPF aplikace cílená na evidenci kulturních akcí pro jednoho konkrétního zákazníka. Projekt je ve stavu **funkčního prototypu** — základní CRUD operace fungují, UI je přítomné, SQLite databáze je zapojena přes EF Core. Kód prozrazuje aktivně se rozvíjejícího vývojáře, který se učí: jsou zde dobré instinkty (XML dokumentační komentáře, výjimky, validační třída), ale zároveň kritické bezpečnostní chyby a architektonické vzory, které si zasluhují okamžitou pozornost. Největší alarmující bod je **bezsolné SHA-256 hashování hesel** v kombinaci s **neomezeným resetem hesla bez ověření identity** — tato kombinace by v produkci vedla k triviálnímu převzetí libovolného účtu.

---

## Technický audit

### 1. Architektura

Projekt **neimplementuje žádný architektonický vzor**. Celá logika sídlí v code-behind (`*.xaml.cs`) souborech. Pro WPF je standardem vzor **MVVM** (Model–View–ViewModel), který odděluje prezentační logiku od UI a umožňuje testovatelnost.

```
Aktuální stav:
  View (XAML) ──→ Code-Behind (.cs) ──→ Manager třídy ──→ EF Core ──→ SQLite

Žádoucí stav (MVVM):
  View (XAML) ──→ ViewModel ──→ Service/Repository ──→ EF Core ──→ SQLite
                     ↕
                   Model
```

**Konkrétní problémy:**

| Problém | Soubor | Závažnost |
|---|---|---|
| `DatabaseContextManager` dědí z `DbContext` A zároveň obsahuje business logiku (`MergeDataWithDb`) | `DatabaseContextManager.cs` | Střední |
| `CultureActionManager` přijímá `ObservableCollection` jako parametr a manipuluje s ní přímo | `CultureActionManager.cs` | Střední |
| `MainWindow.cs`, `CultureActionDetail.cs` obsahují view-logic i data-logic | Screen/*.cs | Střední |
| `UserManager.manager` — veřejné pole téhož typu, nikdy nevyužito (pravděpodobný záměr Singleton) | `UserManager.cs:6` | Nízká |

### 2. Bezpečnost

> ⚠️ **Toto je nejkritičtější sekce celého auditu.**

#### 2.1 Slabé hashování hesel (KRITICKÉ)

Hesla jsou hashována pomocí **SHA-256 bez soli (salt)**. SHA-256 je kryptografická hashovací funkce, *nikoli* funkce pro hashování hesel. Chybí:
- **Salt** → útočník může použít předpočítané rainbow tables
- **Iterace (stretching)** → útočník může prohledávat miliony hesel za sekundu

```csharp
// AKTUÁLNÍ KÓD (User.cs:48–53) – NEBEZPEČNÉ
byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(userPasswd));
hash = Convert.ToBase64String(bytes);
```

Správné řešení: **BCrypt, Argon2 nebo PBKDF2** (viz NAVRH_OPRAV.md).

#### 2.2 Reset hesla bez ověření identity (KRITICKÉ)

`ForgotScreen` umožňuje resetovat heslo **komukoliv, kdo zná uživatelské jméno**, bez jakéhokoliv ověření (email, bezpečnostní otázka, stará heslo). Útočník potřebuje pouze hádat/znát username.

```csharp
// ForgotScreen.xaml.cs:42 – stačí zadat username a nové heslo
if (userManager.ResetPassword(UserNameTextBox.Text, PwdBox.Password))
```

#### 2.3 Nesprávný EF Core atribut `[Owned]` na `User`

Atribut `[Owned]` označuje **Value Object** (hodnotový objekt bez vlastní identity). Třída `User` má primární klíč a vlastní tabulku — použití `[Owned]` je sémanticky špatně a může způsobit neočekávané chování EF Core.

#### 2.4 `ResetDbButton` v produkčním UI

`LoginScreen.xaml.cs:93–113` obsahuje tlačítko, které **smaže a znovu vytvoří celou databázi**. Pokud je toto tlačítko přítomno v XAML produkčního buildu, je to kritická chyba.

#### 2.5 Password v paměti jako plain-text

`User.UserPasswd` je `[NotMapped]` string — pole pro plaintext heslo existuje na modelu i po přihlášení.

### 3. Datová vrstva

| Problém | Popis | Závažnost |
|---|---|---|
| Manuální správa ID | `GetLastIdFromDb()` v konstruktoru `CultureAction` — race condition při souběžných zápisech | Střední |
| `[DatabaseGenerated(None)]` | Vypíná DB auto-increment, vynucuje ruční správu ID | Střední |
| `MergeDataWithDb` vždy vrací `false` | Funkce končí `return false;` bez ohledu na úspěch | Střední (bug) |
| `context.Users.ToList().Find(...)` | Načítá **všechny uživatele** do paměti místo `WHERE` SQL dotazu | Střední (výkon) |
| `context.CultureActionsDatabase.Where(...).ToList()` | OK, ale bez asynchronního volání blokuje UI vlákno | Nízká |
| Žádné migrace | Pouze `EnsureCreated()` — při změně schématu nutno smazat DB | Střední |

### 4. Kvalita kódu

#### 4.1 DRY porušení

Kopírování vlastností `CultureAction` se opakuje na **třech místech**:
- `CultureActionManager.EditAction()` (řádky 91–103)
- `DatabaseContextManager.MergeDataWithDb()` (řádky 66–78)
- `CultureActionDetail.FillFormData()` (řádky 154–167)

#### 4.2 Magic numbers

```csharp
// LoginScreen.xaml.cs:63–76 – co znamená -1, 0, 1?
switch (userManager.IsRegistered(...))
{
    case -1: // uživatel nenalezen
    case 0:  // špatné heslo
    case 1:  // OK
}
```

Měl by existovat `enum LoginResult`.

#### 4.3 Typové chyby

- `ActionPrice` je `float` — pro peněžní hodnoty se používá **`decimal`** (přesná aritmetika)
- Enum hodnoty v `CultureActionType`: `"Zámeká akce"` (chybí `č`), `"Porhlídka Zámku"` (chybí `h`)

#### 4.4 Zbytky vývoje v kódu

- `Debug.WriteLine(...)` jako jediná zpětná vazba na mnohých místech
- `button2` — tlačítko bez smysluplného názvu v `MainWindow.xaml:46`
- `//WIP`, `//for development purpose only` komentáře
- Odkomentovaný debug blok v `DatabaseContextManager.cs:36–42`

#### 4.5 Pravopisné chyby (identifikátory)

| Chyba | Správně | Výskyt |
|---|---|---|
| `numberOfChildern` | `numberOfChildren` | CultureAction, CultureActionManager |
| `oraganiser` | `organiser` | CultureAction, CultureActionManager |
| `ObesravableCollection` | `ObservableCollection` | komentář v CultureActionManager |
| `Retunr` | `Return` | komentář v UserManager |
| `localColection` | `localCollection` | CultureActionManager.EditAction |

### 5. Testovatelnost

**Žádné unit testy neexistují.** Veškerá logika je buď přímo v code-behind (netestovatelné) nebo těsně svázána s EF Core kontextem. Třídy nelze izolovat bez refaktorizace.

### 6. Závislosti

| Balíček | Aktuální verze | Nejnovější (8.x) | Poznámka |
|---|---|---|---|
| `Microsoft.EntityFrameworkCore` | 8.0.1 | 8.0.14+ | Zastaralá minor verze |
| `Microsoft.EntityFrameworkCore.Sqlite` | 8.0.1 | 8.0.14+ | Zastaralá minor verze |
| `Microsoft.EntityFrameworkCore.Design` | 8.0.1 | 8.0.14+ | Zastaralá minor verze |
| `Microsoft.EntityFrameworkCore.Tools` | 8.0.1 | 8.0.14+ | Zastaralá minor verze |
| BCrypt/Argon2 | ❌ chybí | — | Nutné pro bezpečné hashování hesel |

---

## Seznam technického dluhu

1. **Žádný architektonický vzor** — vše v code-behind
2. **Nebezpečné hashování hesel** — SHA-256 bez soli
3. **Neomezený reset hesla** — bez ověření identity
4. **`MergeDataWithDb` vrací vždy `false`** — bug
5. **Manuální správa ID** — race condition potenciál
6. **`[Owned]` na entitě** — nesprávné použití EF Core
7. **Žádné migrace** — pouze `EnsureCreated()`
8. **Žádné testy** — nulové pokrytí
9. **`Debug.WriteLine`** místo loggeru
10. **`float` pro peněžní hodnoty** — zaokrouhlovací chyby
11. **Načítání celé tabulky Users** — `ToList().Find()` místo `FirstOrDefault()` v DB
12. **Dev tlačítka v produkčním UI** — `ResetDbButton`, `button2`
13. **DRY porušení** — kopírování vlastností na 3 místech

---

## Metriky

| Kategorie | Hodnocení | Odůvodnění |
|---|---|---|
| **Architektura** | **3/10** | Žádný architektonický vzor, SRP porušení, těsné vazby |
| **Bezpečnost** | **2/10** | Kritické: slabé hashování + neomezený reset hesla |
| **Dokumentace** | **5/10** | XML komentáře jsou přítomny a smysluplné, README existuje; chybí architektonická dokumentace |
| **Čitelnost** | **5/10** | Pojmenování je víceméně konzistentní, ale typy, překlepy a zbytky dev kódu snižují skóre |

> **Celkové hodnocení: 3.75/10** — Solidní základ pro učební projekt. S refaktorizací má potenciál stát se kvalitní aplikací.
