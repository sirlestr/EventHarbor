using CommunityToolkit.Mvvm.ComponentModel;
using EventHarbor.Domain;
using EventHarbor.Services;

namespace EventHarbor.ViewModels;

public partial class FormViewModel : ObservableObject
{
    private readonly ICultureActionService _service;
    private readonly SessionState _session;

    [ObservableProperty]
    private CultureAction? _source;

    [ObservableProperty]
    private bool _isNew = true;

    public FormViewModel(ICultureActionService service, SessionState session)
    {
        _service = service;
        _session = session;
    }

    public void Initialize(CultureAction? source)
    {
        Source = source;
        IsNew = source is null;
    }
}
