using CommunityToolkit.Mvvm.ComponentModel;
using EventHarbor.Services;

namespace EventHarbor.ViewModels;

public partial class FormViewModel : ObservableObject
{
    private readonly ICultureActionService _service;
    private readonly SessionState _session;

    public FormViewModel(ICultureActionService service, SessionState session)
    {
        _service = service;
        _session = session;
    }
}
