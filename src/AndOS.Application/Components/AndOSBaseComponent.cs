using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
namespace AndOS.Application.Components;

public class AndOSBaseComponent : ComponentBase, IAsyncDisposable, IDisposable
{
    #region Injects
    [Inject] private IContextMenuManager ContextMenuManager { get; init; }
    [Inject] protected IJSRuntime JSRuntime { get; init; }
    [Inject] protected IDialogService DialogService { get; init; }
    [Inject] protected IToastService ToastService { get; init; }
    [Inject] protected HttpClient HttpClient { get; init; }
    [Inject] protected IWindowManager WindowManager { get; init; }
    [Inject] protected IProcessManager ProcessManager { get; init; }
    [Inject] protected IFolderService FolderService { get; init; }
    [Inject] protected IFileService FileService { get; init; }
    [Inject] protected IAccountService AccountService { get; init; }
    [Inject] protected IProgramManager ProgramManager { get; init; }
    [Inject] protected IUserPreferenceService UserPreferenceService { get; init; }
    #endregion

    #region Fields
    public bool Selected { get; set; }
    public string IdElement { get; set; }
    public string ColorOnFocus { get; set; } = "#b8b8f3";

    protected string _backgroundColorFocus => Selected ? ColorOnFocus : "transparent";
    protected string _textWrap => Selected ? "unset" : "nowrap";
    protected string _textOverflow => Selected ? "unset" : "hidden";

    protected ILogger<AndOSBaseComponent> _logger;

    protected ElementReference _container { get; set; }

    protected List<MenuItem> _menuItems = [];
    protected List<Func<Task>> _functionsToRunOnAfterRenderAsync = [];
    protected List<Func<Task>> _functionsToRunOnAfterRender = [];
    #endregion

    #region refresh functions
    protected virtual Task refreshAsync()
    {
        StateHasChanged();
        return Task.CompletedTask;
    }

    protected virtual Task refreshAsync(object obj)
    {
        StateHasChanged();
        return Task.CompletedTask;
    }
    #endregion

    #region Focus or Unfocus functions
    [JSInvokable]
    public void ClickOutSide(bool controlPressed, bool shiftPressed, MouseButton mouseButton) => onClickOutSide(controlPressed, shiftPressed, mouseButton);

    protected virtual void onClickOutSide(bool controlPressed, bool shiftPressed, MouseButton mouseButton) => Select(false);

    public virtual void Select(bool select)
    {
        if (select == Selected)
            return;
        Selected = select;
        StateHasChanged();
    }
    #endregion

    #region life-cycle functions
    public bool EnableLoggingOnInitialized { get; set; } = true;
    protected override void OnInitialized()
    {
        if (EnableLoggingOnInitialized)
            _logger.Log(LogLevel.Debug, "Call {0}", nameof(OnInitialized));
        base.OnInitialized();
    }

    public bool EnableLoggingOnInitializedAsync { get; set; } = true;
    protected override Task OnInitializedAsync()
    {
        if (EnableLoggingOnInitializedAsync)
            _logger.Log(LogLevel.Debug, "Call {0}", nameof(OnInitializedAsync));
        return base.OnInitializedAsync();
    }

    public bool EnableLoggingOnAfterRenderAsync { get; set; } = true;
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (EnableLoggingOnAfterRenderAsync)
            _logger.Log(LogLevel.Debug, "Call {0}", nameof(OnAfterRenderAsync));
        if (firstRender)
            await JSRuntime.InvokeVoidAsync("addClickOutsideListener", _container, DotNetObjectReference.Create(this));

        if (_functionsToRunOnAfterRenderAsync.Any())
            foreach (var func in _functionsToRunOnAfterRenderAsync.ToList())
                try
                {
                    await func();
                }
                catch (Exception e)
                {
                    _logger.LogError(null, e);
                    ToastService.ShowError($"Error on the run {func.Method.Name} in {nameof(OnAfterRenderAsync)}:\n{e.Message}");
                }
                finally
                {
                    _functionsToRunOnAfterRender.Remove(func);
                }
    }

    public bool EnableLoggingOnAfterRender { get; set; } = true;
    protected override void OnAfterRender(bool firstRender)
    {
        if (EnableLoggingOnAfterRender)
            _logger.Log(LogLevel.Debug, "Call {0}", nameof(OnAfterRender));
        if (_functionsToRunOnAfterRender.Any())
            foreach (var func in _functionsToRunOnAfterRender.ToList())
                try
                {
                    func();
                }
                catch (Exception e)
                {
                    _logger.LogError(null, e);
                    ToastService.ShowError($"Error on the run {func.Method.Name} in {nameof(OnAfterRender)}:\n{e.Message}");
                }
                finally
                {
                    _functionsToRunOnAfterRender.Remove(func);
                }
    }

    public bool EnableLoggingDispose { get; set; } = true;
    public virtual void Dispose()
    {
        if (EnableLoggingDispose)
            _logger.Log(LogLevel.Debug, "Call {0}", nameof(Dispose));
    }

    public bool EnableLoggingDisposeAsync { get; set; } = true;
    public virtual ValueTask DisposeAsync()
    {
        if (EnableLoggingDisposeAsync)
            _logger.Log(LogLevel.Debug, "Call {0}", nameof(DisposeAsync));
        return ValueTask.CompletedTask;
    }
    #endregion

    #region ContextMenu functions
    protected async virtual Task ShowMenuItemsAsync(MouseEventArgs e)
    {
        if (ContextMenuManager.Open)
            return;
        await ContextMenuManager.SetItems(_menuItems, e);
        await OnShowMenuItemsAsync(e);
    }

    protected virtual Task OnShowMenuItemsAsync(MouseEventArgs e)
    {
        Selected = true;
        return Task.CompletedTask;
    }
    #endregion
}