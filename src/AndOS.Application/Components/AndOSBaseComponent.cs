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

    protected string _backgroundColorFocus => this.Selected ? this.ColorOnFocus : "transparent";
    protected string _textWrap => this.Selected ? "unset" : "nowrap";
    protected string _textOverflow => this.Selected ? "unset" : "hidden";

    protected ILogger<AndOSBaseComponent> _logger;

    protected ElementReference _container { get; set; }

    protected List<MenuItem> _menuItems = [];
    protected List<Func<Task>> _functionsToRunOnAfterRenderAsync = [];
    protected List<Func<Task>> _functionsToRunOnAfterRender = [];
    #endregion

    #region refresh functions
    protected virtual Task refreshAsync()
    {
        this.StateHasChanged();
        return Task.CompletedTask;
    }

    protected virtual Task refreshAsync(object obj)
    {
        this.StateHasChanged();
        return Task.CompletedTask;
    }
    #endregion

    #region Focus or Unfocus functions
    [JSInvokable]
    public void ClickOutSide(bool controlPressed, bool shiftPressed, MouseButton mouseButton) => this.onClickOutSide(controlPressed, shiftPressed, mouseButton);

    protected virtual void onClickOutSide(bool controlPressed, bool shiftPressed, MouseButton mouseButton) => this.Select(false);

    public virtual void Select(bool select)
    {
        if (select == this.Selected)
            return;
        this.Selected = select;
        this.StateHasChanged();
    }
    #endregion

    #region life-cycle functions
    public bool EnableLoggingOnInitialized { get; set; } = true;
    protected override void OnInitialized()
    {
        if (this.EnableLoggingOnInitialized)
            this._logger.Log(LogLevel.Debug, "Call {0}", nameof(OnInitialized));
        base.OnInitialized();
    }

    public bool EnableLoggingOnInitializedAsync { get; set; } = true;
    protected override Task OnInitializedAsync()
    {
        if (this.EnableLoggingOnInitializedAsync)
            this._logger.Log(LogLevel.Debug, "Call {0}", nameof(OnInitializedAsync));
        return base.OnInitializedAsync();
    }

    public bool EnableLoggingOnAfterRenderAsync { get; set; } = true;
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (this.EnableLoggingOnAfterRenderAsync)
            this._logger.Log(LogLevel.Debug, "Call {0}", nameof(OnAfterRenderAsync));
        if (firstRender)
            await this.JSRuntime.InvokeVoidAsync("addClickOutsideListener", this._container, DotNetObjectReference.Create(this));

        if (this._functionsToRunOnAfterRenderAsync.Any())
            foreach (var func in this._functionsToRunOnAfterRenderAsync.ToList())
                try
                {
                    await func();
                }
                catch (Exception e)
                {
                    this._logger.LogError(null, e);
                    this.ToastService.ShowError($"Error on the run {func.Method.Name} in {nameof(OnAfterRenderAsync)}:\n{e.Message}");
                }
                finally
                {
                    this._functionsToRunOnAfterRender.Remove(func);
                }
    }

    public bool EnableLoggingOnAfterRender { get; set; } = true;
    protected override void OnAfterRender(bool firstRender)
    {
        if (this.EnableLoggingOnAfterRender)
            this._logger.Log(LogLevel.Debug, "Call {0}", nameof(OnAfterRender));
        if (this._functionsToRunOnAfterRender.Any())
            foreach (var func in this._functionsToRunOnAfterRender.ToList())
                try
                {
                    func();
                }
                catch (Exception e)
                {
                    this._logger.LogError(null, e);
                    this.ToastService.ShowError($"Error on the run {func.Method.Name} in {nameof(OnAfterRender)}:\n{e.Message}");
                }
                finally
                {
                    this._functionsToRunOnAfterRender.Remove(func);
                }
    }

    public bool EnableLoggingDispose { get; set; } = true;
    public virtual void Dispose()
    {
        if (this.EnableLoggingDispose)
            this._logger.Log(LogLevel.Debug, "Call {0}", nameof(Dispose));
    }

    public bool EnableLoggingDisposeAsync { get; set; } = true;

    public virtual ValueTask DisposeAsync()
    {
        if (this.EnableLoggingDisposeAsync)
            this._logger.Log(LogLevel.Debug, "Call {0}", nameof(DisposeAsync));
        return ValueTask.CompletedTask;
    }
    #endregion

    #region ContextMenu functions
    protected virtual async Task ShowMenuItemsAsync(MouseEventArgs e)
    {
        if (this.ContextMenuManager.Open)
            return;
        await this.ContextMenuManager.SetItems(this._menuItems, e);
        await this.OnShowMenuItemsAsync(e);
    }

    protected virtual Task OnShowMenuItemsAsync(MouseEventArgs e)
    {
        this.Selected = true;
        return Task.CompletedTask;
    }
    #endregion
}