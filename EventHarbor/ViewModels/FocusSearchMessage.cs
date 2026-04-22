namespace EventHarbor.ViewModels;

/// <summary>
/// Broadcast when the user presses Ctrl+K on the main shell.
/// ListView subscribes and focuses its search input (if currently on the list).
/// </summary>
public sealed class FocusSearchMessage;
