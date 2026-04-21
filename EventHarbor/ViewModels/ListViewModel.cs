using CommunityToolkit.Mvvm.ComponentModel;
using EventHarbor.Services;

namespace EventHarbor.ViewModels;

public partial class ListViewModel : ObservableObject
{
    private readonly ICultureActionService _service;
    private readonly SessionState _session;

    public ListViewModel(ICultureActionService service, SessionState session)
    {
        _service = service;
        _session = session;
    }
}
