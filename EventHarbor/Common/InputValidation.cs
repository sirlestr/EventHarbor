using System.Globalization;

namespace EventHarbor.Common;

public static class InputValidation
{
    public static string ValidateText(string? input, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException($"{fieldName} nemůže být prázdný, zadej prosím hodnotu");
        return input.Trim();
    }

    public static int ValidateInt(string? input, string fieldName, int min = int.MinValue, int max = int.MaxValue)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException($"{fieldName} není zadané, zkontroluj hodnoty");

        if (!int.TryParse(input, NumberStyles.Integer, CultureInfo.CurrentCulture, out var value))
            throw new ArgumentException($"{fieldName} není platné celé číslo");

        if (value < min || value > max)
            throw new ArgumentException($"{fieldName} musí být v rozsahu {min}–{max}");

        return value;
    }

    public static decimal ValidateDecimal(string? input, string fieldName, decimal min = decimal.MinValue, decimal max = decimal.MaxValue)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException($"{fieldName} není zadané, zkontroluj hodnoty");

        // Accept both user-current culture and invariant form (so "19,50" and "19.50" both work).
        if (!decimal.TryParse(input, NumberStyles.Any, CultureInfo.CurrentCulture, out var value) &&
            !decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            throw new ArgumentException($"{fieldName} není platné číslo");

        if (value < min || value > max)
            throw new ArgumentException($"{fieldName} musí být v rozsahu {min}–{max}");

        return value;
    }

    public static void ValidateDateRange(DateTime? start, DateTime? end, string startName = "Začátek", string endName = "Konec")
    {
        if (!start.HasValue)
            throw new ArgumentException($"{startName} nemůže být prázdný");
        if (!end.HasValue)
            throw new ArgumentException($"{endName} nemůže být prázdný");
        if (end.Value < start.Value)
            throw new ArgumentException($"{endName} musí být stejný nebo pozdější než {startName}");
    }
}
