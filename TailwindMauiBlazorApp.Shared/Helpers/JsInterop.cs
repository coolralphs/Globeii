using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TailwindMauiBlazorApp.Core.Models.Entities;
using TailwindMauiBlazorApp.Shared.Components;
using TailwindMauiBlazorApp.Shared.Models;
using TailwindMauiBlazorApp.Shared.Models.ViewModels;
using TailwindMauiBlazorApp.Shared.Pages.Itinerary;

namespace TailwindMauiBlazorApp.Shared.Helpers;

public class JsInterop : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _module;
    private bool _moduleDisposed = false;
    private IJSObjectReference? _themeModule;

    public JsInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        _module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/TailwindMauiBlazorApp.Shared/js/interop.js");
        _themeModule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/TailwindMauiBlazorApp.Shared/js/theme.js");
    }

    public async Task SetTheme(string theme)
    {
        if (_themeModule == null)
            await InitializeAsync();

        if (_themeModule != null)
            await _themeModule.InvokeVoidAsync("setTheme", theme);
    }

    public async Task LoadScript(string src)
    {
        try
        {

            if (_module == null || _moduleDisposed)
                await InitializeAsync();

            if (_module != null && !_moduleDisposed)
                await _module.InvokeAsync<Task>("loadScript", src);
        }
        catch (Exception ex)
        {

        }
        //await _module.InvokeVoidAsync("loadScript", src);
    }

    private IJSObjectReference? _globeIntroInstance;
    private IJSObjectReference? _globeItineraryInstance;
    public async Task<IJSObjectReference?> CreateIntroGlobe(string elementId)
    {
        if (_module == null)
            await InitializeAsync();

        if (_module != null)
        {
            _globeIntroInstance = await _module.InvokeAsync<IJSObjectReference>("createIntroGlobe", elementId);
            return _globeIntroInstance;
        }

        return null;
    }

    public async Task DestroyIntroGlobe()
    {
        if (_globeIntroInstance != null)
        {
            try
            {
                await _globeIntroInstance.InvokeVoidAsync("destroy");
                await _globeIntroInstance.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Circuit disconnected, ignore this error on dispose
            }
            catch (Exception ex)
            {
                // Optionally log other errors
                Console.Error.WriteLine($"Error destroying globe: {ex.Message}");
            }
            finally
            {
                _globeIntroInstance = null;
            }
        }
    }

    public async Task DestroyItineraryGlobe()
    {
        if (_globeItineraryInstance != null)
        {
            try
            {
                await _globeItineraryInstance.InvokeVoidAsync("DestroyItineraryGlobe");
            }
            catch (JSDisconnectedException)
            {
                // Safe to ignore, happens if page is unloading
            }
            catch (ObjectDisposedException)
            {
                // Safe to ignore, object already disposed
            }

            await _globeItineraryInstance.DisposeAsync();
            _globeItineraryInstance = null;
        }
    }

    public async Task CreateItineraryGlobe(string elementId)
    {
        if (_module == null)
            await InitializeAsync();

        if (_module != null)
            await _module.InvokeVoidAsync("createItineraryGlobe", elementId);
    }

    public async Task<IJSObjectReference?> CreateGlobe(string elementId, ICollection<ItineraryPlaceViewModel> places)
    {
        if (_module == null)
            await InitializeAsync();

        if (_module != null)
            //await _module.InvokeVoidAsync("createGlobe", elementId, places);
            _globeItineraryInstance = await _module.InvokeAsync<IJSObjectReference>("createGlobe", elementId, places);
        return _globeItineraryInstance;
    }

    public async Task ScrollToElementById(string elementId)
    {
        if (_module == null)
            await InitializeAsync();

        if (_module != null)
            await _module.InvokeVoidAsync("scrollToElementById", elementId);
    }

    public async Task ScrollButtonRow(string elementId, int direction)
    {
        if (_module == null)
            await InitializeAsync();

        if (_module != null)
            await _module.InvokeVoidAsync("scrollButtonRow", elementId, direction);
    }

    public async Task<(int ScrollLeft, int ScrollWidth, int ClientWidth)> GetScrollInfo(string elementId)
    {
        if (_module == null)
            await InitializeAsync();

        var result = await _module.InvokeAsync<ScrollInfo>("getScrollInfo", elementId);
        return (result.ScrollLeft, result.ScrollWidth, result.ClientWidth);
    }

    public async Task RegisterResizeHandler(DotNetObjectReference<ItineraryPlaces> objRef)
    {
        if (_module == null)
            await InitializeAsync();

        if (_module != null)
            await _module.InvokeVoidAsync("registerResizeHandler", objRef);
    }

    public async Task InitOffcanvasOutsideClick(string offcanvasId, string modalId, DotNetObjectReference<ItineraryPlaces> objRef)
    {
        if (_module == null)
            await InitializeAsync();

        if (_module != null)
            await _module.InvokeVoidAsync("initOffcanvasOutsideClick", offcanvasId, modalId, objRef);
    }

    public async Task OpenOffcanvasHalfScreen(string elementId)
    {
        if (_module == null)
            await InitializeAsync();

        if (_module != null)
            await _module.InvokeVoidAsync("openOffcanvasHalfScreen", elementId);
    }

    public async Task ToggleOffcanvasFullScreen(string elementId, bool isFullScreen)
    {
        if (_module == null)
            await InitializeAsync();

        if (_module != null)
            await _module.InvokeVoidAsync("toggleOffcanvasFullScreen", elementId, isFullScreen);
    }

    public async Task CloseOffcanvas(string offcanvastId)
    {
        if (_module == null)
            await InitializeAsync();

        if (_module != null)
            await _module.InvokeVoidAsync("closeOffcanvas", offcanvastId);
    }

    public async Task RecreateAutocomplete(DotNetObjectReference<ItineraryPlaces> objRef)
    {
        if (_module == null)
            await InitializeAsync();

        if (_module != null)
            await _module.InvokeVoidAsync("recreateAutocomplete", objRef);
    }

    public async Task RegisterOffcanvasCloseHandler<T>(string offcanvasId, DotNetObjectReference<T> objRef) where T : class
        //public async Task RegisterOffcanvasCloseHandler(string offcanvasId, DotNetObjectReference<ItineraryPlaces> objRef)
    {
        if (_module == null)
            await InitializeAsync();

        if (_module != null)
            await _module.InvokeVoidAsync("registerOffcanvasCloseHandler", offcanvasId, objRef);
    }

    public async Task FitGlobe()
    {
        if (_module == null)
            await InitializeAsync();

        if (_module != null)
            await _module.InvokeVoidAsync("fitGlobe");
    }

    public async Task FitGlobeToCoordinates(List<LatLng> coordinates)
    {
        if (_module == null)
            await InitializeAsync();

        if (_module != null)
            await _module.InvokeVoidAsync("fitGlobeToCoordinates", coordinates);
    }

    public async Task<bool> IsIPhone()
    {
        if (_module == null)
            await InitializeAsync();

        if (_module != null)
            return await _module.InvokeAsync<bool>("isIPhone");

        return false;
    }

    public async Task<(bool, bool)> GetScrollState(string elementId)
    {
        if (_module == null)
            await InitializeAsync();

        if (_module != null)
        {
            var result = await _module.InvokeAsync<bool[]>("getScrollState", elementId);

            if (result != null && result.Length == 2)
            {
                return (result[0], result[1]);
            }
        }

        return (false, false);
    }

    public class ScrollInfo
    {
        public int ScrollLeft { get; set; }
        public int ScrollWidth { get; set; }
        public int ClientWidth { get; set; }
    }

    public async Task InitAutocompleteElement(DotNetObjectReference<ItineraryPlaces> dotNetObj)
    {
        if (_module == null)
            await InitializeAsync();

        if (_module != null)
            await _module.InvokeVoidAsync("initAutocompleteElement", dotNetObj);
    }

    public async Task InitAutocompleteElementById<T>(DotNetObjectReference<T> dotNetObj, string elementId)
        where T : class
    {
        if (_module == null)
            await InitializeAsync();

        if (_module != null)
            await _module.InvokeVoidAsync("initAutocompleteElementById", dotNetObj, elementId);
    }

    public async Task AddMarkerToGlobe(object newMarker, string iconType = "place")
    {
        if (_module == null)
            await InitializeAsync();

        if (_module != null)
            await _module.InvokeVoidAsync("addMarkerToGlobe", newMarker, iconType);

    }

    public async Task RemoveMarkerFromGlobe(Guid markerId)
    {
        if (_module == null)
            await InitializeAsync();

        if (_module != null)
            await _module.InvokeVoidAsync("removeMarkerFromGlobe", markerId);

    }

    public async Task UpdateBottomSheetHeight(string height)
    {
        if (_module == null)
            await InitializeAsync();

        if (_module != null)
            await _module.InvokeVoidAsync("updateBottomSheetHeight", height);

    }

    public async Task RegisterOutsideClickHandler<T>(string offcanvasId, DotNetObjectReference<T> dotNetObj) where T : class
    {
        if (_module == null)
            await InitializeAsync();

        if (_module != null)
            await _module.InvokeVoidAsync("registerOutsideClickHandler", offcanvasId, dotNetObj);

    }

    public async ValueTask BlurActiveElementAsync()
    {
        if (_module == null)
        {
            await InitializeAsync();
        }
        if (_module != null)
            await _module!.InvokeVoidAsync("blurActiveElement");
    }

    //public async Task EnableHorizontalDragScroll(string elementId)
    //{
    //    if (_module == null)
    //        await InitializeAsync();

    //    if (_module != null)
    //        await _module.InvokeVoidAsync("enableHorizontalDragScroll", elementId);

    //}
    public async Task RemoveAllListeners()
    {
        try
        {

            if (_module == null)
                await InitializeAsync();

            if (_module != null)
                await _module.InvokeVoidAsync("removeAllListeners");
        }
        catch (Exception ex)
        {

        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await DestroyIntroGlobe();
            await DestroyItineraryGlobe();
            // Dispose any autocomplete listeners
            if (_module != null)
            {
                try
                {
                    await _module.InvokeVoidAsync("disposeAutocomplete");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ JS disposeAutocomplete error: {ex.Message}");
                }

                // Now dispose the module itself
                await _module.DisposeAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ JSInterop DisposeAsync error: {ex.Message}");
        }
        finally
        {
            _module = null;
            _moduleDisposed = true;
        }
    }
}
