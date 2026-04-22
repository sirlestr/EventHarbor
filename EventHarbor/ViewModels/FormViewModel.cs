using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventHarbor.Domain;
using EventHarbor.Services;

namespace EventHarbor.ViewModels;

public partial class FormViewModel : ObservableObject
{
    // Reasonable bounds to block accidental typos without blocking legitimate planning horizons.
    private const decimal MaxCost = 100_000_000m;
    private const int MaxNotesLength = 500;
    private static readonly DateTime MinAllowedDate = new(2000, 1, 1);
    private static readonly DateTime MaxAllowedDate = new(DateTime.Today.Year + 20, 12, 31);

    private readonly ICultureActionService _service;
    private readonly SessionState _session;

    public event EventHandler? Saved;
    public event EventHandler? Cancelled;

    private int? _editingId;

    [ObservableProperty]
    private bool _isNew = true;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private CultureEventType _type = CultureEventType.Exhibition;

    [ObservableProperty]
    private ExhibitionType _exhibition = ExhibitionType.General;

    [ObservableProperty]
    private Organiser _organiser = Organiser.Museum;

    [ObservableProperty]
    private DateTime? _startDate = DateTime.Today;

    [ObservableProperty]
    private string _startTime = "10:00";

    [ObservableProperty]
    private DateTime? _endDate = DateTime.Today;

    [ObservableProperty]
    private string _endTime = "18:00";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalAttendance))]
    [NotifyPropertyChangedFor(nameof(PreviewEvent))]
    private int _children;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalAttendance))]
    [NotifyPropertyChangedFor(nameof(PreviewEvent))]
    private int _adults;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalAttendance))]
    [NotifyPropertyChangedFor(nameof(PreviewEvent))]
    private int _seniors;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalAttendance))]
    [NotifyPropertyChangedFor(nameof(PreviewEvent))]
    private int _disabled;

    [ObservableProperty]
    private decimal _cost;

    [ObservableProperty]
    private bool _isFree;

    [ObservableProperty]
    private string _notes = string.Empty;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _isBusy;

    public int TotalAttendance => Children + Adults + Seniors + Disabled;

    // Preview CultureAction for AttendanceBar binding
    public CultureAction PreviewEvent => new()
    {
        Children = Children,
        Adults = Adults,
        Seniors = Seniors,
        Disabled = Disabled,
    };

    public IReadOnlyList<CultureEventType> AvailableTypes { get; } = Enum.GetValues<CultureEventType>();
    public IReadOnlyList<ExhibitionType> AvailableExhibitions { get; } = Enum.GetValues<ExhibitionType>();

    public FormViewModel(ICultureActionService service, SessionState session)
    {
        _service = service;
        _session = session;
    }

    public void Initialize(CultureAction? source)
    {
        if (source is null)
        {
            _editingId = null;
            IsNew = true;
            return;
        }

        _editingId = source.Id;
        IsNew = false;
        Name = source.Name;
        Type = source.Type;
        Exhibition = source.Exhibition;
        Organiser = source.Organiser;
        if (source.StartAt.HasValue)
        {
            StartDate = source.StartAt.Value.Date;
            StartTime = source.StartAt.Value.ToString("HH:mm");
        }
        if (source.EndAt.HasValue)
        {
            EndDate = source.EndAt.Value.Date;
            EndTime = source.EndAt.Value.ToString("HH:mm");
        }
        Children = source.Children;
        Adults = source.Adults;
        Seniors = source.Seniors;
        Disabled = source.Disabled;
        Cost = source.Cost;
        IsFree = source.IsFree;
        Notes = source.Notes ?? string.Empty;
    }

    [RelayCommand]
    private void SetOrganiser(string org)
    {
        if (Enum.TryParse<Organiser>(org, out var parsed)) Organiser = parsed;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Name))
        {
            ErrorMessage = "Vyplňte název akce.";
            return;
        }

        if (!TryCombineDateTime(StartDate, StartTime, out var start) ||
            !TryCombineDateTime(EndDate, EndTime, out var end))
        {
            ErrorMessage = "Zkontrolujte formát data a času (HH:mm).";
            return;
        }

        if (start < MinAllowedDate || end > MaxAllowedDate)
        {
            ErrorMessage = $"Datum musí být mezi {MinAllowedDate:yyyy} a {MaxAllowedDate:yyyy}.";
            return;
        }

        if (end < start)
        {
            ErrorMessage = "Konec akce musí být stejný nebo pozdější než začátek.";
            return;
        }

        if (Children < 0 || Adults < 0 || Seniors < 0 || Disabled < 0)
        {
            ErrorMessage = "Počty návštěvníků nemohou být záporné.";
            return;
        }

        if (Cost < 0)
        {
            ErrorMessage = "Náklady nemohou být záporné.";
            return;
        }

        if (Cost > MaxCost)
        {
            ErrorMessage = $"Náklady nemohou přesáhnout {MaxCost:N0} Kč.";
            return;
        }

        if (!string.IsNullOrEmpty(Notes) && Notes.Length > MaxNotesLength)
        {
            ErrorMessage = $"Poznámky mohou mít nejvýše {MaxNotesLength} znaků (nyní {Notes.Length}).";
            return;
        }

        try
        {
            IsBusy = true;

            if (IsNew)
            {
                var entity = new CultureAction
                {
                    Name = Name.Trim(),
                    Type = Type,
                    Exhibition = Exhibition,
                    Organiser = Organiser,
                    StartAt = start,
                    EndAt = end,
                    Children = Children,
                    Adults = Adults,
                    Seniors = Seniors,
                    Disabled = Disabled,
                    Cost = Cost,
                    IsFree = IsFree,
                    Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim(),
                    OwnerId = _session.UserId,
                };
                await _service.CreateAsync(entity);
            }
            else
            {
                var existing = await _service.GetByIdAsync(_editingId!.Value);
                if (existing is null)
                {
                    ErrorMessage = "Záznam se nepodařilo načíst, zkuste to znovu.";
                    return;
                }
                existing.Name = Name.Trim();
                existing.Type = Type;
                existing.Exhibition = Exhibition;
                existing.Organiser = Organiser;
                existing.StartAt = start;
                existing.EndAt = end;
                existing.Children = Children;
                existing.Adults = Adults;
                existing.Seniors = Seniors;
                existing.Disabled = Disabled;
                existing.Cost = Cost;
                existing.IsFree = IsFree;
                existing.Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim();
                await _service.UpdateAsync(existing);
            }

            Saved?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void Cancel() => Cancelled?.Invoke(this, EventArgs.Empty);

    private static bool TryCombineDateTime(DateTime? date, string time, out DateTime result)
    {
        result = default;
        if (!date.HasValue) return false;
        if (!TimeSpan.TryParseExact(time, new[] { @"h\:mm", @"hh\:mm" }, System.Globalization.CultureInfo.InvariantCulture, out var t))
            return false;
        result = date.Value.Date + t;
        return true;
    }
}
