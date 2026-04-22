namespace EventHarbor.Services;

public static class SecurityQuestions
{
    public static IReadOnlyList<string> All { get; } = new[]
    {
        "Jméno prvního domácího mazlíčka",
        "Rodné jméno matky",
        "Město narození",
        "Název první základní školy",
        "Oblíbená kniha z dětství",
        "Jméno nejlepšího kamaráda z dětství",
    };
}
